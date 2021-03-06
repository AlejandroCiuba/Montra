using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour
{
    private PlayerController pc;
    // Start is called before the first frame update
    void Awake()
    {
        SetControls();
    }
    void OnEnable() {pc.Controls.Enable();}
    void OnDisable() {pc.Controls.Disable();}

    void Update()
    {
        onGround = IsGrounded();
        if(pressedJump && (onGround || jumpTimer < jumpDelay) && !isJumping) Jump();
        if(!onGround && !isJumping) jumpTimer += Time.deltaTime;
    }
    void FixedUpdate()  
    {
        Move();
        if(!onGround && prb.velocity.y < 0) pressedJump = false;
        if(onGround && !pressedJump) {isJumping = false; jumpTimer = 0;}
        ModifyPhysics();
    }

    void LateUpdate()
    {
        AnimationControls();
        if(!enableControls) pc.Disable();
        else if(enableControls) pc.Enable();
    }

    //======= MOVEMENT COMPONENTS =======
    [Header("Movement Components")]
    [SerializeField] private Rigidbody2D prb;
    [SerializeField] private Collider2D pcol;
    [SerializeField] private LayerMask jumpLayer;
    private Vector2 movement;

    [Header("Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeed = 15f;
    [SerializeField] private float jumpDelay = 0.25f;
    [SerializeField] public bool enableControls = true;
    private float jumpTimer;
    [Header("Physics Variable")]
    [SerializeField] private float gravity = 1f;
    [SerializeField] private float fall = 5f;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool onGround;
    [Range(0f, 1f)]
    [SerializeField] private float groundLength = 0.6f;
    [SerializeField] private Vector3 offset;
    private bool pressedJump;

    private void Move() {prb.velocity = new Vector2(movement.x * speed * Time.deltaTime, prb.velocity.y);}

    private void Jump()
    {
        prb.velocity = new Vector2(prb.velocity.x, 0);
        prb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        jumpTimer = jumpDelay;
        isJumping = true;
    }
    private bool IsGrounded() {return Physics2D.BoxCast(pcol.bounds.center, pcol.bounds.size + offset, 0f, Vector2.down, groundLength, jumpLayer).collider != null;}
    private void ModifyPhysics()
    {
        if(onGround) prb.gravityScale = 0f;
        else {
            prb.gravityScale = gravity;
            if(prb.velocity.y < 0) prb.gravityScale = gravity * fall;
            else if(prb.velocity.y > 0 && !pressedJump) prb.gravityScale = gravity * (fall / 1.25f);
            else if(prb.velocity.y > 0 && pressedJump) prb.gravityScale = gravity * (fall / 2f);
        }
    }

    private void SetControls() 
    {
        pc = new PlayerController();
        pc.Controls.Walk.performed += cxt => movement = (cxt.ReadValue<Vector2>().x >= 0)? Vector2.right : Vector2.left; 
        pc.Controls.Walk.canceled += cxt => movement = Vector2.zero;
        pc.Controls.Jump.started += cxt => pressedJump = true;
        pc.Controls.Jump.canceled += cxt => pressedJump = false;
    }
    //======= ANIMATION COMPONENTS =======
    [Space]
    [Header("Animation Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer player;
    [SerializeField] private SpriteRenderer head;
    [SerializeField] private Sprite[] heads;
    private bool blink = false;

    private void AnimationControls()
    {
        if(movement.x == -1) {player.flipX = true; head.flipX = true;}
        else if(movement.x == 1) {player.flipX = false; head.flipX = false;}

        if(movement.x != 0 && (IsGrounded() || jumpTimer < jumpDelay)) {
            anim.SetBool("isRunning", true);
            head.sprite = heads[1];

            if(movement.x == -1) anim.SetBool("isLeft", true);
            else if(movement.x == 1)anim.SetBool("isLeft", false);
        }
        else {
            anim.SetBool("isRunning", false);
            head.sprite = heads[0];
        }

        if(!IsGrounded() && jumpTimer >= jumpDelay) anim.SetBool("isJumping", true);
        else if(IsGrounded()) anim.SetBool("isJumping", false);

        //Blinking
        var ran = new System.Random();
        if(!anim.GetBool("IsRunning") && ran.NextDouble() > .75f && !blink) StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        blink = true;
        head.sprite = heads[2];
        yield return new WaitForSeconds(.75f);
        blink = false;
    }

    //======= VISUALIZATION FUNCTIONS =======
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pcol.bounds.center + Vector3.down * groundLength, pcol.bounds.size);
    }
}

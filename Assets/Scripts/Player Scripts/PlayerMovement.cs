using UnityEngine; //Mandatory

public class PlayerMovement : MonoBehaviour //All Unity Scripts inherit from a certain group of Objects, MonoBehaviour is the most common
{
    //Update is called once per frame
    void Update() 
    {
        AnimationControl();
        IsGrounded();
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded) Jump();
        if(Input.GetKey(KeyCode.Space) && jumpCounter > 0 && isJumping) VarJump();
        if(Input.GetKeyUp(KeyCode.Space)) jumpCounter = 0;
        UnityEngine.Debug.Log(isGrounded + " " + isJumping);
    }

    //FixedUpdate is called consistently regardless of FPS (every .02 seconds per gametime)
    //Used for things that must be consistent: Physics mostly
    //But the way movement is done here also benefits from it
    void FixedUpdate()
    {
        Move(); //Moves the player
        Fall();
    }

    //================== Movement Controls ==================
    //Player's RigidBody Component
    //[SerializeField] means we can see that variable in the editor and assign it values/objects there
    [SerializeField] private Rigidbody2D prb;
    //Player's Collider Component (Any 2D Shape)
    [SerializeField] private Collider2D pcol;
    [SerializeField] private LayerMask jumpLayer;
    [SerializeField] private float jumpHeight = 10f;
    [SerializeField] private float forgive = .1f; private float forgiveCounter;
    [SerializeField] private float jumpTime; private float jumpCounter;
    private bool isGrounded; private bool isJumping;
    [SerializeField] private float speed = 1000f;
    //The 2 dimensional vector which tracks the players x and y movement
    private Vector2 movement;
    
    //Multiply the direction from Input.GetAxisRaw with our player's speed and Time.deltaTime
    //Time.deltaTime is common for physics related actions and makes sure things are consistent
    //regardless of current frame rate. Otherwise, the player could go faster if their is a boost in frames
    private void Move() 
    {
        movement.x = Input.GetAxisRaw("Horizontal"); //returns either 1 or -1 depending on A key or D key
        prb.velocity = new Vector2(movement.x * speed * Time.deltaTime, prb.velocity.y);
    }

    private void Jump()
    {
        prb.velocity += jumpHeight * Vector2.up;
        isJumping = true;
    }

    private void VarJump()
    {
        prb.velocity += Vector2.up * .25f;
        jumpCounter -= Time.deltaTime;
    }

    private void IsGrounded() 
    {   
        RaycastHit2D ray = Physics2D.BoxCast(pcol.bounds.center, pcol.bounds.size, 0f, Vector2.down, 1f, jumpLayer);
        if(ray.collider != null) {
            isGrounded = true;
            if(Input.GetKey(KeyCode.Space)) isJumping = true;
            else isJumping = false;
            forgiveCounter = forgive;
        }
        else if(ray.collider == null && !isJumping && forgiveCounter > 0) {
            isGrounded = true;
            forgiveCounter -= Time.deltaTime;
        }
        else isGrounded = false;
    }

    //Once the player starts coming down from a jump, make them fall faster
    private void Fall() 
    {
        prb.velocity += new Vector2(prb.velocity.x, Physics2D.gravity.y * Time.deltaTime);
    }

//================== Animation Controls ==================
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer player;
    [SerializeField] private SpriteRenderer head;
    [SerializeField] private Sprite[] heads;

    private void AnimationControl()
    {
        if(movement.x == -1) {player.flipX = true; head.flipX = true;}
        else if(movement.x == 1) {player.flipX = false; head.flipX = false;}

        if(movement.x != 0 && isGrounded) {
            anim.SetBool("isRunning", true);
            head.sprite = heads[1];

            if(movement.x == -1) anim.SetBool("isLeft", true);
            else if(movement.x == 1)anim.SetBool("isLeft", false);
        }
        else {
            anim.SetBool("isRunning", false);
            head.sprite = heads[0];
        }

        if(!isGrounded) anim.SetBool("isJumping", true);
        else if(isGrounded) anim.SetBool("isJumping", false);
    }
}

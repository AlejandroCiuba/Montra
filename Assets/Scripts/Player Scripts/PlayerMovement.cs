using UnityEngine; //Mandatory
using System.Collections;
public class PlayerMovement : MonoBehaviour //All Unity Scripts inherit from a certain group of Objects, MonoBehaviour is the most common
{
    //Update is called once per frame
    void Update() 
    {
        canJump = IsGrounded();
        if(canJump && Input.GetKeyDown(KeyCode.Space)) Jump();
        if(jCount > 0 && Input.GetKey(KeyCode.Space) && isJumping) VarJump();
        if(Input.GetKeyUp(KeyCode.Space)) isJumping = false;
    }

    //FixedUpdate is called consistently regardless of FPS (every .02 seconds per gametime)
    //Used for things that must be consistent: Physics mostly
    //But the way movement is done here also benefits from it
    void FixedUpdate()
    {
        Move(); //Moves the player
        Fall();
    }

    void LateUpdate()
    {
        AnimationControl();
        canJump = IsGrounded();
    }

    //================== Movement Controls ==================
    //Player's RigidBody Component
    //[SerializeField] means we can see that variable in the editor and assign it values/objects there
    [SerializeField] private Rigidbody2D prb;
    //Player's Collider Component (Any 2D Shape)
    [SerializeField] private Collider2D pcol;
    [SerializeField] private float speed = 1000f;
    [SerializeField] private LayerMask jumpLayer; [SerializeField] private Vector3 increase;
    [SerializeField] private float jump; [SerializeField] private float fall;
    [SerializeField] private float jumpTime; private float jCount;
    private bool isJumping; private bool canJump;
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
        prb.velocity = Vector2.up * Mathf.Sqrt(jump);
        isJumping = true;
        jCount = jumpTime;
    }

    private void VarJump()
    {
        prb.velocity = Vector2.up * jump * .75f;
        jCount -= Time.deltaTime;
    }

    private bool IsGrounded() {return Physics2D.BoxCast(pcol.bounds.center, pcol.bounds.size + increase, 0f, Vector2.down, .3f, jumpLayer).collider != null;}

    //Once the player starts coming down from a jump, make them fall faster
    private void Fall() 
    {
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

        if(movement.x != 0 && IsGrounded()) {
            anim.SetBool("isRunning", true);
            head.sprite = heads[1];

            if(movement.x == -1) anim.SetBool("isLeft", true);
            else if(movement.x == 1)anim.SetBool("isLeft", false);
        }
        else {
            anim.SetBool("isRunning", false);
            head.sprite = heads[0];
        }

        if(!IsGrounded()) anim.SetBool("isJumping", true);
        else if(IsGrounded()) anim.SetBool("isJumping", false);
    }
}

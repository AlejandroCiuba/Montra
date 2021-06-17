using UnityEngine; //Mandatory

public class PlayerMovement : MonoBehaviour //All Unity Scripts inherit from a certain group of Objects, MonoBehaviour is the most common
{
    //Update is called once per frame
    void Update() 
    {
        AnimationControl();
        canJump = IsGrounded();
        if(canJump && Input.GetKeyDown(KeyCode.Space)) {Jump(); canJump = false;}//Activates jump
        if(jumpTimeCounter > 0 && Input.GetKey(KeyCode.Space) && isJumping) {VarJump(); canJump = false;} //Controls height based on how long the player holds space
        if(Input.GetKeyUp(KeyCode.Space)) isJumping = false; //Variable jump height modified from BlackThornProd Tutorial
    }

    //FixedUpdate is called consistently regardless of FPS (every .02 seconds per gametime)
    //Used for things that must be consistent: Physics mostly
    //But the way movement is done here also benefits from it
    void FixedUpdate()
    {
        Move(); //Moves the player
        Fall(); //Makes the player fall in accordance to the fallMultiplier
    }

    //================== Movement Controls ==================
    //Player's RigidBody Component
    //[SerializeField] means we can see that variable in the editor and assign it values/objects there
    [SerializeField] private Rigidbody2D prb;
    //Player's Collider Component (Any 2D Shape)
    [SerializeField] private Collider2D pcol;
    [SerializeField] private LayerMask jumpLayer;
    //How fast the player moves; usually set to something stupid high
    [SerializeField] private float speed = 1000f;
    //How high the player jumps, value depends on testing
    [SerializeField] private float jump = 1000f;
    [SerializeField] private float jumpTime = 100f;
    [SerializeField] private Vector3 jumpBounds;
    private float jumpTimeCounter;
    private bool isJumping;

    [SerializeField] private bool canJump = true;
    [SerializeField] private float jumpForgive = .2f;
    private float jumpForgiveCounter;
    //How much quicker the player starts falling once they start
    //coming down from a jump
    [SerializeField] private float fallMultiplier = 2.5f;
    //How close to the ground we need to be to jump again
    [SerializeField] private float checkGround = .1f;
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
        isJumping = true;
        jumpTimeCounter = jumpTime;
        prb.velocity = new Vector2(prb.velocity.x, 0f);
        prb.angularVelocity = 0f;
        prb.velocity = Vector2.up * jump;
    }

    private void VarJump()
    {
        prb.velocity = Vector2.up * jump * .75f;
        jumpTimeCounter -= Time.deltaTime;
    }

    private bool IsGrounded() 
    {   
        RaycastHit2D check = Physics2D.BoxCast(pcol.bounds.center, pcol.bounds.size + jumpBounds, 0f,
        Vector2.down, checkGround, jumpLayer);
        if(check.collider != null) {jumpForgiveCounter = jumpForgive; return true;}
        else if(check.collider == null && jumpForgiveCounter > 0) {jumpForgiveCounter -= Time.deltaTime; return true;}
        else return false;
    }

    //Once the player starts coming down from a jump, make them fall faster
    private void Fall() 
    {
        if(prb.velocity.y < 0f) 
            prb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //speed is used in our velocity calculation later
    public float speed = 5f;
    //jump speed is the seperate speed used only when the player initiates the jump action
    public float jumpSpeed = 5f;
    //variable recieved from the unity input manager. it will be positive when moving right and negative when moving left.
    private float direction = 0f;
    //we need Rigidbody2D to give our ations physics
    private Rigidbody2D player;
    private bool facingRight = true;

    /*vaariables used for determining when the player can jump */
    public Transform groundCheck;//this is an invisible object at the players feet
    public float groundCheckRadius;//radius for detecting ground
    public LayerMask groundLayer;//alows us to make objects labelled as ground
    public bool isTouchingGround;//boolean value to determine a condition

    //for fall/ death detection
    public GameObject fallDetector;
    // Start is called before the first frame update

    private Animator playerAnimation;

    public LifeAndDeath lifeAndDeath;

    //U.I. code is here
    void Start()
    {
        player = GetComponent<Rigidbody2D>(); //assigns the component to an object name
        playerAnimation = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //line below detects if groundcheck position and radius is touching groundLayer and therefore if player feet is touching ground
        isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        direction = Input.GetAxis("Horizontal");
        //this set of if's and else's give our player a velocity vector based on direction and speed
        if (direction > 0f)//Moving right
        {
            player.velocity = new Vector2(direction * speed, player.velocity.y);
        }
        else if (direction < 0f)//Moving left
        {
            player.velocity = new Vector2(direction * speed, player.velocity.y);
        }
        else
        {
            player.velocity = new Vector2(0, player.velocity.y);//this line gives us 0 velocity preventing sliding when idle
        }
        /*This code allows our character to jump*/
        if (Input.GetButtonDown("Jump") && isTouchingGround)
        {
            player.velocity = new Vector2(player.velocity.x, jumpSpeed);
        }
        //code to flip character face is here
        if (facingRight == false && direction > 0)
        {
            flip();
        } else if (facingRight == true && direction < 0)
        {
            flip();
        }
        void flip()
        {
            facingRight = !facingRight;
            Vector3 Scaler = transform.localScale;
            Scaler.x *= -1;
            transform.localScale = Scaler;
            //end of directional facing code
        }
        //fall detector code
        //This line makes the fall detector collider follow the player on the x-axis while keeping its y-axis position
        fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.position.y);


        playerAnimation.SetFloat("Speed", Mathf.Abs(player.velocity.x));//abs stands for absolute and removes negatives from our speed variable
        playerAnimation.SetBool("OnGround", isTouchingGround);

    }
    //on trigger enter means we run when the trigger collider is entered
    void OnTriggerEnter2D(Collider2D collision)
    {
        //here we use tags to determine if the player is entering the FallDetector object or not
        if(collision.tag == "FallDetector")
        {
            //this uses the scene manager library to load scene 0 which is the tutorial scene
            lifeAndDeath.TakeDamage(1000);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Config
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(25f,25f);

    //State
    bool isAlive = true;
    bool canJump = true;
    //Cached component referenced
    Rigidbody2D rb2D;
    Animator anim;
    CapsuleCollider2D bodyCollider2D;
    BoxCollider2D feetCollider2D;

    float gravityScaleAtStart;
    float runSpeedAtStart;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bodyCollider2D = GetComponent<CapsuleCollider2D>();
        feetCollider2D = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = rb2D.gravityScale;
        runSpeedAtStart = runSpeed;
    }

    void Update()
    {
        if(!isAlive) { return; }

        Run();
        Jump();
        ClimbLadder();
        Die();
        FlipSprite();

    }
    private void Run()
    {
        float controlThrow = Input.GetAxis("Horizontal"); //-1 to +1
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, rb2D.velocity.y);
        rb2D.velocity = playerVelocity;

        bool isPlayerMoving = Mathf.Abs(rb2D.velocity.x) > Mathf.Epsilon;
        anim.SetBool("Running", isPlayerMoving);

    }
    private void Jump()
    {
        if (!feetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"))) 
        {
            canJump = true;
            return; 
        }

        if (Input.GetButton("Jump") && canJump)
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            rb2D.velocity += jumpVelocityToAdd;
            canJump = false;
        }
    }
    private void ClimbLadder()
    {
        if (!bodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            anim.SetBool("Climbing", false);
            rb2D.gravityScale = gravityScaleAtStart;
            runSpeed = runSpeedAtStart;
            return;
        }
        runSpeed = runSpeedAtStart * 0.5f;
        float controlThrow = Input.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(rb2D.velocity.x, controlThrow * climbSpeed);
        rb2D.velocity = climbVelocity;
        rb2D.gravityScale = 0f;
        bool isPlayerClimbing = Mathf.Abs(rb2D.velocity.y) > Mathf.Epsilon;
        anim.SetBool("Climbing", isPlayerClimbing);
    }
    private void Die()
    {
        if (bodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            isAlive = false;
            anim.SetTrigger("Dying");
            rb2D.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    private void FlipSprite()
    {
        bool isPlayerMoving = Mathf.Abs(rb2D.velocity.x) > Mathf.Epsilon;
        if (isPlayerMoving)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb2D.velocity.x), 1f);
        }
    }
    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    //Config
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(25f, 25f);

    //State
    bool isAlive = true;

    //Cached component references
    Rigidbody2D rb;
    Animator anim;
    float gravityScaleAtStart;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gravityScaleAtStart = rb.gravityScale;
    }

    void Update()
    {
        if(isAlive) {
            Run();
            Jump();
            ClimbLadder();
            FlipSprite();
            Die();
        }
    }

    private void Run() {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        anim.SetBool("running", playerHasHorizontalSpeed);
    }

    private void Jump() {
        if (CrossPlatformInputManager.GetButtonDown("Jump") && IsTouchingGround()) {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            rb.velocity += jumpVelocityToAdd;
        }
    }

    private void ClimbLadder() {
        if(!transform.GetChild(0).GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Climbing"))) { //GetChild(0) refers to the player child gameobject named feet that has the box collider
            anim.SetBool("climbing", false);
            rb.gravityScale = gravityScaleAtStart;
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(rb.velocity.x, controlThrow * climbSpeed);
        rb.velocity = climbVelocity;
        rb.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
        anim.SetBool("climbing", playerHasVerticalSpeed);
    }

    private bool IsTouchingGround() {
        if (!transform.GetChild(0).GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Ground"))) { //Same thing as the comment above
            return false;
        }

        return true;
    }

    private void FlipSprite() {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        if(playerHasHorizontalSpeed) {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1);
        }
    } //Just changes the local scale x value to either -1 or 1 using Mathf Sign depending on velocity

    private void Die() {
        if (GetComponent<CapsuleCollider2D>().IsTouchingLayers(LayerMask.GetMask("Enemy","Hazards"))) {
            anim.SetTrigger("dying");
            GetComponent<Rigidbody2D>().velocity = deathKick;
            isAlive = false;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}

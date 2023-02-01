using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Samurai : MonoBehaviour {
    [Header("Movement")]
    public float runSpeed; // Horizontal grounded top speed
    public float groundAcc; // Horizontal grounded acceleration
    public float groundDec; // Horizontal grounded deceleration
    public float airAcc; // Horizontal airborne acceleration
    public float airDec; // Horizontal airborne deceleration
    public float jumpStrength; // Vertical initial jump velocity
    float flip, cachedFlip; // Float used for flip direction, 1 = right, -1 = left
    [Header("Effects")]
    public ParticleSystem runDust;
    public ParticleSystem jumpDust;
    public ParticleSystem landDust;
    public ParticleSystem slideDust;

    private bool moving;
    private bool grounded;
    private float acceleration, deceleration;
    private Vector2 moveDirection; // Movement unit vector
    private Vector2 move; // Movement vector
    private Vector2 vel_move; // Used for movement smooth damping

    // Input
    private bool pressing; // Input is being pressed
    private Vector2 inputVector; // Normalized vector read from input

    // Components
    private Animator anim; // Animator component on object
    private Rigidbody2D rb; // Rigidbody2D component on object

    private void Awake() {
        // Get Components
        anim = GetComponent<Animator>(); // Get Animator
        rb = GetComponent<Rigidbody2D>(); // Get Rigidbody2D

        // Initialize variable values
        flip = 1; // Initial direction is right
        cachedFlip = flip; // Cache initial right direction
        grounded = true;
    }

    private void Update() {
        acceleration = grounded ? groundAcc : airAcc;
        deceleration = grounded ? groundDec : airDec;

        inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // TEMP Input controlled from keyboard
        moving = inputVector != Vector2.zero;

        moveDirection.x = Mathf.Clamp(inputVector.x / 2, -1, 1) * 2;

        move.y = rb.velocity.y;
        move.x = moving ? Mathf.SmoothDamp(move.x, moveDirection.x * runSpeed, ref vel_move.x, acceleration) : Mathf.SmoothDamp(move.x, moveDirection.x * runSpeed, ref vel_move.x, deceleration);
        
        if (inputVector.x > 0) { flip = 1; } // Set flip right
        if (inputVector.x < 0) { flip = -1; } // Set flip left
        anim.SetBool("isRunning", inputVector.x != 0);

        transform.localScale = new Vector3(flip, 1, 1);
        if (cachedFlip != flip) {
            anim.SetTrigger("Flip");
            cachedFlip = flip;
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            anim.SetTrigger("Jump");
            move.y = jumpStrength;
        }
    }

    private void FixedUpdate() {
        rb.velocity = move;
    }

    private void InputHandler() {

    }

    private void OnTap() { }
    private void OnHold() { }
    private void OnSwipe() { }

    public void PlayRunDust() { runDust.Play(); }
    public void PlayJumpDust() { jumpDust.Play(); }
    public void PlayLandDust() { landDust.Play(); }
    public void PlaySlideDust() { slideDust.Play(); }
    public void StopAllDust() {
        jumpDust.Stop();
        slideDust.Stop();
    }
}
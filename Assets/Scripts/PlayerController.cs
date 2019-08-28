using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Movement
    private float moveInput = 0f;
    public float speed = 2f;
    public float airControl = 40f;

    //Jump
    public float jumpForce = 10f;
    private bool isGrounded = false;
    public Transform feetPos;
    public float feetCheckRadius = .3f;
    public LayerMask groundMask;
    private float jumpTimeLeft = 0f;
    public float jumpTime = 0.3f;
    private bool isJumping = false;

    //Shot
    public Transform gunHolderPivot;
    public float shotPower = 100f;
    public int shotLeft = 1;
    public int shotMax = 1;

    private Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {

        //Movement
        moveInput = Input.GetAxis("Horizontal");
        if (isGrounded) {
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        } // We can move a bit in the air, but we can accelerate only if velocity is less than ground max speed
        else if ((moveInput > 0 && rb.velocity.x < speed) || (moveInput < 0 && rb.velocity.x > -speed)) {
            rb.AddForce(Vector2.right * moveInput * airControl);
        }
    }

    void Update() {

        //Looking Direction
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if ((mouseWorldPosition.x > transform.position.x && transform.localScale.x < 0) || (mouseWorldPosition.x < transform.position.x && transform.localScale.x > 0)) {
            Flip();
        }
        Vector2 direction = (mouseWorldPosition - (Vector2)gunHolderPivot.position).normalized;
        gunHolderPivot.right = transform.localScale.x > 0 ? direction : -direction;

        //Shot
        if (!isGrounded && Input.GetButtonDown("Fire1") && shotLeft > 0) {
            rb.velocity = -direction * shotPower;
            shotLeft--;
        }

        //Jump
        isGrounded = Physics2D.OverlapCircle(feetPos.position, feetCheckRadius, groundMask);
        if (isGrounded && Input.GetButtonDown("Jump")) {
            isJumping = true;
            shotLeft = shotMax;
            jumpTimeLeft = jumpTime;
            rb.AddForce(Vector3.up * jumpForce * 15);
        }
        if (Input.GetButton("Jump") && isJumping && jumpTimeLeft > 0) {
            rb.AddForce(Vector3.up * jumpForce);
            jumpTimeLeft -= Time.deltaTime;
        }
        if (Input.GetButtonUp("Jump") || jumpTimeLeft <= 0) {
            isJumping = false;
        }
    }

    void Flip() {
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
}

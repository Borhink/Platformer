using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
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
    public Transform gunMuzzle;
    public Projectile projectilePrefab;
    public float shotPower = 100f;
    public int shotLeft = 1;
    public int shotMax = 1;
    private Vector2 shotVelocity;

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

        if (shotVelocity.sqrMagnitude > 0) {
            transform.Translate(shotVelocity.normalized);//To prevent player being stick to the ground on horizontal shots
            rb.velocity = shotVelocity;
            shotVelocity = Vector2.zero;
        }
    }

    void Update() {

        isGrounded = Physics2D.OverlapCircle(feetPos.position, feetCheckRadius, groundMask);
        if (isGrounded) {
            shotLeft = shotMax;
        }

        //Looking Direction
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if ((mouseWorldPosition.x > transform.position.x && transform.localScale.x < 0) || (mouseWorldPosition.x < transform.position.x && transform.localScale.x > 0)) {
            Flip();
        }

        ShotController(mouseWorldPosition);
        JumpController();
    }

    void Flip() {
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    void JumpController() {

        if (isGrounded && Input.GetButtonDown("Jump")) {
            isJumping = true;
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

    void ShotController(Vector2 mouseWorldPosition) {

        Vector2 direction = (mouseWorldPosition - (Vector2)gunHolderPivot.position).normalized;
        gunHolderPivot.right = transform.localScale.x > 0 ? direction : -direction;

        if (Input.GetButtonDown("Fire1") && shotLeft > 0) {
            shotVelocity = -direction * shotPower;
            shotLeft--;
            Projectile projectile = Instantiate(projectilePrefab, gunMuzzle.position, gunMuzzle.rotation) as Projectile;
            projectile.SetDirection(direction);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;

        if(collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Vector3 normal = collision.contacts[0].normal;

            if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y)) {
                shotLeft = shotMax;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private Player _player;


    void Start()
    {
		_player = GetComponent<Player>();
    }

    void FixedUpdate() {

        //Movement
        moveInput = Input.GetAxis("Horizontal");
        if (canMove) {
            if (isGrounded) {
                rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
            } // We can move a bit in the air, but we can accelerate only if velocity is less than ground max speed
            else if ((moveInput > 0 && rb.velocity.x < speed) || (moveInput < 0 && rb.velocity.x > -speed)) {
                rb.AddForce(Vector2.right * moveInput * airControl);
            }
        }

        if (shotVelocity.sqrMagnitude > 0) {
            rb.velocity = shotVelocity;
            StartCoroutine("ShortMoveDisable");
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

    IEnumerator ShortMoveDisable()
    {
        canMove = false;
        yield return new WaitForSeconds(0.15f);
        canMove = true;
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
            // rb.simulated
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

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

    void Update()
    {
        //Gun
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _player.Target(target);
		if (Input.GetButtonDown("Fire1"))
            _player.Fire();

        //Movement
        _player.Move(Input.GetAxis("Horizontal"));

        //Jump
        if (Input.GetButtonDown("Jump"))
            _player.StartJump();
        if (Input.GetButtonUp("Jump"))
            _player.StopJump();
    }

    void FixedUpdate() {

        // if (shotVelocity.sqrMagnitude > 0) {
        //     rb.velocity = shotVelocity;
        //     StartCoroutine("ShortMoveDisable");
        //     shotVelocity = Vector2.zero;
        // }
    }

    // void Update() {

        // ShotController(mouseWorldPosition);
    // }

    // IEnumerator ShortMoveDisable()
    // {
    //     canMove = false;
    //     yield return new WaitForSeconds(0.15f);
    //     canMove = true;
    // }

    // void ShotController(Vector2 mouseWorldPosition) {

    //     Vector2 direction = (mouseWorldPosition - (Vector2)gunHolderPivot.position).normalized;
    //     gunHolderPivot.right = transform.localScale.x > 0 ? direction : -direction;

    //     if (Input.GetButtonDown("Fire1") && shotLeft > 0) {
    //         // rb.simulated
    //         shotVelocity = -direction * shotPower;
    //         shotLeft--;
    //         Projectile projectile = Instantiate(projectilePrefab, gunMuzzle.position, gunMuzzle.rotation) as Projectile;
    //         projectile.SetDirection(direction);
    //     }
    // }
}

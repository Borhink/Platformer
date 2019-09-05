using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
	private Rigidbody2D _rb;

	//Movement
	[SerializeField] private float _speed = 2f;
	[SerializeField] private float _airControl = 40f;
	private float _moveInput = 0f;
	private bool _canMove = true;

    //Jump
	[SerializeField] private float _jumpForce = 10f;
	[SerializeField] private Transform _feetPos;
	[SerializeField] private float _feetCheckRadius = .3f;
	[SerializeField] private LayerMask _groundMask;
	[SerializeField] private float _jumpTime = 0.3f;
	private bool _isGrounded = false;
	private float _jumpTimeLeft = 0f;
	private bool _isJumping = false;

    //Gun
    [SerializeField] private Gun _gun;
	[SerializeField] private Transform _gunHolderPivot;

    void Start()
    {
		_rb = GetComponent<Rigidbody2D>();
    }

    public void Move()
    {

    }

	public void Jump()
    {

    }

    public void Shoot()
    {
        if (_gun)
        {

        }
    }
}

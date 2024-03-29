﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{

	private BoxCollider2D _boxCol;
	private Rigidbody2D _rb;
    private ContactSide2D _contactSide = new ContactSide2D();
	private bool _isGrounded = false;


	[Header("Wall")]
	[SerializeField] private float _wallBounciness = 5f;


	[Header("Gun")]
	[SerializeField] private Gun _gun = null;
	[SerializeField] private Transform _armPivot = null;
	[SerializeField] private Transform _gunHolder = null;
	private Vector2 _target;
	private bool _isFiring = false;
	private bool _isLoadingShot = false;
	private float _shotLoadTimer = 0f;
    delegate void FiringEvent();
    FiringEvent _firing;

    [Header("UI")]
    [SerializeField] RectTransform _powerBarFill;

	void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		_boxCol = GetComponent<BoxCollider2D>();

        _firing = Fire;
	}

    void Update()
    {
        if (_isLoadingShot)
        {
            _shotLoadTimer -= Time.deltaTime;

            float xScale = (_gun.ShotLoadTime() - _shotLoadTimer) / _gun.ShotLoadTime();
            _powerBarFill.localScale = new Vector3(xScale, 1, 1);
            if (_shotLoadTimer <= 0)
            {
                _shotLoadTimer = 0;
                _firing.Invoke();
            }
        }
    }

	void FixedUpdate()
	{
		_isGrounded = _contactSide.bottom;

		if (_isGrounded)
		{
			if (_gun && !_isFiring)
				_gun.Reload();
		}

		if (!_isGrounded)
		{
			int direction = _contactSide.OppositeWallDirection();
			if (direction != 0)
			{
                _rb.velocity = new Vector2(direction * _wallBounciness, 10f);
				_gun.Reload();
			}
		}
        _contactSide.Reset();
	}

	////////////////////////////////////////////////////////////////////////////////////////
	////////                                  Gun                                   ////////
	////////////////////////////////////////////////////////////////////////////////////////
	IEnumerator FiringTimer()
	{
		_isFiring = true;
		yield return new WaitForSeconds(0.25f);
		_isFiring = false;
	}

    public void LoadShot()
    {
        if (_gun && !_isLoadingShot && _gun.LoadShot(out _shotLoadTimer))
        {
            _isLoadingShot = true;
        }
    }

	public void Fire()
	{
		if (_gun && _isLoadingShot && !_isFiring)
		{
            _isLoadingShot = false;
            _powerBarFill.localScale = new Vector3(0, 1, 1);
			Vector2 direction = (_target - (Vector2)_armPivot.position).normalized;
			Vector2 shotVelocity = Vector2.zero;

			_gun.Fire(direction, _shotLoadTimer, ref shotVelocity);
            _rb.velocity = shotVelocity;
            StartCoroutine(FiringTimer());
		}
	}

	public void Target(Vector2 target)
	{
		_target = target;
		if ((_target.x > transform.position.x && transform.localScale.x < 0) || (_target.x < transform.position.x && transform.localScale.x > 0))
			Flip();

		Vector2 directionToTarget = (_target - (Vector2)_armPivot.position).normalized;
		_armPivot.right = transform.localScale.x > 0 ? directionToTarget : -directionToTarget;
	}

	void Flip()
	{
		Vector3 Scaler = transform.localScale;
		Scaler.x *= -1;
		transform.localScale = Scaler;
	}

	////////////////////////////////////////////////////////////////////////////////////////
	////////                               Collisions                               ////////
	////////////////////////////////////////////////////////////////////////////////////////


	void OnCollisionStay2D(Collision2D col)
	{
        _contactSide.Calculate(_boxCol, col.contacts);
	}
}

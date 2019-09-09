using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
	private Rigidbody2D rb;

	private Vector2 direction;
	public float speed = 5f;
	public float lifeTime = 10f;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = direction * speed;
		Destroy(gameObject, lifeTime);
	}

	public void SetDirection(Vector2 _direction) {
		direction = _direction;
	}
}

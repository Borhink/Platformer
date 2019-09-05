using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	[SerializeField] private Transform _gunMuzzle;
	[SerializeField] private Projectile _projectilePrefab;
	[SerializeField] private float _shotPower = 100f;
	[SerializeField] private int _shotLeft = 1;
	[SerializeField] private int _shotMax = 1;
	private Vector2 _shotVelocity;

    public void Shoot()
    {

    }
}

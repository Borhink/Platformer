using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	[SerializeField] private Transform _gunMuzzle = null;
	[SerializeField] private Projectile _projectilePrefab = null;
	[SerializeField] private float _shotPower = 100f;
	[SerializeField] private int _shotLeft = 1;
	[SerializeField] private int _shotMax = 1;

	public void Reload()
	{
		_shotLeft = _shotMax;
	}

	public bool Fire(Vector2 direction, ref Vector2 shotVelocity)
	{
		if (_shotLeft <= 0)
			return false;

		shotVelocity = -direction * _shotPower;
		_shotLeft--;
		Projectile projectile = Instantiate(_projectilePrefab, _gunMuzzle.position, _gunMuzzle.rotation) as Projectile;
		projectile.SetDirection(direction);

		return true;
	}
}

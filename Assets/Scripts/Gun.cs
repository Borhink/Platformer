﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	[SerializeField] private Transform _gunMuzzle = null;
	[SerializeField] private Projectile _projectilePrefab = null;
	[SerializeField] private float _shotPower = 100f;
	[SerializeField] private int _shotLeft = 1;
	[SerializeField] private int _shotMax = 1;
    [SerializeField] private float _shotLoadTime = 0.3f;

	public void Reload()
	{
		_shotLeft = _shotMax;
	}

	public bool LoadShot(out float shotLoadTime)
	{
        shotLoadTime = 0f;
		if (_shotLeft <= 0)
			return false;

		shotLoadTime = _shotLoadTime;
        return true;
	}

	public void Fire(Vector2 direction, float shotLoadTimer, ref Vector2 shotVelocity)
	{
        float ratio = (_shotLoadTime - shotLoadTimer) / (_shotLoadTime * 2) + 0.5f;
		shotVelocity = -direction * _shotPower * ratio;
		_shotLeft--;
		Projectile projectile = Instantiate(_projectilePrefab, _gunMuzzle.position, _gunMuzzle.rotation) as Projectile;
		projectile.SetDirection(direction);
	}

    public float ShotLoadTime()
    {
        return _shotLoadTime;
    }
}

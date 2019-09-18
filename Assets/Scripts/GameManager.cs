using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;

	float _baseDeltaTime;

	void Awake()
	{
		instance = this;
		_baseDeltaTime = Time.fixedDeltaTime;
	}

	public void SlowTime()
	{
		Time.timeScale = 0.25f;
		Time.fixedDeltaTime = Time.timeScale * _baseDeltaTime;
	}

	public void RestoreTime()
	{
		Time.timeScale = 1f;
		Time.fixedDeltaTime = Time.timeScale * _baseDeltaTime;
	}
}

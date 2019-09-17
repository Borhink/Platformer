using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;

	void Awake()
	{
		instance = this;
	}

	public void SlowTime()
	{
		Time.timeScale = 0.25f;
		Debug.Log(Time.fixedDeltaTime);
		Time.fixedDeltaTime /= 4;
	}

	public void RestoreTime()
	{
		Time.timeScale = 1f;
		Time.fixedDeltaTime *= 4;
	}
}

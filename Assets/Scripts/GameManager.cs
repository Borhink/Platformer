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
		Time.timeScale = 0.4f;
	}

	public void RestoreTime()
	{
		Time.timeScale = 1f;
	}
}

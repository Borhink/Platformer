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

		if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			GameManager.instance.SlowTime();
		}
		else if (Input.GetKeyUp(KeyCode.Mouse1))
		{
			GameManager.instance.RestoreTime();
		}
	}
}

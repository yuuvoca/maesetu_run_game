using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{

	SaveManager saveManager;

	void Start()
	{
		saveManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SaveManager>();
	}

	public void OnClick()
	{
		saveManager.Save();
	}

}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InExplanationFirstViewController : ViewController {

	void Start()
	{
		StartCoroutine (StartCount());
	}

	IEnumerator StartCount()
	{
		yield return new WaitForSeconds (1.0f);

		OnNextButtonClick ();
	}

	public void OnNextButtonClick()
	{
		//go to next view
		ViewManagerFinder.Find().Push ( ViewManagerFinder.Find().InExplanationSecondView);
	}

	public void OnBackButtonClick()
	{
		//go to next view
		ViewManagerFinder.Find().Push ( ViewManagerFinder.Find().InStartView);
	}

}

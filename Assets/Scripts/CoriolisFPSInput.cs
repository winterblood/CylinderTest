﻿using UnityEngine;
using System.Collections;

public class CoriolisFPSInput : MonoBehaviour
{
	public bool showDebug = false;
	private CoriolisCharMover motor;

	void Awake ()
	{
		motor = GetComponent("CoriolisCharMover") as CoriolisCharMover;
	}

	void Start ()
	{
	
	}
	
	void Update ()
	{
		// Get the input vector from kayboard or analog stick
		Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		
		if (directionVector != Vector3.zero)
		{
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1, directionLength);
			
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
			
			// Multiply the normalized direction vector by the modified length
			directionVector = directionVector * directionLength;
		}
		
		// Apply the direction to the CharacterMotor
		motor.SetInputDir( ref directionVector, Input.GetButton("Jump") );
	}
	
	void OnGUI()
	{
		if (showDebug)
		{
			int y = 10;
			GUI.Box(new Rect(10, y, 250, 50), "Input horizontal = " + Input.GetAxis("Horizontal")); y+=60;
			GUI.Box(new Rect(10, y, 250, 50), "Input vertical = " + Input.GetAxis("Vertical")); y+=60;
		}
	}
}

// Require a character controller to be attached to the same game object
//@script RequireComponent (CharacterMotor)
//@script AddComponentMenu ("Character/Coriolis FPS Input Controller");
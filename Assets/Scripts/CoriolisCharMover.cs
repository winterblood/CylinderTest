using UnityEngine;
using System.Collections;

public class CoriolisCharMover : MonoBehaviour
{
	public bool showDebug = false;
	public float moveForceMultiplier = 10.0f;

	// The current global direction we want the character to move in.
	private Vector3 inputMoveDirection = Vector3.zero;
	
	// Is the jump button held down? We use this interface instead of checking
	// for the jump button directly so this script can also be used by AIs.
	private bool inputJump = false;

	private Rigidbody rigid;

	// Use this for initialization
	void Start () {
		rigid = gameObject.GetComponent("Rigidbody") as Rigidbody;
	}
	
	void FixedUpdate()
	{
		Vector3 worldMoveDir = gameObject.transform.localToWorldMatrix * inputMoveDirection;
		rigid.AddForce( worldMoveDir * moveForceMultiplier );		
	}
	
	// Update is called once per frame
	void Update ()
	{
		Transform m = gameObject.transform;
		Vector3 towardsCoriolisAxis = -m.position;
		towardsCoriolisAxis.z = 0.0f;
		towardsCoriolisAxis.Normalize();
		
		Quaternion rotation = Quaternion.LookRotation(m.forward, towardsCoriolisAxis);
		m.rotation = rotation;
	}
	
	void OnGUI()
	{
		if (showDebug)
		{
			GUI.Box(new Rect(10, 10, 250, 50), "Input Move = " + inputMoveDirection);
		}
	}
	
	public void SetInputDir( ref Vector3 dir, bool jump )
	{
		inputMoveDirection = dir;
		inputJump = jump;
	}
}

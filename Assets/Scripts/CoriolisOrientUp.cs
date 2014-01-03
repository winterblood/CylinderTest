using UnityEditor;
using UnityEngine;
using System.Collections;

public class CoriolisOrientUp : MonoBehaviour
{
	void OnEnable()
	{
		EditorApplication.update += Update;
	}

	// Use this for initialization
	void Start ()
	{
		
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
	
/*	
		if (Selection.Contains(gameObject))
		{
			Transform m = gameObject.transform;
			Vector3 oldForward = new Vector3();
			oldForward = m.forward;
			Vector3 realUp = new Vector3();
			realUp = -m.position;
			realUp.z = 0.0f;
			realUp.Normalize();
			
			m.up = realUp;
			m.right = Vector3.Cross( oldForward, realUp );
			m.right.Normalize();
			m.forward = Vector3.Cross( realUp, m.right );
			m.forward.Normalize();			
		}
*/		
	}

}

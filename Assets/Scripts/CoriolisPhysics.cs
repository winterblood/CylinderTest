using UnityEditor;
using UnityEngine;
using System.Collections;

public class CoriolisPhysics : MonoBehaviour
{
	static float coriolisRotationSpeed = Mathf.PI / 20.0f;	// radians/sec
	static Vector3 frameAngularVelocity;
	
	public bool showDebug = false;	
	public bool doPhysics = true;
	public bool doOrientUp = false;
	
	private Rigidbody rigid;
	private Vector3 totalForce = Vector3.zero;
	private Transform m = null;
	
	void OnEnable()
	{
		EditorApplication.update += Update;
		// AngularVel = (radiusVec cross linearVel) / radiusVec.MagSqr
		//Vector3 angularVel = Vector3.Cross( radiusFromAxis, rigid.velocity ) / radiusFromAxis.sqrMagnitude;
		frameAngularVelocity = Vector3.forward * coriolisRotationSpeed;
	}

	void Start ()
	{
		m = gameObject.transform;
		if (doPhysics)
		{
			rigid = gameObject.GetComponent("Rigidbody") as Rigidbody;
		}
	}
	
	void FixedUpdate()
	{
		if (doPhysics && rigid && !rigid.IsSleeping())
		{
			// Simulate coriolis effect by applying force spinwards
			Transform m = gameObject.transform;
			Vector3 radiusFromAxis = m.position;
			radiusFromAxis.z = 0.0f;
			//Vector3 spinwards = Vector3.Cross( radiusFromAxis, Vector3.forward );
			//spinwards.Normalize();

			// DEBUG ONLY - Update every frame while debugging, so speed can be adjusted - CP
			frameAngularVelocity = Vector3.forward * coriolisRotationSpeed;

			// Calc coriolis force = (-2*mass*angularVelocityAroundAxis) X linearVelocity
			Vector3 coriolisForce = Vector3.Cross ( frameAngularVelocity*rigid.mass*-2.0f, rigid.velocity );
			
			// Calc centrifugal force = mass*angularVelocityAroundAxis X ( angularVelocityAroundAxis X radiusVec )
			Vector3 centrifugalForce = Vector3.Cross( rigid.mass*frameAngularVelocity, Vector3.Cross( frameAngularVelocity, -radiusFromAxis ) );

			// Apply forces
			totalForce = coriolisForce + centrifugalForce;
			rigid.AddForce( totalForce );
		}
	}
	
	// Update is called once per frame
	void Update ()
	{	
		if (doOrientUp && (!rigid || !rigid.IsSleeping()))
		{
			Vector3 localUp;
			/*
			if (doPhysics)
			{
				localUp = -totalForce;	// "Up" should be in opposition to local "gravity"
			}
			else
			*/
			{
				localUp = -m.position;	// If not simulating physics, apporximate "up" as "towards axis"
			}
			localUp.z = 0.0f;
			localUp.Normalize();
						
			Quaternion rotation = Quaternion.LookRotation(m.forward, localUp);
			m.rotation = rotation;
		}
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
	
	void OnGUI()
	{
		if (showDebug)
		{
			GUI.Box(new Rect(10, 10, 250, 50), "Total Force = " + totalForce.magnitude);
		}
	}

}

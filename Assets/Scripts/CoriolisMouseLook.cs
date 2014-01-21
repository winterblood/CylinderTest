
/// CoriolisMouseLook rotates the transform of player and camera based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Coriolis Mouse Look")]
public class CoriolisMouseLook : MonoBehaviour
{	
	public Camera viewpointCamera;

	public float sensitivityYaw = 15F;
	public float sensitivityPitch = 15F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minPitch = -80F;
	public float maxPitch = 80F;
	
	private float pitch = 0F;
	
	void Update ()
	{
		// transform refers to the player themselves
		transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityYaw, 0);
		
		// viewpointCamera.transform refers to the camera
		pitch += Input.GetAxis("Mouse Y") * sensitivityPitch;
		pitch = Mathf.Clamp (pitch, minPitch, maxPitch);
		
		Vector3 euler = new Vector3(-pitch, viewpointCamera.transform.localEulerAngles.y, 0);
		viewpointCamera.transform.localEulerAngles = euler;
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
}
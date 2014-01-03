
/// CoriolisMouseLook rotates the transform of player and camera based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Coriolis Mouse Look")]
public class CoriolisMouseLook : MonoBehaviour {
	
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public Camera viewpointCamera;

	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	
	float rotationY = 0F;
	
	void Update ()
	{
		// transform refers to the player themselves
		transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		
		// viewpointCamera.transform refers to the camera
		rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
		rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
		Vector3 euler = new Vector3(-rotationY, viewpointCamera.transform.localEulerAngles.y, 0);
		viewpointCamera.transform.localEulerAngles = euler;
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
}
using UnityEngine;
using System.Collections;

public class SunLine : MonoBehaviour {
	
	public float speed = 100.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		Vector3 pos = transform.position;
		pos.z += speed * Time.deltaTime;
		
		if (pos.z > 1700.0f)
			pos.z = -1700.0f;

		transform.position = pos;
	
	}
}

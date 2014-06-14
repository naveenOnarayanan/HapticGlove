using UnityEngine;
using System.Collections;

public class Magic : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//instantiate fire object
	}
	
	// Update is called once per frame
	void Update () {
		//if press key, create fire object in front of view
		if (Input.GetKeyDown ("f")) {
			GameObject fire = (GameObject)Instantiate(Resources.Load ("Flame"));
			Vector3 vector = new Vector3(transform.position.x, 
			                             0, 
			                             transform.position.z);
			fire.transform.position += vector;
		}
	}
}

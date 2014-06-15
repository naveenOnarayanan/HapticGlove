using UnityEngine;
using System.Collections;

public class Magic : MonoBehaviour {
	enum MagicType {
		Fire,
		Ice
	};

	void CreateObject(MagicType type) {
		GameObject obj;

		switch (type) {
			case MagicType.Fire:
				obj = (GameObject)Instantiate(Resources.Load ("Flame"));
				break;
			default:
				obj = (GameObject)Instantiate(Resources.Load ("Stream"));
				break;
		}

		Vector3 vector = Camera.main.ViewportToWorldPoint (new Vector3(0.5f, 0, 2f));
		obj.transform.position += vector;
	}
	
	// Use this for initialization
	void Start () {
		//instantiate fire object
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("f")) {
			CreateObject (MagicType.Fire);
			//TODO: send signal for heat to server
		}

		if (Input.GetKeyDown ("g")) {
			CreateObject (MagicType.Ice);
			//TODO: send signal for cold to server
		}
	}
}

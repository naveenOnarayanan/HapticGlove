using UnityEngine;
using System.Collections;

public class Magic : MonoBehaviour {
	public WWW server;

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
	IEnumerator Start () {
		//instantiate server
		server = new WWW ("http://ip.jsontest.com/");
		yield return server;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("f")) {
			CreateObject (MagicType.Fire);
			//TODO: send signal for heat to server
			//print (server.text);
		}

		if (Input.GetKeyDown ("g")) {
			CreateObject (MagicType.Ice);
			//TODO: send signal for cold to server
		}
	}
}

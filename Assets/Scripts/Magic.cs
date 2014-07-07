using UnityEngine;
using System.Collections;
using Leap;

public class Magic : MonoBehaviour {
	public WWW server;
	Controller controller;

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
		controller = new Controller ();
		controller.EnableGesture (Gesture.GestureType.TYPE_CIRCLE);
		controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);

		//instantiate server
		server = new WWW ("http://ip.jsontest.com/");
		yield return server;
	}
	
	// Update is called once per frame
	void Update () {
		Frame frame = controller.Frame ();

		GestureList gestures = frame.Gestures ();
		
		foreach (Gesture gesture in frame.Gestures()) {
			switch(gesture.Type) {
				case Gesture.GestureType.TYPE_CIRCLE:
					Debug.Log("circle");
					CreateObject(MagicType.Fire);
					break;
				case Gesture.GestureType.TYPE_SWIPE:
					Debug.Log("swipe");
					CreateObject(MagicType.Ice);
					break;
				default:
					Debug.Log("bad gesture");
					break;
			}
		}

		if (Input.GetKeyDown ("f")) {
			CreateObject (MagicType.Fire);
			//TODO: send signal for heat to server
			//Debug.Log (server.text);
		}

		if (Input.GetKeyDown ("g")) {
			CreateObject (MagicType.Ice);
			//TODO: send signal for cold to server
		}
	}
}

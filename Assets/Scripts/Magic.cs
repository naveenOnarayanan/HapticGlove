using UnityEngine;
using System.Collections;
using Leap;

public class Magic : MonoBehaviour {
	public WWW server;
	Controller controller;
	Hand mainHand;

	int counter = 0;

	GameObject obj;
	MagicType lastCall;
  bool fireballCharged = false;


	enum MagicType {
		FireSmall,
		FireMedium,
		FireLarge,
		Fireball,
		Ice
	};

	void CreateObject(MagicType type) {
		Vector3 vector = transform.TransformPoint (mainHand.PalmPosition.ToUnityScaled ());
		if (obj != null && type == lastCall) {
			obj.transform.position = transform.TransformPoint (mainHand.PalmPosition.ToUnityScaled ());
			obj.transform.rotation = transform.rotation;
		} else {
        if (lastCall == MagicType.Fireball) {
          Destroy (obj, 5);
        } else {
          Destroy(obj);
        }
				switch (type) {
					case MagicType.FireSmall:
            obj = (GameObject)Instantiate (Resources.Load ("Fire_01"), vector, transform.rotation);
						break;
					case MagicType.FireMedium:
            obj = (GameObject)Instantiate (Resources.Load ("Fire_02"), vector, transform.rotation);
						break;
          case MagicType.FireLarge:
            obj = (GameObject) Instantiate (Resources.Load ("Fire_03"), vector, transform.rotation);
            break;
					case MagicType.Fireball:
            // TODO: Need to get the fireball to be shot where the user is facing
            Debug.Log (Camera.main.transform.rotation);
            obj = (GameObject)Instantiate(Resources.Load ("Fireball"), vector, Camera.main.transform.rotation);	
						break;
					default:
						obj = (GameObject)Instantiate (Resources.Load ("Stream"));
						break;
				}
		}

		lastCall = type;
	}
	
  IEnumerator Delay(int seconds) {
    yield return new WaitForSeconds (seconds);
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


		mainHand = controller.Frame().Hands[0];

		//Debug.Log (mainHand.Direction + ":" + mainHand.PalmNormal);

		Pointable finger;
		bool fingerFlat = true;

		//Debug.Log (transform.TransformPoint(mainHand.PalmPosition.ToUnityScaled()));

		for (int i = 0; i < controller.Frame().Pointables.Count; i++) {
			finger = controller.Frame ().Pointables[i];
			fingerFlat &= finger.Direction.y > 0;
		}
    
    
    Gesture.GestureType gesture = Gesture.GestureType.TYPE_INVALID;
    foreach (Gesture gestures in frame.Gestures ()) {
      gesture = gestures.Type;
      break;
    }
    
    // Metrics to detect if hand is face-up with fingers open (indicates if hand can charge fireball)
		if (mainHand.Direction.y > 0.5 && mainHand.PalmNormal.y < 0 && mainHand.PalmNormal.z < 0 && fingerFlat) {
      Debug.Log (counter);
      // Counts for 200 frames before switching to larger flame
      if (counter > 200 && counter <= 500) {
        CreateObject(MagicType.FireMedium);
      } else if (counter > 500) {
        CreateObject(MagicType.FireLarge);
      } else {
        CreateObject (MagicType.FireSmall);
      }
      fireballCharged = true;
      counter++;
    } else if (gesture == Gesture.GestureType.TYPE_CIRCLE && fireballCharged) {
      // TODO: need to add logic such that if the fireball was not shot within X amount of time then
      // fireball will not be released
      CreateObject (MagicType.Fireball);
      fireballCharged = false;
    } else {
      if (lastCall == MagicType.Fireball) {
        Debug.Log ("Destroying since it is not fireball");
        Delay (10);
      }
      if (obj != null && lastCall != MagicType.Fireball) {
        Destroy (obj.gameObject); 
      }
      counter = 0;
    }
	}
}

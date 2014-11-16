using Leap;
using SimpleJSON;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

public class Magic : MonoBehaviour {
    NetworkController nc;
    string PELTIER = "peltier";
    string SERVO = "servo";

    Controller controller;
  	Hand mainHand;

  	int chargeCounter = 0;
    int cooldownCounter = 0;

    int SHORT_THRESHOLD = 100;
    int MEDIUM_THRESHOLD = 200;
    int LONG_THRESHOLD = 300;
    int COOLDOWN_THRESHOLD = 500;

  	GameObject obj;
  	MagicType lastCall;

    bool fireballCharged = false;
    bool canCharge = false;

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
                    obj = (GameObject)Instantiate(Resources.Load ("Fireball"), vector, Camera.main.transform.rotation);	
                    //obj.AddComponent<Rigidbody>();
                    //obj.rigidbody.useGravity = false;
                    obj.AddComponent<BoxCollider>();
        						break;
      					default:
        						obj = (GameObject)Instantiate (Resources.Load ("Stream"));
        						break;
    				}
    		}

  		  lastCall = type;
  	}

    void neutralize() {
        fireballCharged = false;
        chargeCounter = 0;
        nc.sendData("\"temperature\": 0", PELTIER);
    }
    
  	// Use this for initialization
  	void Start () {
    		controller = new Controller ();
    		controller.EnableGesture (Gesture.GestureType.TYPE_CIRCLE);
    		controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);

        nc = new NetworkController();
  	}
  	
  	// Update is called once per frame
  	void Update () {
    		mainHand = controller.Frame().Hands[0];

    		//Debug.Log (mainHand.Direction + ":" + mainHand.PalmNormal);

        Gesture.GestureType gesture = Gesture.GestureType.TYPE_INVALID;

        foreach (Gesture gestures in controller.Frame().Gestures ()) {
            gesture = gestures.Type;
            break;
        }

        //cooldown mode, don't let it do anything else
        if (cooldownCounter > 0 && cooldownCounter < COOLDOWN_THRESHOLD) {
            //pull fingers taught
            nc.sendData ("\"temperature\": 0", PELTIER);
            nc.sendData ("\"angle\": 180", SERVO);

            cooldownCounter++;
        } else if (cooldownCounter == COOLDOWN_THRESHOLD) {
            //release fingers
            nc.sendData ("\"angle\": 0", SERVO);

            Debug.Log ("Done cooldown");
            cooldownCounter = 0;
        //charging
        } else if (MagicHelper.handFaceUp(mainHand, controller.Frame ())) {
            if (chargeCounter <= LONG_THRESHOLD) {
                if (chargeCounter > SHORT_THRESHOLD && chargeCounter <= MEDIUM_THRESHOLD) {
                    CreateObject(MagicType.FireMedium);
                } else if (chargeCounter > MEDIUM_THRESHOLD) {
                    CreateObject(MagicType.FireLarge);
                } else {
                    CreateObject (MagicType.FireSmall);
                }

                fireballCharged = true;
                //TODO: differentiate hotness when we have that established
                nc.sendData("\"temperature\": 10", PELTIER);
                chargeCounter++;
            //overloaded, go into cooldown
            } else {
                Debug.Log ("Charged too long, in cooldown mode");
                Destroy (obj.gameObject); 

                neutralize();
                cooldownCounter++;
            }
        //shoot fireball
        //TODO: do when hand's facing forward
        } else if (gesture == Gesture.GestureType.TYPE_CIRCLE && fireballCharged) {
            //TODO: change fireball based on chargedness
            CreateObject (MagicType.Fireball);

            neutralize();

        //not doing a gesture, cancel charges
        } else {
            if (obj != null && lastCall != MagicType.Fireball) {
                Destroy (obj.gameObject); 
            }

            neutralize();
        }
  	}
}

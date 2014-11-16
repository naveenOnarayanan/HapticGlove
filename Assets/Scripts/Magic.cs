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

    int SHORT_THRESHOLD = 200;
    int MEDIUM_THRESHOLD = 400;
    int LONG_THRESHOLD = 600;
    int COOLDOWN_THRESHOLD = 500;

  	GameObject obj;
  	MagicType lastCall;

    bool canCharge = false;
    MagicType canCall;

  	enum MagicType {
  		FireCharge,
  		Fireball,
  		IceCharge
  	};

    enum Size {
        Small,
        Medium,
        Large
    }

  	void CreateObject(MagicType type, Size size = Size.Small) {
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
                case MagicType.FireCharge:
                    switch (size) {
                      case Size.Small:
                          obj = (GameObject)Instantiate (Resources.Load ("Fire_01"), vector, transform.rotation);
                          break;
                      case Size.Medium:
                          obj = (GameObject)Instantiate (Resources.Load ("Fire_02"), vector, transform.rotation);
                          break;
                      case Size.Large:
                          obj = (GameObject) Instantiate (Resources.Load ("Fire_03"), vector, transform.rotation);
                          break;
                    }
                    break;
                //TODO: differentiate difference ices
                case MagicType.IceCharge:
                    switch (size) {
                    case Size.Small:
                        obj = (GameObject)Instantiate (Resources.Load ("Stream"));
                        break;
                    case Size.Medium:
                        obj = (GameObject)Instantiate (Resources.Load ("Stream"));
                        break;
                    case Size.Large:
                        obj = (GameObject)Instantiate (Resources.Load ("Stream"));
                        break;
                    }
                    break;
                case MagicType.Fireball:
                    // TODO: Need to get the fireball to be shot where the user is facing
                    obj = (GameObject)Instantiate(Resources.Load ("Fireball"), vector, Camera.main.transform.rotation); 
                    //obj.AddComponent<Rigidbody>();
                    //obj.rigidbody.useGravity = false;
                    obj.AddComponent<BoxCollider>();
                    break;
                default:
                    break;
        				}
    		}

  		  lastCall = type;
  	}

    void neutralize() {
        chargeCounter = 0;
        canCharge = false;
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

        //Debug.Log (MagicHelper.handFaceForward(mainHand, controller.Frame()) + " " + mainHand.Direction + ":" + mainHand.PalmNormal);

        //TODO: gestures within last X frames
        foreach (Gesture gesture in controller.Frame().Gestures ()) {
            if (gesture.Type == Gesture.GestureType.TYPE_CIRCLE) {
                canCharge = true;
                canCall = MagicType.FireCharge;
            } else if (gesture.Type == Gesture.GestureType.TYPE_SWIPE) {
                canCharge = true;
                canCall = MagicType.IceCharge;
            }
        }

        //cooldown mode, don't let it do anything else
        if (cooldownCounter > 0 && cooldownCounter < COOLDOWN_THRESHOLD) {
            //pull fingers taught
            nc.sendData ("\"angle\": 180", SERVO);

            cooldownCounter++;

            neutralize();
        } else if (cooldownCounter == COOLDOWN_THRESHOLD) {
            //release fingers
            nc.sendData ("\"angle\": 0", SERVO);

            Debug.Log ("Done cooldown");
            cooldownCounter = 0;

            neutralize();
        //charging
        } else if (MagicHelper.handFaceUp(mainHand, controller.Frame()) && canCharge) {
            if (chargeCounter <= LONG_THRESHOLD) {
                Debug.Log (chargeCounter);
                if (chargeCounter > SHORT_THRESHOLD && chargeCounter <= MEDIUM_THRESHOLD) {
                    CreateObject(canCall, Size.Medium);
                } else if (chargeCounter > MEDIUM_THRESHOLD) {
                    CreateObject(canCall, Size.Large);
                } else {
                    CreateObject(canCall, Size.Small);
                }

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
        } else if (MagicHelper.handFaceForward(mainHand, controller.Frame()) && chargeCounter > 0) {
            if (canCall == MagicType.FireCharge) {
                //TODO: change fireball based on chargedness
                CreateObject (MagicType.Fireball);
            } else {
                //TODO: change this to ice
                CreateObject (MagicType.Fireball);
            }

            neutralize();

        //not doing a gesture, cancel charges
        } else {
            if (obj != null && lastCall != MagicType.Fireball) {
                Destroy (obj.gameObject); 
            }
        }
  	}
}

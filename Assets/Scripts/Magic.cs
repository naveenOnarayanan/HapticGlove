using Leap;
using SimpleJSON;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

public class Magic : MonoBehaviour {
    NetworkController nc;
    CooldownHelper cd;

    Controller controller;
  	Hand mainHand;

  	int chargeCounter = 0;
    int idleCounter = 0;

    int MIN_THRESHOLD = 20;
    int SHORT_THRESHOLD = 200;
    int MEDIUM_THRESHOLD = 400;
    int LONG_THRESHOLD = 600;

    int IDLE_THRESHOLD = 50;

    GameObject lastCreatedObj;
  	MagicType lastCall;

    bool canCharge = false;

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
    		if (lastCreatedObj != null && type == lastCall) {
            lastCreatedObj.transform.position = transform.TransformPoint (mainHand.PalmPosition.ToUnityScaled ());
            lastCreatedObj.transform.rotation = transform.rotation;
    		} else {
            if (lastCall == MagicType.Fireball) {
                Destroy (lastCreatedObj, 5);
            } else {
                Destroy(lastCreatedObj);
            }
            string magicType = null;
    				switch (type) {
                case MagicType.FireCharge:
                    switch (size) {
                      case Size.Small:
                          magicType = MagicConstant.FIREBALL_SMALL_NAME;                       
                          break;
                      case Size.Medium:
                          magicType = MagicConstant.FIREBALL_MEDIUM_NAME;
                          break;
                      case Size.Large:
                          magicType = MagicConstant.FIREBALL_LARGE_NAME;
                          break;
                    }
                    
                    break;
                case MagicType.IceCharge:
                    magicType = MagicConstant.ICEWALL_CAST;
                    break;
                case MagicType.Fireball:
                    // TODO: Need to get the fireball to be shot where the user is facing
                    magicType = MagicConstant.FIREBALL_RELEASE_NAME;
                    break;
                default:
                    break;
        				}
            if (magicType != null) {
                lastCreatedObj = (GameObject)Instantiate(Resources.Load (magicType), vector, Camera.main.transform.rotation); 
            }
    		} 
  		  lastCall = type;
  	}

    void neutralize() {
        if (lastCreatedObj != null && lastCall != MagicType.Fireball) {
            Destroy (lastCreatedObj.gameObject); 
            lastCreatedObj = null;
        }

        chargeCounter = 0;
        idleCounter = 0;
        canCharge = false;

        nc.resetPeltier();
    }
    
  	// Use this for initialization
  	void Start () {
    		controller = new Controller ();
        nc = new NetworkController();
        cd = new CooldownHelper ();
  	}
  	
  	// Update is called once per frame
  	void Update () {
        mainHand = controller.Frame ().Hands [0];

        //Debug.Log (HandHelper.isFaceForward(mainHand, controller.Frame()) + " " + mainHand.Direction + ":" + mainHand.PalmNormal);

        /*
        //gestures within last 10 frames
        foreach (Gesture gesture in controller.Frame().Gestures(controller.Frame(10))) {
            if (gesture.Type == Gesture.GestureType.TYPE_CIRCLE) {
                canCharge = true;
            }
        }
        */
        if (mainHand != null) {
            Debug.Log (HandHelper.isClosedFist(mainHand));

        }
        //cooling down, don't do anything else
        if (cd.coolingDown(nc)) {
            neutralize ();
            //TODO: have this work based on data from servo motor
        } /*else if (HandHelper.isClosedFist (mainHand)) {
            canCharge = true;
            Debug.Log ("closed fist");
            //TODO: have this work based on data from servo motor
            //charge fire
        } */
        else if (HandHelper.isFaceUp (mainHand, controller.Frame ()) && canCharge) {
            //TODO: resize fireball instead of creating different ones?
            if (chargeCounter <= LONG_THRESHOLD) {
                if (chargeCounter > SHORT_THRESHOLD && chargeCounter <= MEDIUM_THRESHOLD) {
                    CreateObject (MagicType.FireCharge, Size.Medium);
                } else if (chargeCounter > MEDIUM_THRESHOLD) {
                    CreateObject (MagicType.FireCharge, Size.Large);
                } else {
                    CreateObject (MagicType.FireCharge, Size.Small);
                }
                
                //TODO: differentiate hotness when we have that established
                nc.heatPeltier ();
                chargeCounter++;
                //overloaded, go into cooldown
            } else {
                Debug.Log ("Charged too long, in cooldown mode");
                Destroy (lastCreatedObj.gameObject); 
                
                neutralize ();
                cd.startCooldown();
            }
            //shoot charged object
        } else if (HandHelper.isFaceForward (mainHand, controller.Frame ()) && chargeCounter > MIN_THRESHOLD) {
            //TODO: change fireball based on chargedness
            CreateObject (MagicType.Fireball);

            neutralize ();
            //TODO: charge ice wall
        } else if (HandHelper.isFaceForward(mainHand, controller.Frame ())) {
            //check for collision with an ice object. if there isn't any, create one

            //if there is, increase its strength
        //not doing anything
        } else {
            //buffer period to confirm doing nothing
            if (idleCounter > IDLE_THRESHOLD) {
                neutralize ();
            }
            idleCounter++;
        }
    }
}

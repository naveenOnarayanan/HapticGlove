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

    int MIN_THRESHOLD = 5;
    int SHORT_THRESHOLD = 200;
    int MEDIUM_THRESHOLD = 400;
    int LONG_THRESHOLD = 600;

    int IDLE_THRESHOLD = 50;

    GameObject objInCurrFrame;
    GameObject objInLastFrame;
  	MagicType lastCall;
    Size lastSize = Size.Small;

    bool canCharge = false;

  	enum MagicType {
  		FireCharge,
  		Fireball,
      IceWall
  	};

    enum Size {
        Small,
        Medium,
        Large
    }

    GameObject CreateObject(MagicType type) {
        return CreateObject (type, lastSize);
    }

  	GameObject CreateObject(MagicType type, Size size) {
    		Vector3 vector = mainHand.PalmPosition.ToUnityScaled ();
        Quaternion rotation = Camera.main.transform.rotation;
            
        //if same obj in last frame, then just move it
        if (objInLastFrame != null && type == lastCall && size == lastSize) {
            objInLastFrame.transform.position = transform.TransformPoint (mainHand.PalmPosition.ToUnityScaled ());
            objInLastFrame.transform.rotation = transform.rotation;

            return objInLastFrame;
    		} else {
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
                    vector = transform.TransformPoint(vector);
                    break;
                case MagicType.Fireball:
                    // TODO: Need to get the fireball to be shot where the user is facing
                    magicType = MagicConstant.FIREBALL_RELEASE_NAME;
                    vector = vector + (Vector3.up * 0.2f);
                    vector = transform.TransformPoint(vector);
                    break;
                case MagicType.IceWall:
                    magicType = MagicConstant.ICEWALL_NAME;
                    vector = vector + (Vector3.up * 0.4f);
                    vector = transform.TransformPoint(vector);
                    vector.y = 0f;
                    Instantiate(Resources.Load (MagicConstant.ICEWALL_SUMMON), vector, rotation);
                    break;
                default:
                    break;
        				}

            objInCurrFrame = (GameObject)Instantiate(Resources.Load (magicType), vector, rotation); 

            switch (type) {
                case MagicType.Fireball:
                    objInCurrFrame.name = lastSize.ToString();
                    break;
                case MagicType.IceWall:
                    Vector3 rotate = new Vector3(-90, 0, 0);
                    objInCurrFrame.transform.Rotate(rotate);
                    break;
                default:
                    break;
            }

            lastCall = type;
            lastSize = size;
            
            return objInCurrFrame;
        } 
  	}
        
    void neutralize() {
        if (objInLastFrame != null && lastCall == MagicType.FireCharge) {
            Destroy (objInLastFrame.gameObject); 
        }

        chargeCounter = 0;
        idleCounter = 0;
        canCharge = false;

        nc.resetPeltier();
    }

    void idling() {
        //buffer period to confirm doing nothing when hand's on screen
        if (idleCounter > IDLE_THRESHOLD) {
            //Debug.Log ("doing nothing");
            neutralize ();
        }
        idleCounter++;
    }
    
    // Use this for initialization
    void Start () {
    		controller = new Controller ();
        nc = new NetworkController();
        cd = new CooldownHelper ();

        controller.EnableGesture (Gesture.GestureType.TYPE_CIRCLE);
  	}
  	
  	// Update is called once per frame
  	void Update () {
        mainHand = controller.Frame ().Hands [0];

        objInLastFrame = objInCurrFrame;
        objInCurrFrame = null;

        //testing purposes
        if (Input.GetKeyDown(KeyCode.I))
        {
            objInCurrFrame = CreateObject(MagicType.IceWall);
            return;
        }
        
        //Debug.Log (HandHelper.isFaceForward(mainHand, controller.Frame()) + " " + mainHand.Direction + ":" + mainHand.PalmNormal);

        //cooling down, don't do anything else
        if (cd.coolingDown (nc)) {
            neutralize ();
            //TODO: have this work based on data from servo motor
        //hand stuff
        } else if (mainHand != null && mainHand.IsValid) {
            if (HandHelper.isClosedFist(mainHand)) {
                canCharge = true;
                //Debug.Log ("Closed fist");
            } else if (HandHelper.isFaceUp (mainHand, controller.Frame ()) && canCharge) {
                //TODO: resize fireball instead of creating different ones?
                if (chargeCounter <= LONG_THRESHOLD) {
                    if (chargeCounter > SHORT_THRESHOLD && chargeCounter <= MEDIUM_THRESHOLD) {
                        objInCurrFrame = CreateObject (MagicType.FireCharge, Size.Medium);
                    } else if (chargeCounter > MEDIUM_THRESHOLD) {
                        objInCurrFrame = CreateObject (MagicType.FireCharge, Size.Large);
                    } else {
                        objInCurrFrame = CreateObject (MagicType.FireCharge, Size.Small);
                    }

                    //Debug.Log ("Charging: " + chargeCounter);
                
                    //TODO: differentiate hotness when we have that established
                    nc.heatPeltier ();
                    chargeCounter++;
                    //overloaded, go into cooldown
                } else {
                    Debug.Log ("Charged too long, in cooldown mode");
                    neutralize ();
                    cd.startCooldown ();
                }
                //shoot charged object
            } else if (HandHelper.isFaceForward (mainHand, controller.Frame ()) && chargeCounter > MIN_THRESHOLD) {
                //TODO: change fireball based on chargedness
                objInCurrFrame = CreateObject (MagicType.Fireball);
                Debug.Log ("Shoot fireball");

                neutralize ();
                //charge ice wall
            } else if (HandHelper.isFaceForward (mainHand, controller.Frame ())) {
                Vector3 palmPos = mainHand.PalmPosition.ToUnityScaled ();
                Collider[] hitColliders = Physics.OverlapSphere(transform.TransformPoint(palmPos), 2);

                bool collided = false;

                for (int i = 0; i < hitColliders.Length; i++) {
                    if (hitColliders[i].tag == MagicConstant.ICEWALL_TAG) {
                        hitColliders[i].SendMessage(MagicConstant.ICEWALL_CAST_TAG);
                        collided = true;
                    }
                }

                //create an ice wall if there isn't a nearby one and a circle gesture was performed
                if (!collided) {
                    foreach (Gesture gesture in controller.Frame().Gestures(controller.Frame(10))) {
                        if (gesture.Type == Gesture.GestureType.TYPE_CIRCLE) {
                            objInCurrFrame = CreateObject(MagicType.IceWall);
                            break;
                        }
                    }
                }

            } else {
                idling();
            }
        } else {
            idling();
        }
    }
}

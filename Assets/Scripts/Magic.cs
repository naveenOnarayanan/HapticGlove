using Leap;
using SimpleJSON;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

public class Magic : MonoBehaviour {
    NetworkController nc;

    Controller controller;
    Hand mainHand;

    int chargeCounter = 0;
    int cooldownCounter = 0;

    const int MIN_THRESHOLD = 20;
    const int SHORT_THRESHOLD = 200;
    const int MEDIUM_THRESHOLD = 400;
    const int LONG_THRESHOLD = 600;
    const int COOLDOWN_THRESHOLD = 500;

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
            //TODO: differentiate difference ices
            case MagicType.IceCharge:
                switch (size) {
                case Size.Small:
                    magicType = MagicConstant.ICEWALL_SMALL_NAME;
                    break;
                case Size.Medium:
                    magicType = MagicConstant.ICEWALL_MEDIUM_NAME;
                    break;
                case Size.Large:
                    magicType = MagicConstant.ICEWALL_LARGE_NAME;
                    break;
                }
                break;
            case MagicType.Fireball:
                // TODO: Need to get the fireball to be shot where the user is facing
                magicType = MagicConstant.FIREBALL_RELEASE_NAME;
                break;
            default:
                break;
            }

            if (magicType != null) {
                obj = (GameObject)Instantiate(Resources.Load (magicType), vector, Camera.main.transform.rotation); 
            }
        } 
        lastCall = type;
    }

    void neutralize() {
        obj = null;
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

        //gestures within last 10 frames

        foreach (Gesture gesture in controller.Frame().Gestures(controller.Frame(10))) {
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
        //shoot charged object
        } else if (MagicHelper.handFaceForward(mainHand, controller.Frame()) && chargeCounter > MIN_THRESHOLD) {
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

            canCharge = false;
        }
    }
}

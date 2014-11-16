using Leap;
using SimpleJSON;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Magic : MonoBehaviour {
    public Socket client;
    IPEndPoint servoServer;
    IPEndPoint peltierServer;
    
    public int servoPort;
    public int peltierPort;
    public string IP;
  	
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
    //TODO: change this to true
    bool connected = false;

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
  	
    IEnumerator Delay(int seconds) {
        yield return new WaitForSeconds (seconds);
    }

    void sendData(string data, IPEndPoint server) {
        if (!connected)
            return;

        //TODO: process data as json
        Int32 timestamp = (Int32)(DateTime.UtcNow.Subtract (new DateTime (1970, 1, 1))).TotalSeconds;
        string json = "{\"timestamp\":" + timestamp + ", " + data + "}";
        byte[] d = Encoding.UTF8.GetBytes (json);

        client.SendTo (d, d.Length, SocketFlags.None, server);
    }

    void neutralize() {
        fireballCharged = false;
        chargeCounter = 0;
        sendData("\"temperature\": 0", peltierServer);
    }
    
  	// Use this for initialization
  	void Start () {
    		controller = new Controller ();
    		controller.EnableGesture (Gesture.GestureType.TYPE_CIRCLE);
    		controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);
        
    		//instantiate server
        IP = "169.254.191.81";
        servoPort = 3000;
        peltierPort = 3001;
        servoServer = new IPEndPoint (IPAddress.Parse (IP), servoPort);
        peltierServer = new IPEndPoint (IPAddress.Parse (IP), peltierPort);
        client = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        try {
            //TODO: this doesn't work for UDP, how do a check if connected?
            //client.Connect (servoServer);
            //client.Connect (peltierServer);
        } catch {
            connected = false;
        }
  	}

    Boolean handFaceUp(Frame frame) {
        Pointable finger;
        bool fingerFlat = true;
        
        //Debug.Log (transform.TransformPoint(mainHand.PalmPosition.ToUnityScaled()));
        
        for (int i = 0; i < frame.Pointables.Count; i++) {
            finger = frame.Pointables[i];
            fingerFlat &= finger.Direction.y > 0;
        }

        return mainHand.Direction.y > 0.5 && mainHand.PalmNormal.y < 0 && mainHand.PalmNormal.z < 0 && fingerFlat;
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
            sendData ("\"temperature\": 0", peltierServer);
            sendData ("\"angle\": 180", servoServer);

            cooldownCounter++;
        } else if (cooldownCounter == COOLDOWN_THRESHOLD) {
            //release fingers
            sendData ("\"angle\": 0", servoServer);

            Debug.Log ("Done cooldown");
            cooldownCounter = 0;
        // charging
        } else if (handFaceUp(controller.Frame ())) {
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
                sendData("\"temperature\": 10", peltierServer);
                chargeCounter++;
            //overloaded, go into cooldown
            } else {
                Debug.Log ("Charged too long, in cooldown mode");
                Destroy (obj.gameObject); 

                neutralize();
                cooldownCounter++;
            }
        //shoot fireball
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

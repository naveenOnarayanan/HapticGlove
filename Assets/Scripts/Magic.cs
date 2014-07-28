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

  	int counter = 0;

  	GameObject obj;
  	MagicType lastCall;
    bool fireballCharged = false;
    bool cooldown = false;

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

    void sendData(string data, IPEndPoint server) {
        //TODO: process data as json
        Int32 timestamp = (Int32)(DateTime.UtcNow.Subtract (new DateTime (1970, 1, 1))).TotalSeconds;
        string json = "{\"timestamp\":" + timestamp + ", " + data + "}";
        byte[] d = Encoding.UTF8.GetBytes (json);

        client.SendTo (d, d.Length, SocketFlags.None, server);
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
            //Debug.Log (counter);
            if (counter <= 600 && !cooldown) {
                // Counts for 200 frames before switching to larger flame
                if (counter > 200 && counter <= 400) {
                    CreateObject(MagicType.FireMedium);
                } else if (counter > 400) {
                    CreateObject(MagicType.FireLarge);
                } else {
                    CreateObject (MagicType.FireSmall);
                }

                fireballCharged = true;
                //TODO: differentiate hotness when we have that established
                sendData("\"temperature\": 10", peltierServer);
                counter++;
            } else if (fireballCharged) {
                //TODO: not working as expected. Delay not waiting, and still spamming fire
                Debug.Log ("Charged too long, in cooldown mode");
                Destroy (obj.gameObject); 
                fireballCharged = false;
                cooldown = true;

                sendData("\"temperature\": 0", peltierServer);
                sendData ("\"angle\": 180", servoServer);
                Delay (5);
                sendData ("\"angle\": 0", servoServer);
                Debug.Log ("Done cooldown");
                cooldown = false;
            }
        } else if (gesture == Gesture.GestureType.TYPE_CIRCLE && fireballCharged) {
            // TODO: need to add logic such that if the fireball was not shot within X amount of time then
            // fireball will not be released
            CreateObject (MagicType.Fireball);
            fireballCharged = false;
            sendData("\"temperature\": 0", peltierServer);
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

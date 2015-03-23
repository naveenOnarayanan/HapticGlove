using UnityEngine;
using System.Collections.Generic;
using Leap;


public class ExplanationControl : MonoBehaviour {
	int counter = 0;
    float deltaTime = 0f;
    
	private NetworkController nc;

	TextMesh explanationText;

	Controller controller;
	Hand mainHand;
	
    List<Explanation> explanations = new List<Explanation>();

	void OnApplicationQuit() {
		if (nc != null) {
			nc.stopThread();
		}
	}

	abstract class Explanation {
		public string text = "";

		public abstract bool ReadyForNextInstr (float deltaTime, Frame frame, Hand hand);
	}

	class TimedExplanation : Explanation {
		float endTime = 0f;

		public TimedExplanation(string text, float endTime) {
			this.text = text;
			this.endTime = endTime;
		}

		public override bool ReadyForNextInstr(float deltaTime, Frame frame, Hand hand) {
			if (deltaTime >= endTime) {
				return true;
			} else {
				return false;
			}
		}
	}

	class GestureExplanation : Explanation {
		string gestureType;
		float thresholdTime;

		public GestureExplanation(string text, string gestureType, float thresholdTime) {
			this.text = text;
			this.gestureType = gestureType;
			this.thresholdTime = thresholdTime;
		}

		public override bool ReadyForNextInstr(float deltaTime, Frame frame, Hand hand) {
			if (deltaTime >= thresholdTime) {
				switch (gestureType) {
					case "fist":
						if (hand != null) {
							return HandHelper.isClosedFist (hand);
						} else {
							return false;
						}
					case "fire":
						return GameObject.FindGameObjectsWithTag (MagicConstant.FIRECHARGE_TAG).Length > 0;
					case "fireball":
						return GameObject.FindGameObjectsWithTag (MagicConstant.FIREBALL_TAG).Length > 0;
					case "forward":
						if (hand != null) {
							return HandHelper.isFaceForward (hand, frame);
						} else {
							return false;
						}
					case "ice":
						return GameObject.FindGameObjectsWithTag (MagicConstant.ICEWALL_TAG).Length > 0;
					default:
						Debug.Log ("unrecognized gesture");
						return false;
				}
			} else {
				return false;
			}
		}
	}

  	void Start () {
        GameObject explanationTextL1 = GameObject.FindGameObjectWithTag ("ExplanationText_L1");
		explanationText = explanationTextL1.GetComponent<TextMesh> ();

        // Adding initial configuration message
		explanations.Add(new TimedExplanation("Prepare for battle! You are about to\npartake in a magical duel...\nTO THE DEATH!!", 5));
        explanations.Add(new GestureExplanation("Arm yourself with fireballs.\nTo create a fireball, first make a fist.", "fist", 2));
		explanations.Add(new GestureExplanation("Now open your fist, palm facing up.", "fire", 2));
		explanations.Add(new TimedExplanation("This charges the fire. The more charged,\nthe more damage it does.", 5));
		explanations.Add(new GestureExplanation("Thrust your palm forwards to shoot it.", "fireball", 2));
		explanations.Add(new TimedExplanation("If you charge too long, you'll overload\n and be unable to cast spells\nfor a while.", 5));
		explanations.Add(new GestureExplanation("Defend yourself with ice walls.\nHold your palm forward towards\nthe screen.", "forward", 2));
		explanations.Add(new GestureExplanation("Now make a circular motion.", "ice", 2));
		explanations.Add(new GestureExplanation("Keep holding your palm out to charge it.\nThe more charged the stronger\nthe wall.", "forward", 3));
		explanations.Add(new TimedExplanation("You are now ready to fight!", 3));

		explanationText.text = explanations[counter].text;

		nc = NetworkController.instance();
		controller = new Controller ();
		//nc.accelGyro();
    }

  	// Update is called once per frame
  	void Update () {
		deltaTime += Time.deltaTime;

		Frame frame = controller.Frame ();
		Hand mainHand = frame.Hands[0];

		//load next explanation
		if (explanations[counter].ReadyForNextInstr(deltaTime, frame, mainHand)) {
			counter = counter + 1;

			if (counter == explanations.Count) {
				Application.LoadLevel(Application.loadedLevel + 1);
			} else {
				explanationText.text = explanations[counter].text;
				deltaTime = 0;
			}
        }
  	}
}

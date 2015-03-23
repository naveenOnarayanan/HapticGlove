using UnityEngine;
using System.Collections.Generic;


public class ExplanationControl : MonoBehaviour {
	
    int counter = 0;
    float deltaTime = 0f;
    bool continueInstructions = true;
	private NetworkController nc;
	TextMesh explanationText;
	
    List<Explanation> explanations = new List<Explanation>();

	void OnApplicationQuit() {
		if (nc != null) {
			nc.stopThread();
		}
	}

	abstract class Explanation {
		public string text = "";

		public abstract bool ReadyForNextInstr (float deltaTime);
	}

	class TimedExplanation : Explanation {
		float endTime = 0f;

		public TimedExplanation(string text, float endTime) {
			this.text = text;
			this.endTime = endTime;
		}

		public override bool ReadyForNextInstr(float deltaTime) {
			if (deltaTime >= endTime) {
				return true;
			} else {
				return false;
			}
		}
	}

  	void Start () {
        GameObject explanationTextL1 = GameObject.FindGameObjectWithTag ("ExplanationText_L1");
		explanationText = explanationTextL1.GetComponent<TextMesh> ();

		//if you charge too long, you'll overload and be unable to cast spells for a while
		//defend yourself with ice walls
		//hold your palm forward towards the screen <PALM FORWARD IMAGE>
		//now make a circular motion <CIRCLE IMAGE>
		//keep holding your palm out to charge it. the more charged the stronger the wall
		//you are now ready to fight!

        // Adding initial configuration message
		explanations.Add(new TimedExplanation("Prepare for battle! You are about to\npartake in a magical duel...\nTO THE DEATH!!", 3));
        explanations.Add(new TimedExplanation("Arm yourself with fireballs\nTo create a fireball, first make a fist.", 3));
		explanations.Add(new TimedExplanation("Now open your fist, palm facing up.", 3));
		explanations.Add(new TimedExplanation("This charges the fire. The more charged,\nthe more damage it does.", 3));
		explanations.Add(new TimedExplanation("Thrust your palm forwards to shoot it.", 3));

		explanationText.text = explanations[counter].text;

		nc = NetworkController.instance();
		//nc.accelGyro();
    }

  	// Update is called once per frame
  	void Update () {
		deltaTime += Time.deltaTime;

		//load next explanation
		if (explanations[counter].ReadyForNextInstr(deltaTime)) {
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

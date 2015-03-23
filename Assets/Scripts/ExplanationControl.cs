using UnityEngine;
using System.Collections.Generic;


public class ExplanationControl : MonoBehaviour {

    const float EXPLANATION_TIME = 5f;
    int counter = 0;
    float deltaTime = 5f;
    bool continueInstructions = true;
	private NetworkController nc;

    class Explanation {
        public string explanationTxtL1 = "";
        public string explanationTxtL2 = "";
        public Vector3 positionL1;
        public Vector3 positionL2 = new Vector3(0, 0);
        public Explanation(string explanationTxtL1, Vector3 positionL1, string explanationTxtL2, Vector3 positionL2) {
            this.explanationTxtL1 = explanationTxtL1;
            if (explanationTxtL2 != null) {
              this.explanationTxtL2 = explanationTxtL2;
            }
            this.positionL1 = positionL1;
            if (positionL2 != null) {
              this.positionL2 = (Vector3)positionL2;
            }
        }
    }

    List<Explanation> explanations = new List<Explanation>();

	void OnApplicationQuit() {
		if (nc != null) {
			nc.stopThread();
		}
	}

  	void Start () {
        GameObject explanationTextL1 = GameObject.FindGameObjectWithTag ("ExplanationText_L1");
        GameObject explanationTextL2 = GameObject.FindGameObjectWithTag ("ExplanationText_L2");

        // Adding initial configuration message
        explanations.Add(new Explanation(
            "Lets configure", new Vector3(-5.866141f, 7.897055f),
            "your new glove!", new Vector3(-6.437281f, 4.92359f)
        ));
        explanations.Add (new Explanation("Please make a fist!", new Vector3(-8.111927f, 7.41565f), null, new Vector3(0, 0)));
		explanations.Add (new Explanation("Good! Now relax", new Vector3(-6.425037f, 7.897055f), "your hand!", new Vector3(-6.437281f, 4.92359f)));
		explanations.Add (new Explanation("Face your palm", new Vector3(-6.425037f, 7.897055f), "upwards", new Vector3(-6.437281f, 4.92359f)));
		explanations.Add (new Explanation("Now, face your", new Vector3(-6.425037f, 7.897055f), "palm forwards", new Vector3(-6.437281f, 4.92359f)));
		nc = NetworkController.instance();
		//nc.accelGyro();
    }

	bool ReadyForThisInstr() {
		return deltaTime < 0;
	}

	bool EndOfThisInstr() {
		return deltaTime == 1;
	}

  	// Update is called once per frame
  	void Update () {
		deltaTime -= Time.deltaTime;

		//load next explanation
		if (ReadyForThisInstr() && continueInstructions) {
			if (counter >= 5) {
				Application.LoadLevel(Application.loadedLevel + 1);
				return;
			}
            // Find two explanation text lines
            GameObject explanationTextL1 = GameObject.FindGameObjectWithTag ("ExplanationText_L1");
            GameObject explanationTextL2 = GameObject.FindGameObjectWithTag ("ExplanationText_L2");

            Vector3 textPositionL1 = explanations[counter].positionL1;
            Vector3 textPositionL2 = explanations[counter].positionL2;
            textPositionL1.z = textPositionL2.z = explanationTextL1.transform.position.z;

            explanationTextL1.GetComponent<TextMesh>().text = explanations[counter].explanationTxtL1;
            explanationTextL2.GetComponent<TextMesh>().text = explanations[counter].explanationTxtL2;

            explanationTextL1.transform.position = textPositionL1;
            explanationTextL2.transform.position = textPositionL2;

            deltaTime = EXPLANATION_TIME;
            counter = counter + 1;

			// Prevents text from changing until we receive calibrated values from the game
			if (counter > 1) {
				continueInstructions = false;
			}
        }

		/*
		// Reset bottom motor for closed fist
		if (counter == 2) {
			if (ReadyForThisInstr()) {
				nc.resetBottomServo();
				deltaTime = 3f;
				continueInstructions = true;
			}

			if (EndOfThisInstr()) {
				UserData.servo_closed_fist = UserData.servo;
			}
		}
		// Reset top 2 motors for Open palm
		else if (counter == 3) {
			if (ReadyForThisInstr()) {
				nc.resetTopServo();
				deltaTime = 3f;
				continueInstructions = true;
			}

			if (EndOfThisInstr()) {
				UserData.servo_neutral = UserData.servo;
			}
		}
		// Open palm facing up
		else if (counter == 4) {
			if (ReadyForThisInstr()) {
				deltaTime = 3f;
				continueInstructions = true;
			}

			if (EndOfThisInstr()) {
				UserData.accel_gyro_face_up = UserData.accel_gyro;
			}
		}
		// Open palm facing forward
		else if (counter == 5) {
			if (ReadyForThisInstr()) {
				deltaTime = 3f;
				continueInstructions = true;
			}

			if (EndOfThisInstr()) {
				UserData.accel_gyro_face_forward = UserData.accel_gyro;
			}
		}
		*/
  	}
}

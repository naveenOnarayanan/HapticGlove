using UnityEngine;
using System.Collections.Generic;


public class ExplanationControl : MonoBehaviour {

    const float EXPLANATION_TIME = 5f;
    int counter = 0;
    float deltaTime = 5f;
    bool continueInstructions = false;

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

  	void Start () {
        GameObject explanationTextL1 = GameObject.FindGameObjectWithTag ("ExplanationText_L1");
        GameObject explanationTextL2 = GameObject.FindGameObjectWithTag ("ExplanationText_L2");

        // Adding initial configuration message
        explanations.Add(new Explanation(
            "Lets configure", new Vector3(-5.866141f, 7.897055f),
            "your new glove!", new Vector3(-6.437281f, 4.92359f)
        ));
        explanations.Add (new Explanation("Please make a fist!", new Vector3(-8.111927f, 7.41565f), null, new Vector3(0, 0)));
        explanations.Add (new Explanation("Open your palm!", new Vector3(-6.425037f, 7.897055f), null, new Vector3(0, 0)));
    }
  	// Update is called once per frame
  	void Update () {
        deltaTime -= Time.deltaTime;
        if (deltaTime < 0 && (counter <= 2 || continueInstructions)) {
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
        }

  	}
}

using UnityEngine;
using System.Collections;

public class IceWall : Character {
    int chargeCounter = 0;
    Vector3 startScale;
    float scaleFactor = 1.0F;

  	// Use this for initialization
  	void Start () {
        health = 25;
        totalHealth = 25;
        startScale = this.gameObject.transform.localScale;
        Debug.Log (health);
  	}
  	
  	// Update is called once per frame
  	void Update () {
        if (health <= 0) {
              Destroy (this.gameObject);
        }
  	}
    
    //response when ice cast nearby
    //has to be named same thing as MagicConstant.ICEWALL_CAST_TAG
    void IceCast() {
        chargeCounter++;

        //increment size
        scaleFactor += 0.002F;
        this.gameObject.transform.localScale = startScale * scaleFactor;

        //increment health
        if (chargeCounter > 60) {
            health += 5;
            totalHealth += 5;
            Debug.Log ("charging ice: " + health);
            chargeCounter = 0;
        }
    }
}

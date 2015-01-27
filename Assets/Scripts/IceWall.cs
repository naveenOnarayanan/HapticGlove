using UnityEngine;
using System.Collections;

public class IceWall : Character {
    int chargeCounter = 0;

  	// Use this for initialization
  	void Start () {
          health = 25;
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

        if (chargeCounter > 60) {
            health += 5;
            Debug.Log ("charging ice: " + health);
            chargeCounter = 0;
        }
    }
}

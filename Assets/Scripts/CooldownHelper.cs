using UnityEngine;
using System;
using System.Collections;
using Leap;

//helper methods going into cooldown
public class CooldownHelper {
    int cooldownCounter;
    int COOLDOWN_THRESHOLD = 500;

    public CooldownHelper() {
        cooldownCounter = 0;
    }

    public bool coolingDown(NetworkController nc) {
        //cooldown mode, don't let it do anything else
        if (cooldownCounter > 0 && cooldownCounter < COOLDOWN_THRESHOLD) {
            //pull fingers taught
            nc.pullServo();

            cooldownCounter++;
        } else if (cooldownCounter == COOLDOWN_THRESHOLD) {
            //release fingers
            //nc.resetTopServo ();
            
            Debug.Log ("Done cooldown");
            cooldownCounter = 0;
        }

        return cooldownCounter > 0;
    }

    public void startCooldown() {
        cooldownCounter++;
    }
}



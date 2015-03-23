using UnityEngine;
using System;
using System.Collections;
using Leap;

//helper methods for doing magic stuffs
public static class HandHelper {
    public static bool isFaceUp(Hand hand, Frame frame) {
		/*
        Pointable finger;
        bool fingerFlat = true;
        
        //Debug.Log (transform.TransformPoint(hand.PalmPosition.ToUnityScaled()));
        for (int i = 0; i < frame.Pointables.Count; i++) {
            finger = frame.Pointables[i];
            fingerFlat &= finger.Direction.y > 0;
        }
        */
        
		bool fromLeap = hand.Direction.y > 0.5 && hand.PalmNormal.y < 0 && hand.PalmNormal.z < 0;// && fingerFlat;
		//bool fromSensors = UserData.isFaceUp();

		return fromLeap;// || fromSensors;
    }

    public static bool isFaceForward(Hand hand, Frame frame) {
		bool fromLeap = hand.PalmNormal.y > 0.8 && hand.PalmNormal.z > -0.2 && hand.PalmNormal.z < 0.5;
		//bool fromSensors = UserData.isFaceForward();
		
		return fromLeap;// || fromSensors;
    }

    static int getExtendedFingers(Hand hand) {
        int f = 0;

        for (int i = 0; i < hand.Fingers.Count; i++) {
            if (hand.Fingers[i].IsExtended) {
                f++;
            }
        }

        return f;
    }

    public static bool isClosedFist(Hand hand) {
		bool fromLeap = getExtendedFingers(hand) == 0;
		//bool fromSensors = UserData.isClosedFist();
		
		return fromLeap;// || fromSensors;
    }
}

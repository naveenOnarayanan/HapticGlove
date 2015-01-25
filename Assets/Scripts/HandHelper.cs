using UnityEngine;
using System;
using System.Collections;
using Leap;

//helper methods for doing magic stuffs
public static class HandHelper {
    public static bool isFaceUp(Hand mainHand, Frame frame) {
        Pointable finger;
        bool fingerFlat = true;
        
        //Debug.Log (transform.TransformPoint(mainHand.PalmPosition.ToUnityScaled()));
        
        for (int i = 0; i < frame.Pointables.Count; i++) {
            finger = frame.Pointables[i];
            fingerFlat &= finger.Direction.y > 0;
        }
        
        return mainHand.Direction.y > 0.5 && mainHand.PalmNormal.y < 0 && mainHand.PalmNormal.z < 0 && fingerFlat;
    }

    public static bool isFaceForward(Hand mainHand, Frame frame) {
        return mainHand.PalmNormal.y > 0.8 && mainHand.PalmNormal.z > 0.1 && mainHand.PalmNormal.z < 0.5;
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
        //return getExtendedFingers(hand) == 0;
        return true;
    }
}

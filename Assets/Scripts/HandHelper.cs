using UnityEngine;
using System;
using System.Collections;
using Leap;

//helper methods for doing magic stuffs
public static class HandHelper {
    public static bool isFaceUp(Hand hand, Frame frame) {
        Pointable finger;
        bool fingerFlat = true;
        
        //Debug.Log (transform.TransformPoint(hand.PalmPosition.ToUnityScaled()));
        
        for (int i = 0; i < frame.Pointables.Count; i++) {
            finger = frame.Pointables[i];
            fingerFlat &= finger.Direction.y > 0;
        }
        
        return hand.Direction.y > 0.5 && hand.PalmNormal.y < 0 && hand.PalmNormal.z < 0 && fingerFlat;
    }

    public static bool isFaceForward(Hand hand, Frame frame) {
        return hand.PalmNormal.y > 0.8 && hand.PalmNormal.z > 0.1 && hand.PalmNormal.z < 0.5;
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
        return getExtendedFingers(hand) == 0;
    }
}

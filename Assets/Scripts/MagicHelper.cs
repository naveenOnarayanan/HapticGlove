using UnityEngine;
using System;
using System.Collections;
using Leap;

//helper methods for doing magic stuffs
public static class MagicHelper {
    public static bool handFaceUp(Hand mainHand, Frame frame) {
        Pointable finger;
        bool fingerFlat = true;
        
        //Debug.Log (transform.TransformPoint(mainHand.PalmPosition.ToUnityScaled()));
        
        for (int i = 0; i < frame.Pointables.Count; i++) {
            finger = frame.Pointables[i];
            fingerFlat &= finger.Direction.y > 0;
        }
        
        return mainHand.Direction.y > 0.5 && mainHand.PalmNormal.y < 0 && mainHand.PalmNormal.z < 0 && fingerFlat;
    }
}

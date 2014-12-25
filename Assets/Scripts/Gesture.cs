using Leap;
using SimpleJSON;
using System;
using System.Collections;
using System.Text;
using UniRx;
using UnityEngine;

public class HandGesture : MonoBehaviour {
    private Subject<Hand> gestures;
    public HandGesture (Object[] listeners) {
        this.gestures = new Subject<Hand> ();
        for (int i = 0; i < listeners.Length; i++) {
            this.gestures.OnNext(listeners[i]);
        }
    }

    public void handleGesture(Type clazz, int numObjectsPerBuffer, long timeLimit, long distanceThreshold) {
        this.gestures
            // Buffer objects (i.e. 2 hands, 10 fingers, etc..)
            .Buffer (numObjectsPerBuffer, 1)
            .Select ( h => h[1].PalmPosition.y - h[0].PalmPosition.y)
            .Scan (0f, (dt, d) => Math.Sign(dt) == Math.Sign(d) ? dt + d : d)
            .Where(dt => Math.Abs(dt) > distanceThreshold)
            // filter out redundant updates for same direction
            .Scan(0f, (dt, d) => Math.Sign(dt) == Math.Sign(d) ? dt : d)
            .DistinctUntilChanged()
            // apply timestamp to updates that exceeded distance threshold
            .Timestamp()
            // buffer two updates to collect movements in opposite directions
            .Buffer(2, 1)
            // check whether the movements happend in predefined time frame
            .Where(dt => dt[1].Timestamp.Ticks-dt[0].Timestamp.Ticks < timeLimit)
            // determine direction of gesture
            .Select(dt => (Math.Sign(dt[1].Value) == 1) ? GestureConstants.DIRECTION_DOWN : GestureConstants.DIRECTION_DOWN)
            // notify listeners via gesture stream
            .Subscribe(direction => {
                // TODO: Run gesture with direction passed in
            });


    }
}
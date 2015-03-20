using UnityEngine;
using System.Collections;

public static class UserData {
    public static double[] accel_gyro_face_forward;
	public static double[] accel_gyro_face_up;
    public static double[] servo_closed_fist;
	public static double[] servo_neutral;

    public static double[] accel_gyro;
    public static double[] servo;
    public static int timestamp = 0;
	
	//TODO: figure out what these thresholds are
	static double ACCEL_THRESHOLD = 10;
	static double SERVO_THRESHOLD = 10;

	static bool isSimilar(double[] data, double[] standard, double threshold) {
		bool similar;

		for (int i = 0; i < data.Length; i++) {
			similar = (data[i] > (standard[i] - threshold)) && (data[i] < (standard[i] + threshold));
			if (!similar) {
				return false;
			}
		}

		return true;
	}

	public static bool isFaceUp() {
		return isSimilar (accel_gyro, accel_gyro_face_up, ACCEL_THRESHOLD);
	}

	public static bool isFaceForward() {
		return isSimilar(accel_gyro, accel_gyro_face_forward, ACCEL_THRESHOLD);
	}

	public static bool isClosedFist() {
		return isSimilar(servo, servo_closed_fist, SERVO_THRESHOLD);
	}
}
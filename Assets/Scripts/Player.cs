using UnityEngine;
using System.Collections;

public class Player : Character {
  //private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];

	// Use this for initialization
	void Start () {
        health = 100;
        Debug.Log (health);
	}
	
	// Update is called once per frame
	void Update () {
      if (health <= 0) {
            Debug.Log ("game over");
            //TODO: end game

      }
	}
}

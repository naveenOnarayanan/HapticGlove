using UnityEngine;
using System.Collections;

public class Player : Character {
  //private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];

	// Use this for initialization
	protected void Start () {
        health = 100;
        totalHealth = 100;

		base.Start ();
	}

	protected void CheckGameOver() {
		if (health <= 0) {
			Application.LoadLevel("replay");
		}
	}
	
	// Update is called once per frame
	protected void Update () {
		CheckGameOver();

		base.Update ();
	}
}

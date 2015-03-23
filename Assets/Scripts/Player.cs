using UnityEngine;
using System.Collections;

public class Player : Character {
  //private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];

	// Use this for initialization
	protected void Start () {
        health = 300;
        totalHealth = 300;

		base.Start ();
	}

	protected void CheckGameOver() {
		if (health <= 0) {
			MagicConstant.LOSER = this.name;
			Application.LoadLevel(MagicConstant.REPLAY_LEVEL);
		}
	}
	
	// Update is called once per frame
	protected void Update () {
		CheckGameOver();

		base.Update ();
	}
}

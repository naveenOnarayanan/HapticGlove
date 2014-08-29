using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
  public int health;
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

  void OnParticleCollision(GameObject collision){
      Debug.Log (collision.name);

      if (collision.name == "Fireball") {
          //TODO: make explosion
          Destroy(collision);

          //TODO: change health decrement based on type of fireball
          health -= 25;
          Debug.Log(health);
      }
  }
}

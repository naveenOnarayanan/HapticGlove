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
      Debug.Log ("Collision has occured on: " + collision.name);

      if (collision.tag == MagicConstant.FIREBALL_TAG) {

          //TODO: change health decrement based on type of fireball
          health -= 25;
          Debug.Log(health);
      }
  }
}

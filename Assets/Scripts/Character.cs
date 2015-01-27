using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
    public int health;
    
    void OnParticleCollision(GameObject collision){
        Debug.Log ("Collision has occured on: " + collision.name);
        
        if (collision.tag == MagicConstant.FIREBALL_TAG) {
            
            //TODO: change health decrement based on type of fireball
            health -= 25;
            Debug.Log(health);
        }
    }
}

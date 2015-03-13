using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
    public int health;
    const int SMALL_DMG = 25;
    const int MED_DMG = 50;
    const int LARGE_DMG = 75;
    
    void OnParticleCollision(GameObject collision){
        Debug.Log ("Collision has occured on: " + collision.name);
        
        if (collision.tag == MagicConstant.FIREBALL_TAG) {
            int dmg = 0;

            switch (collision.name) {
              case "Small":
                  dmg = SMALL_DMG;
                  break;
              case "Medium":
                  dmg = MED_DMG;
                  break;
              case "Large":
                  dmg = LARGE_DMG;
                  break;
              default:
                  break;
            }

            health -= dmg;
            Debug.Log(health);
        }
    }
}

using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

    void OnParticleCollision(GameObject gameObj) {
        Debug.Log("Collision");
        if (gameObj.tag == MagicConstant.FIREBALL_TAG) {
            Application.LoadLevel(Application.loadedLevel + 1);
        }
    }
}

using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

    void OnParticleCollision(GameObject gameObj) {
        if (gameObj.tag == MagicConstant.FIREBALL_TAG) {
            Application.LoadLevel("game");
        }
    }
}

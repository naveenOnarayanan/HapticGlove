using UnityEngine;
using System.Collections;

public class ReplayMenu : MonoBehaviour
{
	string winner;

	void Start() {
		if (MagicConstant.LOSER == MagicConstant.PLAYER) {
			winner = "Enemy wins!";
		} else if (MagicConstant.LOSER == MagicConstant.ENEMY) {
			winner = "You win!";
		}

		TextMesh winnerText = GameObject.Find (MagicConstant.WINNER_TEXT).GetComponent<TextMesh>();
		winnerText.text = winner;
	}

	void OnParticleCollision(GameObject gameObj) {
		if (gameObj.tag == MagicConstant.FIREBALL_TAG) {
			if (gameObj.transform.position.x > 0) {
				Application.LoadLevel(MagicConstant.TUTORIAL_LEVEL);
			} else {
				Application.LoadLevel(MagicConstant.GAME_LEVEL);

			}
		}
	}
}


using UnityEngine;
using System.Collections;

public class ReplayMenu : Menu
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
}


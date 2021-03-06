using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
    public int health;
    public int totalHealth;
    //public RectTransform healthBar;
    //public Transform cameraTrans;

	float boxW = 150f;
	float boxH = 20f;

    const int SMALL_DMG = 25;
    const int MED_DMG = 50;
    const int LARGE_DMG = 75;

    protected void Start () {
        //TODO: create health bar
    }

	/*
	void OnGUI() {

		Vector2 boxPosition = Camera.main.WorldToScreenPoint(transform.position);
		Debug.Log (Screen.height + " " + Screen.width + " " + boxPosition);
		// "Flip" it into screen coordinates
		boxPosition.y = Screen.height - boxPosition.y;
		
		// Center the label over the coordinates
		boxPosition.x -= boxW * 0.5f;
		boxPosition.y -= boxH * 0.5f;
		GUI.Box(new Rect(boxPosition.x, boxPosition.y, boxW, boxH), this.name);
	}
	*/


	
    protected void Update() {
        //TODO: update location of health bar relative to camera
        //Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, cameraTrans.position);
        //healthBar.anchoredPosition = screenPoint - canvasRectT.sizeDelta / 2f;
    }
    
    void OnParticleCollision(GameObject collision){
        Debug.Log ("Collision has occured on: " + collision.name);
        
        if (collision.tag == MagicConstant.FIREBALL_TAG) {
            int dmg = 0;

            switch (collision.name) {
              case MagicConstant.FIREBALL_SMALL_NAME:
                  dmg = SMALL_DMG;
                  break;
              case MagicConstant.FIREBALL_MEDIUM_NAME:
                  dmg = MED_DMG;
                  break;
              case MagicConstant.FIREBALL_LARGE_NAME:
                  dmg = LARGE_DMG;
                  break;
              default:
                  break;
            }

            health -= dmg;
            //TODO: update health bar
            Debug.Log(health);
        }
    }
}

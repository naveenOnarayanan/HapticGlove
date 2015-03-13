using UnityEngine;
using System.Collections;

public class Enemy : Player
{
    Animation animation;
    GameObject player;
    int moveCounter = 0;

    Vector3 getPlayerPos() {
        return player.collider.transform.position;
    }

    // Use this for initialization
    void Start ()
    {
        animation = this.GetComponent<Animation> ();
        animation.Play ("crouch");

        player = GameObject.Find ("player");
    }
	
    // Update is called once per frame
    void Update ()
    {
        moveCounter++;

        if (moveCounter == 10) {
            animation.Play ("run");
            Vector3 currPos = transform.position;
            Vector3 playerPos = getPlayerPos ();
            transform.position = Vector3.MoveTowards (currPos, playerPos, 0.1f);
            moveCounter = 0;
        }
    }
}


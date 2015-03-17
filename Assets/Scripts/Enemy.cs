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

    //TODO: this is currently incorrect
    Quaternion getPlayerDir() {
        Vector3 change =  getPlayerPos ();
        return Quaternion.Euler (change);
    }

    void shootFireball() {
        Quaternion dir = getPlayerDir ();
        //lift it off the floor
        Vector3 pos = transform.position + transform.forward;
        Instantiate(Resources.Load (MagicConstant.FIREBALL_RELEASE_NAME), pos, dir);
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

        //slow down movements
        if (moveCounter == 10) {
            moveCounter = 0;

            Vector3 currPos = transform.position;
            Vector3 playerPos = getPlayerPos ();
            float diff = (currPos - playerPos).sqrMagnitude;

            //TODO: rotate if not in line of sight, only run when facing you
            //TODO: run around walls instead of through them
            //TODO: randomly shoot fireballs

            //don't want to get too close
            if (diff > 10) {
                animation.Play ("run");
                Vector3 moveTowards = Vector3.MoveTowards (currPos, playerPos, 0.1f);
                moveTowards.y = 0;
                transform.position = moveTowards;
            }
        }
    }
}


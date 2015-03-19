using UnityEngine;
using System.Collections;

public class Enemy : Player
{
    Animation animation;
    GameObject player;
    int moveCounter = 0;
    int DIST_THRESHOLD = 50;

    Vector3 getPlayerPos() {
        return player.transform.position;
    }

    void ShootFireball() {
        //lift it off the floor
        Vector3 pos = transform.position + transform.up + (2 * transform.forward);
        GameObject fireball = (GameObject)Instantiate(Resources.Load (MagicConstant.FIREBALL_RELEASE_NAME), pos, transform.rotation);
        fireball.name = "Small";
        Destroy (fireball, 10);
    }

    //TODO: fix this
    bool InLineOfSight() {
        RaycastHit hit;
        Vector3 pos = transform.position + transform.up + transform.forward;
        Vector3 dir = transform.rotation.eulerAngles;
        Debug.Log (pos);

        if (Physics.Raycast(pos, dir, out hit)) {
            return hit.transform.position == getPlayerPos();
        } else {
            return false;
        }
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

        //testing purposes
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShootFireball();
        }

        //slow down movements
        if (moveCounter == 5) {
            moveCounter = 0;

            Vector3 currPos = transform.position;
            Vector3 playerPos = getPlayerPos ();
            float diff = (currPos - playerPos).sqrMagnitude;

            //TODO:
            /*
              if ray magnitude is farther than opponent, turn or run
              if ray is less than opponent, there's an obstacle, so turn 
              if ray is same as opponent
                if opponent is father than distance threshold, run towards them + randomly shoot fireballs
                if same as distance threshold, strafe
                if less than distance threshold, back up
            */

            Debug.Log(InLineOfSight());

            //don't want to get too close
            if (diff > DIST_THRESHOLD) {
                animation.Play ("run");
                Vector3 moveTowards = Vector3.MoveTowards (currPos, playerPos, 0.2f);
                moveTowards.y = 0;
                transform.position = moveTowards;
            } else {
                animation.Play("idle");
            }
        }
    }
}


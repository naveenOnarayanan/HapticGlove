using UnityEngine;
using System.Collections;
using System;

public class Enemy : Player
{
    Animation animation;
    GameObject player;
    int moveCounter = 0;

    int FIREBALL_CHANCE = 3; //1 in X chance to shoot fireball
    int DIST_THRESHOLD = 50;

    System.Random rng = new System.Random();

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
    
    void animate(string move) {
        if (!animation.IsPlaying (move)) {
            animation.Play(move);
        }
    }

    void Run() {
        animate("run");
        Vector3 moveTowards = Vector3.MoveTowards (transform.position, getPlayerPos(), 0.2f);
        moveTowards.y = 0;
        transform.position = moveTowards;
    }

    void TurnTowards() {
        Vector3 playerPos = getPlayerPos();
        playerPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(playerPos - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
    }

    //twice the turning of turnTowards
    void TurnAway() {

    }

    //returns infinity if no intersection
    Vector3 LineOfSightIntersection() {
        RaycastHit hit;
        Vector3 pos = transform.position + transform.up + transform.forward;
        Vector3 dir = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(pos, dir, out hit)) {
            return hit.transform.position;
        } else {
            return new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        }
    }

    // Use this for initialization
    void Start ()
    {
        animation = this.GetComponent<Animation> ();
        animate("crouch");

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

            Vector3 playerPos = getPlayerPos();
            float diff = (transform.position - playerPos).sqrMagnitude;

            Vector3 intersection = LineOfSightIntersection();
            float intersectionLength = intersection.sqrMagnitude;
            float playerLength = playerPos.sqrMagnitude;
            
            if (diff > DIST_THRESHOLD) {
                //approach player
                if (intersectionLength > playerLength) {
                    TurnTowards();
                    Run();
                } else if (intersectionLength < playerLength) {
                    //obstacle in the way
                    TurnAway();
                } else {
                    //randomly shoot fireballs towards player since facing him
                    int rand = rng.Next(20);
                    if (rand == 0) {
                        ShootFireball();
                    }

                    Run();
                }
            } else {
                //don't want to get too close, so only turn
                if (intersectionLength > playerLength) {
                    TurnTowards();
                } else if (intersectionLength == playerLength) {
                    //randomly shoot fireballs towards player since facing him
                    int rand = rng.Next(20);
                    if (rand == 0) {
                        ShootFireball();
                    }
                }

                animate("idle");
            } 
        }
    }
}


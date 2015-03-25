using UnityEngine;
using System.Collections;
using System;

public class Enemy : Player
{
    Animation animation;
    GameObject player;
    int moveCounter = 0;

	int STARTING_HEALTH = 100;

	int CHANCE_ATTACK_ICE = 20;
	int EASY_CHANCE_ATTACK = 1000;
	int MED_CHANCE_ATTACK = 50;
	int HARD_CHANCE_ATTACK = 20;

	int chanceAttackPlayer;

	int MOVE_THRESHOLD = 5;
    int DIST_THRESHOLD = 50;

	float TURN_SPEED = 0.2f;
	float RUN_SPEED = 0.2f;

	int WAIT_TIME = -100;

    System.Random rng = new System.Random();

    Vector3 getPlayerPos() {
        return player.transform.position;
    }

	//1 in X chance to shoot fireball
    void ShootFireball(int chance) {
		int rand = rng.Next(chance);
		if (rand == 0) {
			//lift it off the floor
			Vector3 pos = transform.position + transform.up + (2 * transform.forward);
			GameObject fireball = (GameObject)Instantiate (Resources.Load (MagicConstant.FIREBALL_RELEASE_NAME), pos, transform.rotation);
			fireball.name = MagicConstant.FIREBALL_SMALL_NAME;
			Destroy (fireball, 10);
		}
    }
    
    void animate(string move) {
        if (!animation.IsPlaying (move)) {
            animation.Play(move);
        }
    }

    void Run(Vector3 towards) {
        animate(MagicConstant.RUN_ANIM);
        Vector3 moveTowards = Vector3.MoveTowards (transform.position, towards, RUN_SPEED);
        moveTowards.y = 0;
        transform.position = moveTowards;
    }

    void TurnTowards() {
        Vector3 playerPos = getPlayerPos();
        playerPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(playerPos - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, TURN_SPEED);
    }

    //twice the turning of turnTowards
    void TurnAway() {
		Vector3 playerPos = getPlayerPos();
		playerPos.y = 0;
		Quaternion rotation = Quaternion.LookRotation(transform.position - playerPos);
		
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, TURN_SPEED * 2f);
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
    protected void Start ()
    {
		base.Start ();

		health = STARTING_HEALTH;
		totalHealth = STARTING_HEALTH;

        animation = this.GetComponent<Animation> ();
        animate(MagicConstant.CROUCH_ANIM);
		//give the player a sec to get started
		moveCounter = WAIT_TIME;

		chanceAttackPlayer = EASY_CHANCE_ATTACK;

        player = GameObject.Find (MagicConstant.PLAYER);
    }
	
    // Update is called once per frame
    protected void Update ()
    {
		base.Update ();

        moveCounter++;

        //testing purposes
        if (Input.GetKeyDown(KeyCode.O))
        {
            ShootFireball(1);
        }

		//ramp up difficulty of AI
		if (health < STARTING_HEALTH && health > 50 && chanceAttackPlayer != MED_CHANCE_ATTACK) {
			chanceAttackPlayer = MED_CHANCE_ATTACK;
		} else if (health <= 50 && chanceAttackPlayer != HARD_CHANCE_ATTACK) {
			chanceAttackPlayer = HARD_CHANCE_ATTACK;
		}

        //slow down movements
		if (moveCounter == MOVE_THRESHOLD) {
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
                    Run(playerPos);
                } else if (intersectionLength < playerLength) {
                    //obstacle in the way, potentially destroy it / turn away from it
					ShootFireball(CHANCE_ATTACK_ICE);
                    TurnAway();
                } else {
					ShootFireball(chanceAttackPlayer);
					Run(playerPos);
                }
            } else {
                //don't want to get too close, so only turn
                if (intersectionLength > playerLength) {
					Run(transform.position + transform.forward);
					TurnTowards();
                } else if (intersectionLength < playerLength) {
					ShootFireball(CHANCE_ATTACK_ICE);
					TurnAway();
				} else {
                    //randomly shoot fireballs towards player since facing him
					ShootFireball(chanceAttackPlayer);
					animate(MagicConstant.IDLE_ANIM);
                }
            } 
        }
    }
}


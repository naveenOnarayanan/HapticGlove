public class MagicConstant {

    // Tags to find and detect magic on element
	public const string FIRECHARGE_TAG = "FireCharge";
	public const string FIREBALL_TAG = "Fireball";
	public const string ICEWALL_TAG = "IceWall";

	//spell name in IceWall class
	public const string ICEWALL_CHARGE_SPELLNAME = "IceCharge";
    
    // Magic element resource names
	public const string FIRECHARGE_SMALL_NAME = "Fire_01";
	public const string FIRECHARGE_MEDIUM_NAME = "Fire_02";
	public const string FIRECHARGE_LARGE_NAME = "Fire_03";
	public const string ICEWALL_NAME = "Ice_Wall";
	public const string ICEWALL_SUMMON = "Ice_Effect";
	public const string FIREBALL_RELEASE_NAME = "Fireball";

	//name for fireball depending on charge level
	public const string FIREBALL_SMALL_NAME = "Small";
	public const string FIREBALL_MEDIUM_NAME = "Medium";
	public const string FIREBALL_LARGE_NAME = "Large";

	//game object names
	public const string PLAYER = "player";
	public const string ENEMY = "enemy";
	public const string WINNER_TEXT = "winnerText";

	//scene names
	public const string REPLAY_LEVEL = "replay";
	public const string GAME_LEVEL = "game";
	public const string TUTORIAL_LEVEL = "tutorial";

	//enemy animation names
	public const string CROUCH_ANIM = "crouch";
	public const string IDLE_ANIM = "idle";
	public const string RUN_ANIM = "run";

	//information stored to be used across scenes
	public static string LOSER = "";
}

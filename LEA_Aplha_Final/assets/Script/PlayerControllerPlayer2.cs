using UnityEngine;
using System.Collections;

public class PlayerControllerPlayer2 : MonoBehaviour {
	
	public float gravity = 4;	// Die Gravitation für den Charakter
	public float speed = 10;	// Die Beschleunigung
	public float jumpPower = 10;	//Die Sprungkraft
	public AudioClip attackSound;	//Sound der bei einem Angriff abgespielt wird
	public float boosttime;			//Zeit für den Boost
	public float normalspeed;		//Normale Beschleunigung
	public SendDamageCollider sendDamageColliderR;	//Rechter DamageSender
	public SendDamageCollider sendDamageColliderL;	//Linker DamageSender
	public Transform staminaBar;		//Ausdauer Anzeige
	public Transform healthBar;			//Leben Anzeige
	
	[HideInInspector]
	public bool sceneSwitch = false;
	
	bool lookRight = false;		//Schaut der Charakter nach Rechts?
	public bool isPunching = false;	//Schlägt der Charakter zu?
	public bool isKicking = false;		//Tritt der Charakter zu?
	bool inputJump = false;		//Springt der Charakter?
	float attackingTime = 0.4F;	//Zeit für einen Angriff
	float velocity = 0;			//Die Bewegungsgeschwindigkeit
	Vector3 moveDirection = Vector3.zero;	//Bewegungsrichtung
	CharacterController characterController;	//Unityklasse, Charaktersteuerung
	SpriteController spriteController;		//Animationssteuerung
	HealthController healthController; 		//Lebenssteuerung
	
	// Use this for initialization
	void Start () 
	{
		characterController = GetComponent<CharacterController>();
		spriteController = GetComponent<SpriteController>();
		healthController = GetComponent<HealthController>();
		normalspeed = speed;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.timeScale >0)
		{
		    InputCheck();
			Move();
			BoosterCheck();
			SetAnimation();
			Fight();
		}
		
		staminaBar.gameObject.SendMessage("ApplyStamina",healthController.stamina,SendMessageOptions.DontRequireReceiver);	//Daten für die GUI wird auf die Leiste gesendet
		healthBar.gameObject.SendMessage("ApplyLife",healthController.currentHealth,SendMessageOptions.DontRequireReceiver);	//Daten für die GUI wird auf die Leiste gesendet
	}
	
	//Steuerung für den Charakter
	void InputCheck()
	{
		if(healthController.currentHealth > 0 )
		{
			if(Input.GetAxis("Horizontal2") > 0.5 || Input.GetAxis("Horizontal2") < -0.5)
				velocity = Input.GetAxis("Horizontal2") * speed;
			
			else if(Input.GetAxis("Horizontal2Keyboard") > 0 || Input.GetAxis("Horizontal2Keyboard") < 0)
				velocity = Input.GetAxis("Horizontal2Keyboard") * speed;
			
			else
				velocity = 0;
				
			
			if (velocity > 0)
				lookRight = true;
			
			if (velocity < 0)
				lookRight = false;
			
			if (Input.GetKeyDown(KeyCode.Joystick2Button16) || Input.GetKeyDown(KeyCode.Joystick2Button0) || Input.GetKeyDown (KeyCode.Keypad0))
			{	
				inputJump = true;
								
			}
			else
			{
				inputJump = false;	
			}
			
			if ((Input.GetKeyDown(KeyCode.Joystick2Button17)||Input.GetKeyDown(KeyCode.Joystick2Button1)||Input.GetKeyDown (KeyCode.Keypad4)) &&!isPunching)
			{
				if(healthController.stamina > 0)
				{
					if(healthController.stamina - 12 >= 0)
					{
					
						healthController.stamina -= 12;
						isPunching = true;

						StartCoroutine(ResetIsAttacking());
					}
				}
			}
			if ((Input.GetKeyDown(KeyCode.Joystick2Button2)||Input.GetKeyDown(KeyCode.Joystick2Button18)||Input.GetKeyDown(KeyCode.Keypad6)) && !isKicking)
			{
				if(healthController.stamina > 0)
				{
					if(healthController.stamina - 18 >= 0)
					{
					
						isKicking = true;
						healthController.stamina -= 18;
						StartCoroutine(ResetIsAttacking());
					}
				}
			}
		}

	}
	
	//Bewegung des Charakters
	void Move()
	{
		
		if (characterController.isGrounded)
		{
			moveDirection.y = -1;
			if (inputJump)
			{
			if(healthController.stamina > 0)
				{
					if(healthController.stamina - 15 >= 0)
					{
						moveDirection.y = jumpPower;
						healthController.stamina -= 15;	
					}
				}
			}
			
		}
		else
		{
			moveDirection.y -= gravity;
		}
		moveDirection.x = velocity;
		
		characterController.Move(moveDirection * Time.deltaTime);
	}
	
	//Hier wird überprüft ob angegriffen wird und in welche Richtung angegriffen wird.
	void Fight()
	{
		if(isPunching || isKicking)
		{
			if(lookRight)
			{
				sendDamageColliderR.attacking = true;
				sendDamageColliderL.attacking = false;
			}
			else
			{
				sendDamageColliderR.attacking = false;
				sendDamageColliderL.attacking = true;
			}
		}
		else
		{
			sendDamageColliderR.attacking = false;
			sendDamageColliderL.attacking = false;
		}
	}
	
	
	//Hier werden die Animationen gesetzt
	void SetAnimation()
	{
		//Bewegt sich der Charakter?
		if (velocity > 0)
		{
			spriteController.SetAnimation(SpriteController.AnimationType.goRight);	
		}
		
		if (velocity < 0)
		{
			spriteController.SetAnimation(SpriteController.AnimationType.goLeft);	
		}
		
		//Steht der Charakter?
		if (velocity == 0)
		{
			if (lookRight)
			{
				spriteController.SetAnimation(SpriteController.AnimationType.stayRight);	
			}
			else
			{
				spriteController.SetAnimation(SpriteController.AnimationType.stayLeft);	
			}
		}
		
		//Befindet sich der Charakter auf dem Boden?
		if (!characterController.isGrounded)
		{
			if (lookRight)
			{
				spriteController.SetAnimation(SpriteController.AnimationType.jumpRight);	
			}
			else
			{
				spriteController.SetAnimation(SpriteController.AnimationType.jumpLeft);	
			}	
		}
		
		//Schlägt der Charakter zu?
		if(isPunching)
		{
			if(lookRight)
			{
				spriteController.SetAnimation(SpriteController.AnimationType.punchRight);
			}
			else
			{
				spriteController.SetAnimation(SpriteController.AnimationType.punchLeft);
			}
		}
		
		//Tritt der Charakter zu?
		if(isKicking)
		{
			if(lookRight)
			{
				spriteController.SetAnimation(SpriteController.AnimationType.kickRight);
			}
			else
			{
				spriteController.SetAnimation(SpriteController.AnimationType.kickLeft);
			}
		}
		
		//Ist der Player tot?
		if(healthController.currentHealth <= 0)
		{
			velocity = 0;
			if(lookRight)
			{
				spriteController.SetAnimation(SpriteController.AnimationType.dieRight);	
			}
			else
			{
				spriteController.SetAnimation(SpriteController.AnimationType.dieLeft);
			}
			PlayerPrefs.SetInt("Win",1);	//Hier wird gespeichert welcher Spieler gewonnen hat
			StartCoroutine(GameOver());		//GameOver Wartezeit wird gespeichert	
			
			
		}
	}
	
	//Zeitroutine für einen Angriff
	IEnumerator ResetIsAttacking()
	{
		yield return new WaitForSeconds(attackingTime);
		isPunching = false;
		isKicking = false;
	}
	
	//Wartezeit vor dem GameOver
	IEnumerator GameOver()
	{
		yield return new WaitForSeconds(2.7f);	
		Application.LoadLevel(2);
	}
	
	//Befindet sich der Player in einem Speedboost
	void BoosterCheck()
	{
		boosttime -=Time.deltaTime;
		if(boosttime <= 0)
		{
			speed = normalspeed;
		}
	}
	
	//Boost wird aktiviert
	void ApplySpeed(float speedboost)
	{
		
		speed = speedboost;
		
		boosttime = 5;
	}
	
}

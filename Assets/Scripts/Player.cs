using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    Animator animator;

    bool paused;

    public float MaxSpeed = 6.0f;

    bool grounded = true;

    public Transform groundCheck;

    float groundRadius = 0.2f;

    public LayerMask GroundLayer;
    public LayerMask PlatformLayer;

    public float JumpForce = 700;

    GameManager gameManager;

    public float DeadZone = 0.1f;
    float gravity = 0.0f;

    float playerStartX;
    

    public GameObject JumpLandDustParticles;

    public Transform DustSpawnPoint;

    public AnimationClip Jumping;
    public AnimationClip Running;

    public int SideBlinders;
    public int BodyDoubles;
    public int Disguises;

    public float SideBlinderDuration = 3.0f;
    public float DisguiseDuration = 3.0f;


    public bool SideBlindersActive = false;

    public GameObject BodyDoubleProjectile;

    bool isCorrectingX;
    float timeStartCorrectingX;

    float endDisguise = 0.0f;
    float endSideBlinders = 0.0f;

    public bool DisguiseActive = false;
    void Awake()
    {
        animator = GetComponent<Animator>();

        
        paused = true;

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

	void Start () 
    {
        Messenger.AddListener("StartButtonClicked", UnPause);

        Messenger.AddListener("Pause", Pause);
        rigidbody2D.fixedAngle = false;

        Messenger.AddListener<string>("PowerupGained", GainPowerup);
        gravity = rigidbody2D.gravityScale;

        playerStartX = transform.position.x;

        
	}

    void FixedUpdate()
    {
        if (paused == false)
        {

            // check for grounded
            bool groundedThisFrame = Physics2D.OverlapCircle(groundCheck.position, groundRadius, GroundLayer);
            bool groundedOnPlatform = Physics2D.OverlapCircle(groundCheck.position, groundRadius, PlatformLayer);

            //Debug.Log("grounded this frame: " + groundedThisFrame);
            //Debug.Log("platform grounded: " + groundedOnPlatform);

            if (grounded == false && groundedThisFrame == true)
            {
                

                // spawn landing particle system
                Debug.Log("landed");
                Instantiate(JumpLandDustParticles, DustSpawnPoint.position, Quaternion.Euler(-90, 0, 0));

                animator.SetBool("jumping", false);


            }

            if (grounded == false && groundedOnPlatform == true && rigidbody2D.velocity.y < 0)
            {
                // spawn landing particle system
                Debug.Log("landed on platform");
                Instantiate(JumpLandDustParticles, DustSpawnPoint.position, Quaternion.Euler(-90, 0, 0));

                animator.SetBool("jumping", false);
            }

            if (grounded == true && groundedThisFrame == false)
            {
                animator.SetBool("jumping", true);

            }



            grounded = groundedThisFrame;

            if (groundedOnPlatform == true && rigidbody2D.velocity.y <= 0)
            {
                animator.SetBool("jumping", false);
                grounded = true;
            }
            //set ground in our Animator to match grounded
            //anim.SetBool("Ground", grounded);
        }

        transform.rotation = Quaternion.identity;

    }
	
	void Update () 
    {
        if (paused == true)
        {
            
            if(animator.enabled == true)
            {
                animator.enabled = false;
            }
            
            
        }

        if (paused == false)
        {
            
            if (animator.enabled == false)
            {
                animator.enabled = true;
            }
            
            //if we are on the ground and the space bar was pressed, change our ground state and add an upward force
            if (Input.GetKeyDown(KeyCode.Space) && grounded == true)
            {
                //anim.SetBool("Ground", false);
                rigidbody2D.AddForce(new Vector2(0, JumpForce));
            }


            if (animator.GetCurrentAnimatorStateInfo(0).IsName("RahulHairFlip"))
            {
                animator.speed = 1.0f;
            }

            else if (grounded == true)
            {
                animator.speed = gameManager.PlayerSpeedMultiplier;
            }

            else
            {
                animator.speed = 1.0f;
            }

            if (DisguiseActive == true)
            {
                if (Time.time >= endDisguise)
                {
                    // deactivate disguise

                    DisguiseActive = false;
                    Debug.Log("disguise deactive");
                    animator.SetBool("disguise", false);
                    gameManager.DisguiseLevel = 0.0f;
                }
            }

            if (SideBlindersActive == true)
            {
                if (Time.time >= endSideBlinders)
                {
                    // deactivate disguise

                    SideBlindersActive = false;
                    Debug.Log("side blinders deactive");
                    animator.SetBool("sideBlinders", false);
                }
            }
        }

        if (isCorrectingX == false && Mathf.Abs(playerStartX - transform.position.x) > 1.5f)
        {

            isCorrectingX = true;
            timeStartCorrectingX = Time.time;
            StartCoroutine(correctX());
        }
	}

    IEnumerator correctX()
    {
        Debug.Log("correcting X position...");
        Debug.Log("need to correct " + Mathf.Abs(playerStartX - transform.position.x) + " units");
        Debug.Log("StartX: " + playerStartX);

       

        while (Mathf.Abs(playerStartX - transform.position.x) > 0.1f)
        {
            Debug.Log("correcting");
            transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, playerStartX, 0.02f), transform.position.y, transform.position.z);

            yield return 0;
        }

        yield return new WaitForSeconds(1.0f);
        isCorrectingX = false;

        Debug.Log("correcting done");
        yield return null;
    }

    void UnPause()
    {
        paused = false;

        rigidbody2D.isKinematic = false;
    }

    void Pause()
    {
        paused = true;

        rigidbody2D.isKinematic = true;
    }

    void GainPowerup(string powerup)
    {
        if (powerup == "BodyDouble")
        {
            BodyDoubles += 1;
            Instantiate(BodyDoubleProjectile, transform.position, Quaternion.identity);
        }

        if (powerup == "Disguise")
        {
            UseDisguise();
        }

        if(powerup == "SideBlinders")
        {
            UseSideBlinders();
        }
    }

    public void UseDisguise()
    {
        Debug.Log("disguise");

        if (endDisguise <= Time.time)
        {
            // disguise not enabled
            Debug.Log("first disguise");
            endDisguise = Time.time + DisguiseDuration;
        }

        else 
        {
            // disguise already enabled, add time to it
            endDisguise = endDisguise + DisguiseDuration;
        }

        DisguiseActive = true;
        SideBlindersActive = false;
        endSideBlinders = 0.0f;

        animator.SetBool("disguise", true);
        animator.SetBool("sideBlinders", false);
        gameManager.DisguiseLevel = 0.5f;
    }

    public void UseSideBlinders()
    {
        /*
        if(SideBlinders >0 && SideBlindersActive == false)
        {
            SideBlindersActive = true;
            SideBlinders -= 1;
            Debug.Log("Side blinders used!");
            StartCoroutine(SideBlinderActivate());
        }
         */

        Debug.Log("sideblinders");

        if (endSideBlinders <= Time.time)
        {
            // side blinders not enabled
            Debug.Log("first side blinders");
            endSideBlinders = Time.time + SideBlinderDuration;
        }

        else
        {
            // side blinders already enabled, add time to it
            endSideBlinders = endSideBlinders + SideBlinderDuration;
        }


        SideBlindersActive = true;
        DisguiseActive = false;
        endDisguise = 0.0f;
        gameManager.DisguiseLevel = 0.0f;

        animator.SetBool("disguise", false);
        animator.SetBool("sideBlinders", true); 
    }
    IEnumerator SideBlinderActivate()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(1f, 1f, 1f, .5f);

        yield return new WaitForSeconds(SideBlinderDuration  - 1.0f);

        // flash warning time - final second
        sprite.color = new Color(1f, 1f, 1f, 1.0f);
        yield return new WaitForSeconds(0.25f);
        sprite.color = new Color(1f, 1f, 1f, .5f);
        yield return new WaitForSeconds(0.25f);
        sprite.color = new Color(1f, 1f, 1f, 1.0f);
        yield return new WaitForSeconds(0.25f);
        sprite.color = new Color(1f, 1f, 1f, .5f);
        yield return new WaitForSeconds(0.25f);
        sprite.color = new Color(1f, 1f, 1f, 1.0f);

        SideBlindersActive = false;

        yield return null;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Platform")
        {
            float maxDistance = (col.transform.localScale.y + transform.localScale.y) / 2.0f;
            if (rigidbody2D.velocity.y < 0 &&
                transform.position.y - col.transform.position.y - rigidbody2D.velocity.y * Time.fixedDeltaTime > maxDistance - DeadZone)
            {
                rigidbody2D.gravityScale = 0;
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
                //animator.SetBool("jumping", false);
                //grounded = true;
                //transform.position = new Vector3(transform.position.x, col.transform.position.y + maxDistance, transform.position.z);
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Platform")
        {
            rigidbody2D.gravityScale = gravity;
        }
    }



    
}

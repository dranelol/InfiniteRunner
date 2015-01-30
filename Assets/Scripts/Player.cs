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
    

    

    public GameObject JumpLandDustParticles;

    public Transform DustSpawnPoint;

    public AnimationClip Jumping;
    public AnimationClip Running;

    public int SideBlinders;
    public int BodyDoubles;
    public int Disguises;

    public float SideBlinderDuration = 3.0f;


    public bool SideBlindersActive = false;

    public GameObject BodyDoubleProjectile;
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
	}

    void FixedUpdate()
    {
        if (paused == false)
        {

            // check for grounded
            bool groundedThisFrame = Physics2D.OverlapCircle(groundCheck.position, groundRadius, GroundLayer);
            bool groundedOnPlatform = Physics2D.OverlapCircle(groundCheck.position, groundRadius, PlatformLayer);

            Debug.Log("grounded this frame: " + groundedThisFrame);
            Debug.Log("platform grounded: " + groundedOnPlatform);

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
        }

        
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
            Disguises += 1;
        }

        if(powerup == "SideBlinders")
        {
            SideBlinders += 1;
        }
    }

    public void UseSideBlinders()
    {
        if(SideBlinders >0 && SideBlindersActive == false)
        {
            SideBlindersActive = true;
            SideBlinders -= 1;
            Debug.Log("Side blinders used!");
            StartCoroutine(SideBlinderActivate());
        }
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

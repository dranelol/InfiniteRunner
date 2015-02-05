using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{

    public float PlayerSpeedMultiplier = 4.0f;

    /// <summary>
    ///  time, in seconds, it will take for the player to reach the next speed lerp goal
    /// </summary>
    public float PlayerSpeedLerpTime = 4.0f;
    public float CrowdSpeedMultiplier = 3.0f;

    public float CrowdSpeedLerpTime = 2.0f;

    public float PlayerSpeedMax = 10.0f;
    public float CrowdSpeedMax = 9.0f;

    public float InitialCrowdSpeedmax = 9.0f;
    
    bool paused = true;

    bool playerLerping;
    float playerStartLerpTime;
    float playerSpeedLerpStart;
    float playerSpeedLerpGoal;

    public bool crowdLerping;
    float crowdStartLerpTime;
    float crowdSpeedLerpStart;
    float crowdSpeedLerpGoal;

    public GUIText SpeedMultiplierText;
    public GUIText CrowdSpeedMultiplierText;
    public GUIText ScoreText;
    public GUIText CrowdDistance; 

    public GameObject RestartButton;


    public int Score;

    public float BodyDoubleDuration = 3.0f;
    public float DisguiseLevel = 0.0f;
    public bool BodyDoubleUsed = false;

    public List<GameObject> Powerups = new List<GameObject>();
    public List<GameObject> Obstacles = new List<GameObject>();


    Player player;
    GameObject crowd;

    void Awake()
    {


        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        crowd = GameObject.FindGameObjectWithTag("Crowd");
        

    }

	void Start () 
    {
        Messenger.AddListener("StartButtonClicked", UnPause);
        Messenger.AddListener<float>("SpeedChanged", SpeedChange);
        Messenger.AddListener<int>("ScoreChanged", ScoreChange);
        Messenger.AddListener("CrowdCatch", Lose);
        Messenger.AddListener("Pause", Pause);
        Messenger.AddListener("BodyDoubleHitCrowd", UseBodyDouble);

        playerSpeedLerpGoal = 2.0f;

        crowdSpeedLerpGoal = 1.5f;
	}


    void FixedUpdate()
    {
        // speed multiplier starts default at 1; lerps towards 6 over time without any interaction with the world

        // speed goes up by 1 every second; 5 seconds for full 6 speed

        if(paused == false)
        {
            // ACCELERATE PLAYER
            if(PlayerSpeedMultiplier < PlayerSpeedMax)
            {
                if (playerLerping == false)
                {
                    playerLerping = true;
                    PlayerSpeedLerpTime = 2.0f;
                    playerStartLerpTime = Time.time;
                    playerSpeedLerpStart = PlayerSpeedMultiplier;
                    playerSpeedLerpGoal = Mathf.Min(PlayerSpeedMultiplier + 1.0f, PlayerSpeedMax);

                    StartCoroutine(PlayerLerpSpeed());
                }
            }

            if (BodyDoubleUsed == false)
            {
                // DECELERATE CROWD

                // if crowd is faster than player by greater than 1.0
                if (CrowdSpeedMultiplier > PlayerSpeedMultiplier && CrowdSpeedMultiplier - PlayerSpeedMultiplier > 1.0f)
                {
                    if (crowdLerping == false)
                    {
                        crowdLerping = true;
                        CrowdSpeedLerpTime = 2.0f;
                        crowdStartLerpTime = Time.time;
                        crowdSpeedLerpStart = CrowdSpeedMultiplier;

                        if (DisguiseLevel > 0.0f)
                        {
                            crowdSpeedLerpGoal = Mathf.Min(PlayerSpeedMultiplier - DisguiseLevel, CrowdSpeedMax);
                        }

                        else
                        {
                            crowdSpeedLerpGoal = Mathf.Min(PlayerSpeedMultiplier + 1.0f, CrowdSpeedMax);
                        }

                        StartCoroutine(CrowdLerpSpeed());
                    }

                }


                // if crowd is faster than player by less than or equal to 1.0
                else if (CrowdSpeedMultiplier > PlayerSpeedMultiplier)
                {
                    if (crowdLerping == false)
                    {

                        if (DisguiseLevel > 0.0f)
                        {
                            crowdLerping = true;
                            CrowdSpeedLerpTime = 2.0f;
                            crowdStartLerpTime = Time.time;
                            crowdSpeedLerpStart = CrowdSpeedMultiplier;
                            crowdSpeedLerpGoal = Mathf.Min(PlayerSpeedMultiplier - DisguiseLevel, CrowdSpeedMax);

                            StartCoroutine(CrowdLerpSpeed());
                        }
                    }
                }

                // decelerate over maximum
                if (CrowdSpeedMultiplier > CrowdSpeedMax)
                {
                    if (crowdLerping == false)
                    {
                        crowdLerping = true;
                        CrowdSpeedLerpTime = 2.0f;
                        crowdStartLerpTime = Time.time;
                        crowdSpeedLerpStart = CrowdSpeedMultiplier;
                        crowdSpeedLerpGoal = CrowdSpeedMax;

                        StartCoroutine(CrowdLerpSpeed());
                    }
                }

                // ACCELERATE CROWD

                if (CrowdSpeedMultiplier < CrowdSpeedMax)
                {
                    if (crowdLerping == false)
                    {
                        crowdLerping = true;
                        CrowdSpeedLerpTime = 2.0f;
                        crowdStartLerpTime = Time.time;
                        crowdSpeedLerpStart = CrowdSpeedMultiplier;





                        crowdSpeedLerpGoal = Mathf.Min(CrowdSpeedMultiplier + 0.5f, CrowdSpeedMax);


                        StartCoroutine(CrowdLerpSpeed());
                    }
                }
            }

            
        }

        CrowdSpeedMax = InitialCrowdSpeedmax - DisguiseLevel;
    }

    void Update()
    {

    }

    IEnumerator PlayerLerpSpeed()
    {
        //Debug.Log("lerp speed goal: " + playerSpeedLerpGoal);
        //Debug.Log("current speed at start of lerp: " + PlayerSpeedMultiplier);
        // lerp till they're close enough
        while(Mathf.Abs(PlayerSpeedMultiplier - playerSpeedLerpGoal) >= 0.05f)
        {
            float timeSinceStart = Time.time - playerStartLerpTime;

            float percentDone = timeSinceStart / PlayerSpeedLerpTime;

            PlayerSpeedMultiplier = Mathf.Lerp(playerSpeedLerpStart, playerSpeedLerpGoal, percentDone);
            yield return 0;
        }
        //normalize
        //PlayerSpeedMultiplier = speedLerpGoal;
        playerLerping = false;
        yield return null;
    }

    IEnumerator CrowdLerpSpeed()
    {
        //Debug.Log("lerp speed goal: " + crowdSpeedLerpGoal);
        // lerp till they're close enough
        while (Mathf.Abs(CrowdSpeedMultiplier - crowdSpeedLerpGoal) >= 0.05f && BodyDoubleUsed == false)
        {
            float timeSinceStart = Time.time - crowdStartLerpTime;

            float percentDone = timeSinceStart / CrowdSpeedLerpTime;

            CrowdSpeedMultiplier = Mathf.Lerp(crowdSpeedLerpStart, crowdSpeedLerpGoal, percentDone);
            yield return 0;
        }
        //normalize
        //PlayerSpeedMultiplier = speedLerpGoal;
        crowdLerping = false;
        yield return null;
    }


    void SpeedChange(float percentageChange)
    {
        StopCoroutine(PlayerLerpSpeed());
        //StopAllCoroutines();

        playerLerping = true;
        PlayerSpeedLerpTime = 0.5f;
        playerStartLerpTime = Time.time;
        playerSpeedLerpStart = PlayerSpeedMultiplier;
        playerSpeedLerpGoal = PlayerSpeedMultiplier * percentageChange;
        
        

        
        StartCoroutine(PlayerLerpSpeed());
    }

    void OnGUI()
    {
        SpeedMultiplierText.text = "Speed Multiplier: " + PlayerSpeedMultiplier.ToString();
        CrowdSpeedMultiplierText.text = "Crowd Speed Multiplier: " + CrowdSpeedMultiplier.ToString();

        ScoreText.text = "Score: " + Score.ToString();

        float distance = Vector3.Distance(player.transform.position, crowd.transform.position) - 1.0f;

        CrowdDistance.text = "Crowd Distance: " + distance.ToString("F2") + "m";
    }


    void ScoreChange(int delta)
    {
        Score += delta;
    }

    void Pause()
    {
        
        paused = true;

    }

    void UnPause()
    {
        paused = false;
    }

    void Lose()
    {
        StopAllCoroutines();

        RestartButton.GetComponent<UISprite>().enabled = true;
        RestartButton.GetComponent<BoxCollider>().enabled = true;
        RestartButton.GetComponentInChildren<UILabel>().enabled = true;

    }

    public void Restart()
    {
        RestartButton.GetComponent<UISprite>().enabled = false;
        RestartButton.GetComponent<BoxCollider>().enabled = false;
        RestartButton.GetComponentInChildren<UILabel>().enabled = false;

        Application.LoadLevel(Application.loadedLevel);
    }


    public void UseBodyDouble()
    {
        StartCoroutine(BodyDouble());
    }

    IEnumerator BodyDouble()
    {
        //StopCoroutine(CrowdLerpSpeed());
        //StopAllCoroutines();
        float previousCrowdSpeed = CrowdSpeedMultiplier;
        BodyDoubleUsed = true;
        CrowdSpeedMultiplier = 0.0f;
        Debug.Log("crowd waiting...");
        yield return new WaitForSeconds(BodyDoubleDuration);
        Debug.Log("crowd done waiting");
        CrowdSpeedMultiplier = previousCrowdSpeed;
        BodyDoubleUsed = false;
        Messenger.Broadcast("BodyDoubleDone");
        yield return null;
    }

    public void SpawnObject(GameObject orig, bool isPowerup)
    {
        if (isPowerup == true)
        {
            // spawn random powerup
            GameObject powerup = (GameObject)Instantiate(Powerups[Random.Range(0, Powerups.Count)], orig.transform.localPosition, Quaternion.identity);

            powerup.transform.parent = orig.transform;
            powerup.transform.position = new Vector3(orig.transform.position.x, powerup.transform.position.y, powerup.transform.position.z);
            //powerup.transform.position = orig.transform.position;
            //powerup.transform.position = new Vector3(0, orig.transform.position.y, orig.transform.position.z);
            
        }

        else
        {
            float rng = Random.value;

            // 30 / 70 split; pu / obs

            if(rng >= 0.0f && rng <= 0.3f)
            {
                GameObject powerup = (GameObject)Instantiate(Powerups[Random.Range(0, Powerups.Count)], orig.transform.localPosition, Quaternion.identity);
                powerup.transform.parent = orig.transform;
                powerup.transform.position = new Vector3(orig.transform.position.x, powerup.transform.position.y, powerup.transform.position.z);
                //powerup.transform.position = new Vector3(0, orig.transform.position.y, orig.transform.position.z);
            }

            else
            {
                GameObject obstacle = (GameObject)Instantiate(Obstacles[Random.Range(0, Obstacles.Count)], orig.transform.localPosition, Quaternion.identity);
                obstacle.transform.parent = orig.transform;
                obstacle.transform.position = new Vector3(orig.transform.position.x, obstacle.transform.position.y, obstacle.transform.position.z);
                //obstacle.transform.position = new Vector3(0, orig.transform.position.y, orig.transform.position.z);
            }
        }
    }
}

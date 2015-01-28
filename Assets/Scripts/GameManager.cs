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

    public GameObject bodyDoubleButton;
    public GameObject sideBlinderButton;
    public GameObject disguiseButton;

    public int Score;

    public float BodyDoubleDuration = 3.0f;
    public float DisguiseLevel = 0.0f;
    public bool BodyDoubleUsed = false;

    public List<GameObject> Powerups = new List<GameObject>();
    public List<GameObject> Obstacles = new List<GameObject>();


    Player player;
    GameObject crowd;
    Player playerScript;

    void Awake()
    {


        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        crowd = GameObject.FindGameObjectWithTag("Crowd");
        

        playerScript = player.GetComponent<Player>();
    }

	void Start () 
    {
        Messenger.AddListener("StartButtonClicked", UnPause);
        Messenger.AddListener<float>("SpeedChanged", SpeedChange);
        Messenger.AddListener<int>("ScoreChanged", ScoreChange);
        Messenger.AddListener("CrowdCatch", Lose);
        Messenger.AddListener("Pause", Pause);

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

        bodyDoubleButton.GetComponent<UISprite>().enabled = false;
        bodyDoubleButton.GetComponent<BoxCollider>().enabled = false;
        bodyDoubleButton.GetComponentInChildren<UILabel>().enabled = false;

        disguiseButton.GetComponent<UISprite>().enabled = false;
        disguiseButton.GetComponent<BoxCollider>().enabled = false;
        disguiseButton.GetComponentInChildren<UILabel>().enabled = false;

        sideBlinderButton.GetComponent<UISprite>().enabled = false;
        sideBlinderButton.GetComponent<BoxCollider>().enabled = false;
        sideBlinderButton.GetComponentInChildren<UILabel>().enabled = false;
    }

    void UnPause()
    {
        paused = false;

        bodyDoubleButton.GetComponent<UISprite>().enabled = true;
        bodyDoubleButton.GetComponent<BoxCollider>().enabled = true;
        bodyDoubleButton.GetComponentInChildren<UILabel>().enabled = true;

        disguiseButton.GetComponent<UISprite>().enabled = true;
        disguiseButton.GetComponent<BoxCollider>().enabled = true;
        disguiseButton.GetComponentInChildren<UILabel>().enabled = true;

        sideBlinderButton.GetComponent<UISprite>().enabled = true;
        sideBlinderButton.GetComponent<BoxCollider>().enabled = true;
        sideBlinderButton.GetComponentInChildren<UILabel>().enabled = true;
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

    public void UseDisguise()
    {
        if(player.Disguises > 0)
        {
            player.Disguises -= 1;
            DisguiseLevel = 0.5f;
        }
    }

    public void UseBodyDouble()
    {
        if (player.BodyDoubles > 0 && BodyDoubleUsed == false)
        {
            player.BodyDoubles -= 1;
            BodyDoubleUsed = true;
            Messenger.Broadcast("BodyDoubleUsed");
            StartCoroutine(BodyDouble());
            
        }
    }

    IEnumerator BodyDouble()
    {
        StopCoroutine(CrowdLerpSpeed());
        float previousCrowdSpeed = CrowdSpeedMultiplier;

        CrowdSpeedMultiplier = 0.0f;

        yield return new WaitForSeconds(BodyDoubleDuration);

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
            Debug.Log("generating powerup");
            GameObject powerup = (GameObject)Instantiate(Powerups[Random.Range(0, Powerups.Count)], orig.transform.localPosition, Quaternion.identity);

            powerup.transform.position = orig.transform.position;
            powerup.transform.parent = orig.transform;
        }

        else
        {
            float rng = Random.value;

            // 30 / 70 split; pu / obs

            if(rng >= 0.0f && rng <= 0.3f)
            {
                Debug.Log("generating powerup");
                GameObject powerup = (GameObject)Instantiate(Powerups[Random.Range(0, Powerups.Count)], orig.transform.localPosition, Quaternion.identity);
                powerup.transform.parent = orig.transform;
            }

            else
            {
                Debug.Log("generating obstacle");
                GameObject obstacle = (GameObject)Instantiate(Obstacles[Random.Range(0, Obstacles.Count)], orig.transform.localPosition, Quaternion.identity);
                obstacle.transform.parent = orig.transform;
            }
        }
    }
}

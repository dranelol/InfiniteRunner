using UnityEngine;
using System.Collections;

public class Crowd : MonoBehaviour 
{
    GameManager gameManager;
	// Use this for initialization
    bool paused = true;

    bool bodyDoubleActive = false;

    Animator anim;

    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        
    }
	void Start ()
    {
        Messenger.AddListener("StartButtonClicked", UnPause);
        Messenger.AddListener("Pause", Pause);
        Messenger.AddListener("BodyDoubleUsed", BodyDoubleActive);
        Messenger.AddListener("BodyDoubleDone", BodyDoubleDeactivate);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (paused == false)
        {
            // get difference between player and crowd velocity

            float velocityDiff = 0.0f;
            if (bodyDoubleActive == false)
            {
                velocityDiff = gameManager.PlayerSpeedMultiplier - gameManager.CrowdSpeedMultiplier;
            }

            else
            {
                velocityDiff = gameManager.PlayerSpeedMultiplier;
            }

            transform.position -= Vector3.right * velocityDiff * Time.deltaTime;

            anim.speed = gameManager.CrowdSpeedMultiplier;
        }

        else
        {
            anim.speed = 0.0f;
        }

	}

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.tag == "Player")
        {
            Debug.Log("crowd gotcha");
            Messenger.Broadcast("CrowdCatch");
            Messenger.Broadcast("Pause");
        }

    }

    void Pause()
    {
        paused = true;
    }

    void UnPause()
    {
        paused = false;
    }

    void BodyDoubleActive()
    {
        bodyDoubleActive = true;
    }

    void BodyDoubleDeactivate()
    {
        bodyDoubleActive = false;
    }
}

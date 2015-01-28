using UnityEngine;
using System.Collections;

public class MoveParticles : MonoBehaviour 
{
    GameManager gameManager;

    float MoveSpeed = 1.0f;

    float SpeedMultiplier = 1.0f;

    bool paused = false;

    void Awake()
    {
        Messenger.AddListener("Pause", Pause);
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

	void Start () 
    {
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (paused == false)
        {
            transform.position -= Vector3.right * MoveSpeed * SpeedMultiplier * Time.deltaTime * gameManager.PlayerSpeedMultiplier;
        }
	}

    void Pause()
    {
        Debug.Log("particles paused");
        paused = true;
    }

    void UnPause()
    {
        paused = false;
    }

}

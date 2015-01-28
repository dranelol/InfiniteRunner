using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour 
{
    public int ScoreIncrease;

    bool paused = true;
	void Awake()
    {
        
    }
	void Start () 
    {
        Messenger.AddListener("Pause", Pause);
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("coin hit");
            Messenger.Broadcast("ScoreChanged", ScoreIncrease);
        }

        gameObject.SetActive(false);
    }

    void Pause()
    {
        paused = true;
        GetComponent<Animator>().speed = 0;
    }

    void UnPause()
    {
        paused = false;
        GetComponent<Animator>().speed = 1;
    }
}

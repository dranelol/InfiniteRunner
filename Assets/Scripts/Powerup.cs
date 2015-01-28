using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour 
{
    public string PowerupType;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.tag == "Player")
        {
            Debug.Log(PowerupType + " hit");
            Messenger.Broadcast("PowerupGained", PowerupType);
        }

        gameObject.SetActive(false);
    }
}

using UnityEngine;
using System.Collections;

public class SlowObstacle : MonoBehaviour {

    public float PercentageSlow;

	void Start () 
    {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.tag == "Player")
        {
            Debug.Log("rock hit");
            Messenger.Broadcast("SpeedChanged", PercentageSlow);
        }

        gameObject.SetActive(false);
    }

}

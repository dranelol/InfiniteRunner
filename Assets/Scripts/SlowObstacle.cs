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
            Debug.Log(col.gameObject.name);

            Player playerScript0 = col.gameObject.GetComponent<Player>();

            Player playerScript1;

            if (playerScript0 == null)
            {
                playerScript1 = col.transform.parent.gameObject.GetComponent<Player>();

                if (playerScript1.SideBlindersActive == false)
                {
                    Messenger.Broadcast("SpeedChanged", PercentageSlow);
                    Debug.Log("obstacle hit");
                    gameObject.SetActive(false);
                }
            }

            else
            {
                if (playerScript0.SideBlindersActive == false)
                {
                    Messenger.Broadcast("SpeedChanged", PercentageSlow);
                    Debug.Log("obstacle hit");
                    gameObject.SetActive(false);
                }
            }


            
        }

        
    }

}

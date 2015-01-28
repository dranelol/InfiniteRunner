using UnityEngine;
using System.Collections;

public class SpikeDeath : MonoBehaviour {

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
            // yea whatever
            Debug.Log("spikedeath");
            Messenger.Broadcast("CrowdCatch");
            Messenger.Broadcast("Pause");
        }

    }
}

using UnityEngine;
using System.Collections;

public class BodyDouble : MonoBehaviour 
{
    GameObject crowd;

    public float MoveSpeed;
    void Awake()
    {
        crowd = GameObject.FindGameObjectWithTag("Crowd");
    }
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        transform.position = Vector3.MoveTowards(transform.position, crowd.transform.position, MoveSpeed * Time.deltaTime);

        if (crowd.renderer.bounds.Intersects(renderer.bounds))
        {
            Debug.Log("crowd hit");
            Messenger.Broadcast("BodyDoubleHitCrowd");
            gameObject.SetActive(false);
        }

	}

    void OnTriggerEnter2D(Collider2D col)
    {
        
        if (col.gameObject.tag == "Crowd")
        {
            Debug.Log("crowd hit");
            Messenger.Broadcast("BodyDoubleHitCrowd");
            gameObject.SetActive(false);
        }

        
    }
}

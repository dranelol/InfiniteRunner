using UnityEngine;
using System.Collections;

public class Mirror : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.tag == "Player")
        {
            if(col.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("RahulWalk 0"))
            {
                col.gameObject.GetComponent<Animator>().SetTrigger("hairFlip");
            }
        }

        gameObject.SetActive(false);
    }
}

using UnityEngine;
using System.Collections;

public class DisplayPowerupAmt : MonoBehaviour 
{
    UILabel label;

    public string PowerupType;

    Player player;

    void Awake()
    {
        label = GetComponentInChildren<UILabel>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (PowerupType == "BodyDouble")
        {
            label.text = "Body Doubles: " + player.BodyDoubles;
        }

        if (PowerupType == "SideBlinders")
        {
            label.text = "Side Blinders: " + player.SideBlinders;
        }

        if (PowerupType == "Disguise")
        {
            label.text = "Disguises: " + player.Disguises;
        }

	}
}

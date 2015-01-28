using UnityEngine;
using System.Collections;

public class UIActionHandler : MonoBehaviour 
{
    /// <summary>
    /// Whether or not this button should be usable by the player
    /// </summary>
    public bool Enabled;

    /// <summary>
    /// Whether or not this button should react to click events
    /// </summary>
    public bool Interactable;

    public string ObjectName;

	void Start () 
    {
	    
	}
	
	void Update () 
    {

	
	}

    public void BroadcastClick()
    {
        if (Interactable == true && Enabled == true)
        {
            Messenger.Broadcast(ObjectName + "Clicked");
        }
    }

    public void EnableInteraction()
    {
        Interactable = true;
    }

    public void DisableInteraction()
    {
        Interactable = false;
    }

    public void ToggleInteraction()
    {
        Interactable = !Interactable;
        //Debug.Log(gameObject.name + " interaction changed to " + Interactable);
    }

    public void Disable()
    {
        Enabled = false;
        GetComponent<UILabel>().color = Color.black;
    }

    public void Enable()
    {
        Enabled = true;
        GetComponent<UILabel>().color = Color.white;
    }

    public void StartGame()
    {
        Debug.Log("start game");
        Messenger.Broadcast("StartGenerating");
    }

    

}

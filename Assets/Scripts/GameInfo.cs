using UnityEngine;
using System.Collections;

public class GameInfo : MonoBehaviour 
{
    public bool Activated;

    TweenScale tweenScale;
    TweenAlpha tweenAlpha;

    void Awake()
    {
        tweenScale = GetComponent<TweenScale>();
        tweenAlpha = GetComponent<TweenAlpha>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GameInfoClicked()
    {
        if (Activated == true)
        {
            Deactivate();
        }

        else
        {
            Activate();
        }
    }

    public void Activate()
    {
        Activated = true;

        tweenScale.PlayForward();
        tweenAlpha.PlayForward();
    }

    public void Deactivate()
    {
        Activated = false;

        tweenScale.PlayReverse();
        tweenAlpha.PlayReverse();
    }
}

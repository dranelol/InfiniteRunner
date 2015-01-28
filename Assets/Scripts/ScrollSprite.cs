using UnityEngine;
using System.Collections;

public class ScrollSprite : MonoBehaviour 
{
    public float ScrollSpeed;

    public float ScrollMultiplier;

    Vector2 offset;

    bool paused;

    Renderer renderer;

    GameManager gameManager;

    void Awake()
    {
        renderer = GetComponent<Renderer>();

        

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

	void Start () 
    {
        Messenger.AddListener("StartButtonClicked", UnPause);
        Messenger.AddListener("Pause", Pause);
        paused = true;
	}
	
	void Update () 
    {
        if (paused == false)
        {
            // 1.0 speed gives a full texture scroll every second
            offset = renderer.material.mainTextureOffset;
            offset.x = offset.x + (ScrollSpeed * Time.deltaTime * ScrollMultiplier * gameManager.PlayerSpeedMultiplier);
            renderer.material.mainTextureOffset = offset;

        }
	}

    void UnPause()
    {
        paused = false;
    }

    void Pause()
    {
        paused = true;
    }
}

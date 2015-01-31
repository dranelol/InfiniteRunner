using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteGroupData : MonoBehaviour 
{
    int activeSprites = 0;
    int inactiveSprites = 0;

    public int GroupTileWidth;
    public int GroupTileSize;


    List<GameObject> sprites = new List<GameObject>();

    public List<GameObject> ObjectsInGroup = new List<GameObject>();

    GameManager gameManager;

    void Awake()
    {
        Messenger.AddListener("GroupActivated" + gameObject.name, GroupActivated);

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

    }
	void Start () 
    {
        inactiveSprites = transform.childCount;

        foreach(Transform sprite in transform)
        {
            sprites.Add(sprite.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void GroupActivated()
    {
        activeSprites = transform.childCount;
        inactiveSprites = 0;

    }

    public void SpawnObjects()
    {
        foreach(GameObject obj in ObjectsInGroup)
        {
            if (obj.name == "Obstacle")
            {
                gameManager.SpawnObject(obj, false);
            }

            else
            {
                gameManager.SpawnObject(obj, true);
            }
        }
    }

    
}

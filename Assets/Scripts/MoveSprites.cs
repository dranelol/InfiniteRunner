using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveSprites : MonoBehaviour 
{
    public List<GameObject> ActiveGroups = new List<GameObject>();

    public List<GameObject> InactiveGroups = new List<GameObject>();

    public List<GameObject> StartActiveGroups = new List<GameObject>();

    bool paused;

    bool generating;

    public float MoveSpeed;

    public float SpeedMultiplier = 1.0f;

    /// <summary>
    ///  base time, in seconds, in between generating new element groups
    /// </summary>
    public float GenerateDelayBase;

    /// <summary>
    /// maximum random time to add to base time in between generating new element groups
    /// </summary>
    public float GenerateDelayRandomMax;

    /// <summary>
    /// multiplier to increase/decrease generation speed
    /// </summary>
    public float GenerateMultiplier = 1.0f;

    float generateDelay;

    public Transform LeftBoundary;

    public Transform ResetPosition;

    List<GameObject> toDeactivate = new List<GameObject>();

    GameManager gameManager;

    GameObject player;

    float screenWidth;

    public bool CollisionGroup = false;

    public bool StartActive = false;

    public bool ReactivateElements = false;

    bool firstGenerate = false;


    void Awake()
    {
        paused = true;

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
	void Start () 
    {
        Messenger.AddListener("StartButtonClicked", UnPause);
        Messenger.AddListener<GameObject>("DeactivateGroup", Deactivate);
        Messenger.AddListener("StartGenerating", StartGenerating);
        Messenger.AddListener("Pause", Pause);

        // add children to lists
        Debug.Log("starting");
        foreach (Transform child in transform)
        {

            if (StartActive == false)
            {
                child.gameObject.SetActive(false);
                InactiveGroups.Add(child.gameObject);
            }

            else
            {
                if (StartActiveGroups.Contains(child.gameObject))
                {
                    ActiveGroups.Add(child.gameObject);
                }

                else
                {
                    child.gameObject.SetActive(false);
                    InactiveGroups.Add(child.gameObject);
                }
            }

            

            
            //child.position = ResetPosition.position;
        }

        // start generation loop

        generateDelay = GenerateDelayBase + Random.Range(0, GenerateDelayRandomMax);

        float height = 2.0f * Camera.main.orthographicSize;
        screenWidth = height * Camera.main.aspect;
	}

    void FixedUpdate()
    {
        if (paused == false && generating == true)
        {
            GenerateGroup();
        }
    }
	
	void Update () 
    {
        if (paused == false)
        {
            // check for deactive group

            if (toDeactivate.Count > 0)
            {
                foreach(GameObject group in toDeactivate)
                {
                    ActiveGroups.Remove(group);

                    if (group.name != "ObstacleGroup1")
                    {
                        InactiveGroups.Add(group);
                    }

                    
                }

                toDeactivate.Clear();
                
            }
            // move all active groups
            foreach (GameObject spriteGroup in ActiveGroups)
            {
                spriteGroup.transform.position -= Vector3.right * MoveSpeed * SpeedMultiplier * Time.deltaTime * gameManager.PlayerSpeedMultiplier;
                
                /*
                if (spriteGroup.transform.position.x < LeftBoundary.position.x && spriteGroup.active == true)
                {
                    Deactivate(spriteGroup);
                }
                 */
            } 
        }
	}


    void Pause()
    {
        paused = true;
    }

    void UnPause()
    {
        paused = false;
    }

    void Deactivate(GameObject group)
    {
        if(ReactivateElements == true)
        {
            foreach (Transform child in group.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        group.SetActive(false);
        group.transform.position = new Vector3(ResetPosition.position.x, group.transform.position.y, group.transform.position.z);
        //sprite.transform.position = ResetPosition.position;
        //Debug.Log("deactivating element group: " + group.name);
        
        // move group to deactivate on next update
        toDeactivate.Add(group);
    }

    void GenerateGroup()
    {
        List<GameObject> roomsToRemove = new List<GameObject>();
        bool addRooms = true;
        float playerX = player.transform.position.x;
        float removeRoomX = playerX - screenWidth;
        float addRoomX = playerX + screenWidth;
        float furthestRoomEndX = 0;

        foreach (GameObject group in ActiveGroups)
        {
            SpriteGroupData groupData = group.GetComponent<SpriteGroupData>();

            float roomWidth = 0.0f;

            Transform width = group.transform.Find("Width");

            if (width != null)
            {
                roomWidth = width.localScale.x;
            }

            else
            {
                roomWidth = groupData.GroupTileWidth * (groupData.GroupTileSize / 100);
            }

            float roomStartX = group.transform.position.x - (roomWidth * 0.5f);
            float roomEndX = roomStartX + roomWidth;

            if (roomStartX > addRoomX)
            {
                addRooms = false;
            }

            if (roomEndX < removeRoomX)
            {
                roomsToRemove.Add(group);
            }

            furthestRoomEndX = Mathf.Max(furthestRoomEndX, roomEndX);
            /*
            // hacky, but it works.
            if (group.name == "ObstacleGroup1" && firstGenerate == false)
            {
                Debug.Log("hacky");
                Debug.Log(furthestRoomEndX);
                Debug.Log(roomEndX);
                Debug.Log(roomStartX);
                Debug.Log(roomWidth);
                SpawnElementGroup(furthestRoomEndX);// + (roomWidth * 0.5f));
                firstGenerate = true;
                return;
            }
            */

        }

        // deactivate groups
        foreach (GameObject group in roomsToRemove)
        {
            Deactivate(group);

            
        }

        if (addRooms == true)
        {
            SpawnElementGroup(furthestRoomEndX);
        }
    }


    void SpawnElementGroup(float furthestRoomEndX)
    {
        // find random inactive element group
        GameObject newGroup = InactiveGroups[Random.Range(0, InactiveGroups.Count - 1)];
        //Debug.Log("spawning element group: " + newGroup.name);
        //Messenger.Broadcast("GroupActivated" + newGroup.name);

        // enable it + all children



        newGroup.SetActive(true);

        foreach (Transform element in newGroup.transform)
        {
            element.gameObject.SetActive(true);
        }

        SpriteGroupData groupData = newGroup.GetComponent<SpriteGroupData>();

        // spawn powerups and obstacles if this is a group that does so

        if (groupData != null)
        {
            groupData.SpawnObjects();
        }

        // move to active

        InactiveGroups.Remove(newGroup);

        ActiveGroups.Add(newGroup);

        float roomWidth = 0.0f;

        Transform width = newGroup.transform.Find("Width");

        if (width != null)
        {
            roomWidth = width.localScale.x;
        }

        else
        {
            roomWidth = groupData.GroupTileWidth * (groupData.GroupTileSize / 100);
        }
        
        float roomCenter = furthestRoomEndX + roomWidth * 0.5f;

        
        newGroup.transform.position = new Vector3(roomCenter, newGroup.transform.position.y, newGroup.transform.position.z);
    }

    void StartGenerating()
    {
        generating = true;
    }
}

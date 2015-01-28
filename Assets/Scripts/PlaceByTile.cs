using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PlaceByTile : MonoBehaviour 
{
    public Vector3 Tile;
    public float PixelUnitRatio;
    public float PixelWidth;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        transform.position = new Vector3(Tile.x * (PixelWidth / PixelUnitRatio), transform.position.y, transform.position.z);
	}
}

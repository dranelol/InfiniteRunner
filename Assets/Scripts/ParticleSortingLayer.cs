using UnityEngine;
using System.Collections;

public class ParticleSortingLayer : MonoBehaviour 
{
    public string SortingLayerName;
    public int SortingLayerOrder;

	// Use this for initialization
	void Start () 
    {
        particleSystem.renderer.sortingLayerName = SortingLayerName;
        particleSystem.renderer.sortingOrder = SortingLayerOrder;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class Opacity : MonoBehaviour {

	public float OpacityVal;

    SpriteRenderer sprite;
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        sprite.color = new Color(1, 1, 1, OpacityVal);
	}
}

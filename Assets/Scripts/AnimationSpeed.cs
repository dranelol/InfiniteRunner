using UnityEngine;
using System.Collections;

public class AnimationSpeed : MonoBehaviour 
{
    public float AnimationSpeedVal;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        anim.speed = AnimationSpeedVal;
	}
}

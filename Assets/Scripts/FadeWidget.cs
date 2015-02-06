using UnityEngine;
using System.Collections;

public class FadeWidget : MonoBehaviour 
{
    public GameObject ComponentToFade;
    TweenAlpha tween;

    void Awake()
    {
        tween = ComponentToFade.GetComponent<TweenAlpha>();
    }

    public void FadeOut()
    {
        tween.enabled = true;
        tween.PlayForward();
    }

    public void FadeIn()
    {
        tween.enabled = true;
        tween.PlayReverse();
    }
}

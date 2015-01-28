using UnityEngine;
using System.Collections;

public class Disable : MonoBehaviour {

    public void DisableThis()
    {
        //gameObject.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<UISprite>().enabled = false;
        GetComponentInChildren<UILabel>().enabled = false;
    }
}

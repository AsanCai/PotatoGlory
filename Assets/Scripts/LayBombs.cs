using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayBombs : MonoBehaviour {

    [HideInInspector]
    public bool bombLaid = false;

    public int bombCount = 0;
    public AudioClip bombsAway;
    public GameObject bomb;


    //private GUITexture bombHUD;

    //private void Awake() {
    //    bombHUD = GameObject.Find("bombHUD").GetComponent<GUITexture>();
    //}


    void Update () {
		if(Input.GetButtonDown("Fire2") && !bombLaid && bombCount > 0) {
            bombCount--;

            bombLaid = true;

            AudioSource.PlayClipAtPoint(bombsAway, transform.position);

            Instantiate(bomb, transform.position, transform.rotation);
        }

        //bombHUD.enabled = bombCount > 0;

    }
}

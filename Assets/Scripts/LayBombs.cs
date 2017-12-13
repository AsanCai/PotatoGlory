using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayBombs : MonoBehaviour {

    [HideInInspector]
    public bool bombLaid = false;

    public int bombCount = 0;
    public AudioClip bombsAway;
    public GameObject bomb;


    private Image bombHUD;

    private void Awake() {
        GameObject obj = GameObject.Find("bombHUD");

        bombHUD = obj.GetComponent<Image>();
    }


    void Update () {
		if(Input.GetButtonDown("Fire2") && !bombLaid && bombCount > 0) {
            bombCount--;

            bombLaid = true;

            AudioSource.PlayClipAtPoint(bombsAway, transform.position);

            Instantiate(bomb, transform.position, transform.rotation);
        }

        bombHUD.enabled = bombCount > 0;

    }
}

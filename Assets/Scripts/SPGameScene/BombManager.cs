using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombManager : MonoBehaviour {
    
    [HideInInspector]
    public static BombManager bm;
    public int bombCount;
    [HideInInspector]
    public bool bombLaid;

    //提示可放置炸弹的UI
    private Image bombHUD;

    private void Awake() {
        bm = GetComponent<BombManager>();

        bombHUD = GameObject.Find("bombHUD").GetComponent<Image>();
    }

    private void Update() {
        if (bombCount > 0 && bombHUD.enabled == false) {
            bombHUD.enabled = true;
            return;
        } 

        if(bombCount <=0 && bombHUD.enabled == true) {
            bombHUD.enabled = false;
            return;
        }
    }

    private void Start() {
        bombLaid = false;
    }
}

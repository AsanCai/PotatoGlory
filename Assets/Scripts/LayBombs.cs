using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LayBombs : MonoBehaviour, IPointerDownHandler {

    [HideInInspector]
    public bool bombLaid = false;

    public int bombCount = 0;
    public AudioClip bombsAway;
    public GameObject bomb;
    
    //提示可防止炸弹的UI
    private Image bombHUD;
    //因为放置炸弹的代码绑定到按钮上了，所以需要获取玩家的位置实例化炸弹
    private Transform player;

    private void Awake() {
        GameObject obj = GameObject.Find("bombHUD");
        bombHUD = obj.GetComponent<Image>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    public virtual void OnPointerDown(PointerEventData ped) {

        if(!bombLaid && bombCount > 0) {
            bombCount--;

            bombLaid = true;

            AudioSource.PlayClipAtPoint(bombsAway, player.position);

            Instantiate(bomb, player.position, player.rotation);
        }
    }

    private void Update() {
        if(bombCount > 0) {
            bombHUD.enabled = true;
        } else {
            bombHUD.enabled = false;
        }
    }
}

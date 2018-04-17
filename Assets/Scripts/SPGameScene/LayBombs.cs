using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LayBombs : MonoBehaviour, IPointerDownHandler {
    public AudioClip bombsAway;
    public GameObject bomb;
    
    
    //因为放置炸弹的代码绑定到按钮上了，所以需要获取玩家的位置实例化炸弹
    private Transform player;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    public virtual void OnPointerDown(PointerEventData ped) {

        if(!BombManager.bm.bombLaid && BombManager.bm.bombCount > 0) {
            BombManager.bm.bombCount--;

            BombManager.bm.bombLaid = true;

            AudioSource.PlayClipAtPoint(bombsAway, player.position);

            Instantiate(bomb, player.position, player.rotation);
        }
    }
}

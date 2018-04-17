using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootBombs : MonoBehaviour, IPointerDownHandler {
    public GameObject bomb;
    public float force = 1000.0f;

    //因为放置炸弹的代码绑定到按钮上了，所以需要获取玩家的位置实例化炸弹
    private Transform player;
    private Transform gun;
    private AudioSource audioSource;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        gun = player.Find("Gun");

        audioSource = gun.gameObject.GetComponent<AudioSource>();
    }


    public virtual void OnPointerDown(PointerEventData ped) {
        if (!BombManager.bm.bombLaid && BombManager.bm.bombCount > 0) {
            BombManager.bm.bombCount--;

            BombManager.bm.bombLaid = true;

            audioSource.Play();

            GameObject obj = Instantiate(bomb, gun.position, Quaternion.identity);
            Vector3 vec = new Vector3(force * player.localScale.x, 0f, 0f);
            obj.GetComponent<Rigidbody2D>().AddForce(vec);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon;

namespace AsanCai.MPScene {
    public class KillTrigger : PunBehaviour {

        public GameObject splash;

        private GameObject cam;
        private PlayerHealth playerHealth;

        private void Awake() {
            cam = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.tag == "Player") {
                cam.GetComponent<CameraFollow>().enabled = false;

                playerHealth = collision.GetComponent<PlayerHealth>();

                if (playerHealth.isAlive) {
                    playerHealth.Hurt(playerHealth.maxHP, collision.transform.position);
                }

                //这里不能删去玩家对象，否则会出现同步错误
                //把玩家对象的所有Sprite子对象设置为不可见即可
                SpriteRenderer[] spr = collision.gameObject.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer s in spr) {
                    s.enabled = false;
                }

                Instantiate(splash, collision.transform.position, transform.rotation);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon;

namespace AsanCai.MPScene {
    public class HealthPickup : PunBehaviour {
        [Tooltip("回复的血量")]
        public int healthBouns = 25;
        [Tooltip("拾取加血包道具的音效")]
        public AudioClip collect;

        private Animator anim;
        private bool landed;

        private PlayerHealth playerHealth;
        private bool hasPicked = false;

        private void Awake() {
            anim = transform.root.GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (!hasPicked && collision.tag == "Player") {
                //避免重复拾取道具
                hasPicked = true;
                //让两个客户端同时执行
                photonView.RPC("PickedUp", PhotonTargets.All);
                

                playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                playerHealth.Recover(healthBouns);
            }

            if (collision.tag == "ground" && !landed) {
                anim.SetTrigger("Land");

                transform.parent = null;
                gameObject.AddComponent<Rigidbody2D>();
                landed = true;
            }
        }

        [PunRPC]
        private void PickedUp() {
            AudioSource.PlayClipAtPoint(collect, transform.position);
            Destroy(transform.root.gameObject);
        }
    }
}

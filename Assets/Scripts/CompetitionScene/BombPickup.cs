using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon;

namespace AsanCai.Competition {
    public class BombPickup : PunBehaviour {
        [Tooltip("捡到炸弹时的音效")]
        public AudioClip collect;
        [Tooltip("增加的炸弹数")]
        public int bombNum = 1;

        private Animator anim;
        private bool landed = false;

        private LayBombs layBombs;
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

                layBombs = collision.gameObject.GetComponent<LayBombs>();
                layBombs.PickUpBomb(bombNum);
            }

            if (!landed && collision.tag == "ground") {
                anim.SetTrigger("Land");

                transform.parent = null;
                gameObject.AddComponent<Rigidbody2D>();
                landed = true;
            }
        }

        [PunRPC]
        private void PickedUp() {
            hasPicked = true;

            AudioSource.PlayClipAtPoint(collect, transform.position);
            Destroy(transform.root.gameObject);
        }
    }
}

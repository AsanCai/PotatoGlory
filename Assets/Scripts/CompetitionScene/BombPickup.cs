using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsanCai.Competition {
    public class BombPickup : MonoBehaviour {
        [Tooltip("捡到炸弹时的音效")]
        public AudioClip pickupClip;
        [Tooltip("增加的炸弹数")]
        public int bombNum = 1;

        private Animator anim;
        private bool landed = false;

        private LayBombs layBombs;

        private void Awake() {
            anim = transform.root.GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.tag == "Player") {
                Destroy(transform.root.gameObject);

                AudioSource.PlayClipAtPoint(pickupClip, transform.position);

                layBombs = collision.gameObject.GetComponent<LayBombs>();
                layBombs.AddBomb(bombNum);
            } else {
                if (!landed && collision.tag == "ground") {
                    anim.SetTrigger("Land");

                    transform.parent = null;
                    gameObject.AddComponent<Rigidbody2D>();
                    landed = true;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon;

namespace AsanCai.Competition {
    public class Bomb : PunBehaviour {
        [Tooltip("炸弹的爆炸半径")]
        public float bombRadius = 8f;
        [Tooltip("角色被炸弹炸到时受到的冲击力")]
        public float bombForce = 100f;
        [Tooltip("炸弹对玩家产生伤害")]
        public int damage = 50;

        [Tooltip("炸弹爆炸音效")]
        public AudioClip boom;
        [Tooltip("炸弹倒计时音效")]
        public AudioClip fuse;
        [Tooltip("炸弹倒计时时间")]
        public float fuseTime = 3f;

        [Tooltip("爆炸白光特效")]
        public GameObject explosion;

        //爆炸粒子特效
        private ParticleSystem explosionFX;

        //炸弹Sprite
        private SpriteRenderer sprite;

        private void Awake() {
            explosionFX = GameObject.
                FindGameObjectWithTag("ExplosionFX").GetComponent<ParticleSystem>();

            sprite = GetComponent<SpriteRenderer>();
        }

        private void Start() {
            if (transform.root == transform)
                StartCoroutine(BombDetonation());
        }

        IEnumerator BombDetonation() {
            AudioSource.PlayClipAtPoint(fuse, transform.position);

            yield return new WaitForSeconds(fuseTime);

            //调用爆炸函数
            photonView.RPC("Explode", PhotonTargets.All, transform.position);
        }

        [PunRPC]
        public void Explode(Vector3 pos) {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, bombRadius, 1 << LayerMask.NameToLayer("Enemies"));

            foreach (Collider2D en in enemies) {
                Rigidbody2D rb = en.GetComponent<Rigidbody2D>();

                if (rb != null) {
                    if (rb.tag == "Enemy") {
                        //rb.gameObject.GetComponent<Enemy>().HP = 0;
                    }

                    if (rb.tag == "Player") {
                        if (PhotonNetwork.isMasterClient) {
                            en.GetComponent<PlayerHealth>().Hurt(damage, pos, true);
                        }

                        //实现被炸飞的效果
                        Vector3 deltaPos = rb.transform.position - pos;
                        Vector3 force = deltaPos.normalized * bombForce;
                        rb.AddForce(force);
                    }
                }
            }

            explosionFX.transform.position = pos;
            explosionFX.Play();

            Instantiate(explosion, pos, Quaternion.identity);

            AudioSource.PlayClipAtPoint(boom, pos);

            //设置炸弹为不可见
            sprite.enabled = false;

            //延迟销毁
            Destroy(gameObject, 1.0f);
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon;

namespace AsanCai.Competition {
    public class Bomb : PunBehaviour {
        [Tooltip("炸弹的爆炸半径")]
        public float bombRadius = 8f;
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

        //用于保存是哪个玩家实例化的
        [HideInInspector]
        private int player;
        //爆炸粒子特效
        private ParticleSystem explosionFX;

        private PlayerHealth playerHealth;

        private void Awake() {
            explosionFX = GameObject.
                FindGameObjectWithTag("ExplosionFX").GetComponent<ParticleSystem>();
        }

        private void Start() {
            //只有主客户端才能发起协程
            //避免两个客户端都发起协程，重复调用
            if (transform.root == transform && PhotonNetwork.isMasterClient)
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

            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, bombRadius, 
                1 << LayerMask.NameToLayer("Enemies") | 1 << LayerMask.NameToLayer("Player"));

            foreach (Collider2D en in enemies) {
                Rigidbody2D rb = en.GetComponent<Rigidbody2D>();

                if (rb != null) {
                    if (rb.tag == "Enemy") {
                        if (PhotonNetwork.isMasterClient) {
                            Enemy enemy = en.GetComponent<Enemy>();

                            enemy.Hurt(enemy.maxHP, pos, player);
                        }
                    }

                    if (rb.tag == "Player") {
                        playerHealth = en.GetComponent<PlayerHealth>();
                        //当能对其他玩家造成伤害且玩家不为无敌状态时，炸弹才能对玩家造成伤害
                        if (GameManager.gm.hurtOtherPlayer && !playerHealth.invincible) {
                            if (PhotonNetwork.isMasterClient) {
                                en.GetComponent<PlayerHealth>().Hurt(damage, pos);
                            }
                        }
                    }
                }
            }
            

            explosionFX.transform.position = pos;
            explosionFX.Play();

            Instantiate(explosion, pos, Quaternion.identity);

            AudioSource.PlayClipAtPoint(boom, pos);

            Destroy(gameObject);
        }


        public void CreatedByPlayer(int p) {
            photonView.RPC("SetPlayer", PhotonTargets.All, p);
        }

        [PunRPC]
        private void SetPlayer(int p) {
            player = p;
        }
    }

}
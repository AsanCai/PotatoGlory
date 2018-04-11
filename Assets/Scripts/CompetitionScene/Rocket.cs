using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon;

namespace AsanCai.Competition {
    public class Rocket : PunBehaviour {
        [Tooltip("爆炸效果")]
        public GameObject explosion;
        [Tooltip("导弹造成的伤害")]
        public int damage = 10;

        private PlayerHealth playerHealth;

        void Start() {
            //如果在2秒内没有被销毁，就由这行代码销毁
            Destroy(gameObject, 2);
        }

        [PunRPC]
        void OnExplode(Vector3 pos) {
            //随机生成一个四元数
            Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            //实例化爆炸对象
            Instantiate(explosion, pos, randomRotation);
            
            //销毁自身
            Destroy(gameObject);
        }


        private void OnTriggerEnter2D(Collider2D collision) {
            //若子弹打中Enemy，那么调用Enemy的Hurt函数
            if (collision.tag == "Enemy") {

                collision.gameObject.GetComponent<Enemy>().Hurt();

                photonView.RPC("OnExplode", PhotonTargets.All, transform.position);
                OnExplode(transform.position);

            } else if (collision.tag == "BombPickUp") {
                //collision.gameObject.GetComponent<Bomb>().Explode();

                //Destroy(collision.transform.root.gameObject);

                //Destroy(gameObject);


            } else if(collision.tag == "Player") {
                playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

                //当玩家为无敌状态时，不做任何操作
                if (playerHealth.invincible) {
                    return;
                }

                //调用受击函数
                playerHealth.Hurt(damage, transform.position);

                //调用爆炸函数
                photonView.RPC("OnExplode", PhotonTargets.All, transform.position);

            } else {
                //调用爆炸函数
                photonView.RPC("OnExplode", PhotonTargets.All, transform.position);
            }
        }
    }
}

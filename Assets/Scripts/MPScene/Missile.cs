using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon;

namespace AsanCai.MPScene {
    public class Missile : PunBehaviour {
        [Tooltip("爆炸效果")]
        public GameObject explosion;
        [Tooltip("导弹造成的伤害")]
        public int damage = 10;

        //用于保存是哪个玩家实例化的
        [HideInInspector]
        private int player;

        private PlayerHealth playerHealth;

        


        private void OnTriggerEnter2D(Collider2D collision) {
            //若子弹打中Enemy，那么调用Enemy的Hurt函数
            if (collision.tag == "Enemy") {
                Enemy enemy = collision.GetComponent<Enemy>();

                enemy.Hurt(enemy.damage, transform.position, player);

                photonView.RPC("OnExplode", PhotonTargets.All, transform.position);

            } else if(collision.tag == "Player") {
                playerHealth = collision.GetComponent<PlayerHealth>();
                //当能对其他玩家造成伤害或者玩家不为无敌状态时，对其造成伤害
                if (GameManager.gm.hurtOtherPlayer && !playerHealth.invincible) {
                    //调用受击函数
                    playerHealth.Hurt(damage, transform.position);
                    //调用爆炸函数
                    photonView.RPC("OnExplode", PhotonTargets.All, transform.position);
                }
            } else {
                //调用爆炸函数
                photonView.RPC("OnExplode", PhotonTargets.All, transform.position);
            }
        }


        public void CreatedByPlayer(int p) {
            photonView.RPC("SetPlayer", PhotonTargets.All, p);
        }

        #region RPC函数
        [PunRPC]
        private void SetPlayer(int p) {
            player = p;
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
        #endregion
    }
}

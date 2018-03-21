using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

namespace AsanCai.Competition {
    public class PlayerHealth : PunBehaviour {

        [Tooltip("角色的生命值")]
        public int maxHP = 100;
        [Tooltip("角色受到伤害后的免伤时间")]
        public float invincibleTime = 2f;
        [Tooltip("角色受到伤害时受到的力，制造击退效果")]
        public float hurtForce = 10f;
        [Tooltip("受伤害时减少的血量")]
        public int damageAmount = 10;
        [Tooltip("受伤音效")]
        public AudioClip[] ouchClips;

        //玩家所属队伍
        [HideInInspector]
        public int player;
        //玩家当前的血量
        [HideInInspector]
        public int currentHP;
        //玩家是否存活
        [HideInInspector]
        public bool isAlive;
        //玩家对象是否无敌
        [HideInInspector]
        public bool invincible;   


        private PlayerController playCtrl;
        private Animator anim;
        //用于检测当前是否处于免伤状态
        private float timer;

        private void Awake() {
            currentHP = maxHP;
            isAlive = true;
            invincible = false;
            timer = 0;
        }

        private void Start() {
            playCtrl = GetComponent<PlayerController>();
            anim = GetComponent<Animator>();

            if (!photonView.isMine) {
                return;
            }

            //使用RPC，更新其他客户端中该玩家对象当前血量
            photonView.RPC("UpdateHP", PhotonTargets.Others, currentHP);	
            

            if(PhotonNetwork.player.CustomProperties["Player"].ToString() == "Player1") {
                player = 1;
            } else {
                player = 2;
            }
            //使用RPC，设置其他客户端中该玩家对象的队伍
            photonView.RPC("SetPlayer", PhotonTargets.Others, player);		
        }

        private void Update() {
            //不是本地玩家对象，结束函数执行
            if (!photonView.isMine)     
                return;

            //累加玩家对象的无敌时间
            timer += Time.deltaTime;

            //使用RPC，设置所有客户端该玩家对象的无敌状态
            if (timer > invincibleTime && invincible == true) {
                photonView.RPC("SetInvincible", PhotonTargets.All, false);
            }
            if (timer <= invincibleTime && invincible == false) {
                photonView.RPC("SetInvincible", PhotonTargets.All, true);

                //重置计时器
                timer = 0;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            //假如撞到怪物
            if (collision.gameObject.tag == "Enemy") {
                //调用受伤函数
                TakeDamage(damageAmount, collision.transform);
            }
        }

        //受伤函数
        public void TakeDamage(int damage, Transform enemy) {
            //玩家死亡或者无敌，不执行扣血函数
            if (!isAlive || invincible)            
                return;

            //角色不能跳跃
            playCtrl.jump = false;

            //给角色加上后退的力，制造击退效果
            Vector3 hurtVector = transform.position - enemy.position + Vector3.up * 5f;
            GetComponent<Rigidbody2D>().AddForce(hurtVector * hurtForce);

            //随机播放音频
            int i = Random.Range(0, ouchClips.Length);
            AudioSource.PlayClipAtPoint(ouchClips[i], transform.position);


            if (PhotonNetwork.isMasterClient) {
                //更新角色的生命值
                currentHP -= damage;
                //更新所有客户端，该玩家对象的生命值
                photonView.RPC("UpdateHP", PhotonTargets.All, currentHP);	
            }
        }

        [PunRPC]
        public void UpdateHP(int newHP) {
            currentHP = newHP;

            if(currentHP <= 0) {
                isAlive = false;

                Death();
            }
        }

        //RPC函数，设置玩家的无敌状态
        [PunRPC]
        void SetInvincible(bool isInvincible) {
            invincible = isInvincible;
        }

        //RPC函数，设置玩家队伍
        [PunRPC]
        void SetPlayer(int newpalyer) {
            player = newpalyer;
        }


        private void Death() {

            if (photonView.isMine) {
                //播放死亡动画
                anim.SetTrigger("Die");
            }

            //角色死亡
            Collider2D[] cols = GetComponents<Collider2D>();
            foreach (Collider2D c in cols) {
                c.isTrigger = true;
            }

            //把sortingLayer改为UI，下落的时候可以看到
            SpriteRenderer[] spr = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer s in spr) {
                s.sortingLayerName = "UI";
            }

            //禁用脚本
            GetComponent<PlayerController>().enabled = false;
            GetComponent<PlayerShoot>().enabled = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

namespace AsanCai.MPScene {
    public class PlayerHealth : PunBehaviour {

        [Tooltip("角色的生命值")]
        public int maxHP = 100;
        [Tooltip("角色受到伤害后的免伤时间")]
        public float invincibleTime = 2f;
        [Tooltip("角色受到伤害时受到的力，制造击退效果")]
        public float hurtForce = 100f;
        [Tooltip("受伤害时减少的血量")]
        public int damageAmount = 10;
        [Tooltip("受伤音效")]
        public AudioClip[] ouchClips;
        [Tooltip("玩家头上的血量条")]
        public SpriteRenderer healthBar;

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
        private PlayerShoot playerShoot;
        private Rigidbody2D body;
        private Animator anim;
        //用于操作玩家头上的血量条
        private Vector3 healthScale;
        //计时器
        private float timer;
        //用于设置自定义属性
        private ExitGames.Client.Photon.Hashtable costomProperties;


        private void Awake() {
            playCtrl = GetComponent<PlayerController>();
            playerShoot = GetComponent<PlayerShoot>();
            body = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();

            //初始化必需的值
            currentHP = maxHP;
            /* ***
             * 不在这里赋值，而是直接在声明时赋值，炸弹和导弹不能对玩家造成伤害，为什么？
             * 无论在声明的时候赋什么值，两个变量都会被设置为false
             * ***/
            isAlive = true;
            invincible = false;
        }

        private void Start() {
            timer = 0;
            healthScale = healthBar.transform.localScale;

            if (!photonView.isMine) {
                return;
            }

            //使用RPC，更新其他客户端中该玩家对象当前血量
            photonView.RPC("UpdateHP", PhotonTargets.Others, currentHP, PhotonNetwork.player);	
            
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

            if (invincible) {
                //当前是无敌状态，累加玩家对象的无敌时间
                timer += Time.deltaTime;
                //超过无敌时间，退出无敌状态
                if (timer > invincibleTime) {
                    photonView.RPC("SetInvincible", PhotonTargets.All, false);

                    //当前不是无敌状态，重置计时器
                    timer = 0;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            //假如撞到怪物
            if (collision.gameObject.tag == "Enemy") {
                //调用受伤函数
                photonView.RPC("TakeDamage", PhotonTargets.All,
                    damageAmount, collision.transform.position);
            }
        }

        #region 公用函数
        //受伤函数
        public void Hurt(int damage, Vector3 enemyPos) {
            //确保无论是哪个客户端触发的，都能更新血量
            photonView.RPC("TakeDamage", PhotonTargets.All, damage, enemyPos);
        }

        //加血函数
        public void Recover(int hp) {
            //确保无论是哪个客户端触发的，都能更新血量
            photonView.RPC("AddHp", PhotonTargets.All, hp);
        }

        //死亡函数
        private void Death() {

            //播放死亡动画，这里死亡动画不需要进行同步
            anim.SetTrigger("Die");

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
            playCtrl.enabled = false;
            playerShoot.enabled = false;
        }
        #endregion

        #region RPC函数
        [PunRPC]
        //受伤函数
        private void TakeDamage(int damage, Vector3 hitPos) {
            //玩家死亡或者无敌，不执行扣血函数
            if (!isAlive || invincible)
                return;

            //让角色进入无敌状态
            photonView.RPC("SetInvincible", PhotonTargets.All, true);
            //角色不能跳跃
            playCtrl.jump = false;

            Vector3 hurtVector = transform.position - hitPos + Vector3.up * 5f ;
            body.AddForce(hurtVector * hurtForce);


            //随机播放音频
            int i = Random.Range(0, ouchClips.Length);
            AudioSource.PlayClipAtPoint(ouchClips[i], transform.position);

            //只有主客户端有权限更新血量值
            if (PhotonNetwork.isMasterClient) {
                //更新角色的生命值
                currentHP -= damage;
                //更新所有客户端，该玩家对象的生命值
                photonView.RPC("UpdateHP", PhotonTargets.All, currentHP, PhotonNetwork.player);
                //更新玩家头上血量条的显示
                photonView.RPC("UpdateHealthDisplay", PhotonTargets.All);
            }
        }

        [PunRPC]
        private void AddHp(int hp) {
            //只有主客户端有权限更新血量值
            if (PhotonNetwork.isMasterClient) {
                //更新角色的生命值
                currentHP = currentHP + hp;
                currentHP = currentHP > 100 ? 100 : currentHP;

                //更新所有客户端，该玩家对象的生命值
                photonView.RPC("UpdateHP", PhotonTargets.All, currentHP, PhotonNetwork.player);
                //更新玩家头上血量条的显示
                photonView.RPC("UpdateHealthDisplay", PhotonTargets.All);
            }
        }

        [PunRPC]
        private void UpdateHP(int newHP, PhotonPlayer p) {
            currentHP = newHP;

            if (currentHP <= 0) {
                isAlive = false;

                Death();
            }
            //设置玩家当前的存活属性
            costomProperties = new ExitGames.Client.Photon.Hashtable { { "isAlive", isAlive } };
            p.SetCustomProperties(costomProperties);

            //更新玩家存活状态
            GameManager.gm.UpdateAliveState(player, isAlive);
        }


        //更新玩家头上的血量条
        [PunRPC]
        private void UpdateHealthDisplay() {
            //把玩家头上的生命条的颜色逐渐变红
            healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - currentHP * 0.01f);
            //缩短玩家头上的生命条
            healthBar.transform.localScale = new Vector3(healthScale.x * currentHP * 0.01f, 1, 1);
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
        #endregion

        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon;

namespace AsanCai.Competition {
    public class Enemy : PunBehaviour {
        [Tooltip("怪物移动的速度")]
        public float moveSpeed = 2f;
        [Tooltip("怪物的血量，也就是要被子弹打几下才死")]
        public int maxHP = 2;
        [Tooltip("受伤时受到的力")]
        public float hurtForce;
        [Tooltip("普通受伤时减少的血量")]
        public int damage = 1;
        [Tooltip("击杀怪物的得分")]
        public int score;
        [Tooltip("死亡的图片")]
        public Sprite deadEnemy;
        [Tooltip("受伤的图片")]
        public Sprite damagedEnemy;
        [Tooltip("死亡的音效")]
        public AudioClip[] deathClips;
        [Tooltip("保存得分的特效")]
        public GameObject hundredPointsUI;
        [Tooltip("死亡时的最小旋转角")]
        public float deathSpinMin = -100f;
        [Tooltip("死亡时的最大旋转角")]
        public float deathSpinMax = 100f;
        [Tooltip("用于检测怪物前方是否有障碍物")]
        public Transform frontCheck;
        [Tooltip("用于检测当前是否站在地面上")]
        public Transform groundCheck;

        //用于设置怪物对象显示的Sprite
        private SpriteRenderer spriteRenderer;
        //检测怪物当前是否死亡
        private bool dead = false;
        //用于设置怪物对象的物理属性
        private Rigidbody2D body;
        //怪物当前血量
        private int currentHp;
        //判断怪物当前是否站在地面上
        private bool grounded;

        private void Awake() {
            spriteRenderer = transform.Find("body").GetComponent<SpriteRenderer>();

            body = GetComponent<Rigidbody2D>();
        }

        private void Start() {
            currentHp = maxHP;
            //初始化hp，此时设置一个玩家为0，即不存在
            photonView.RPC("UpdateHp", PhotonTargets.All, currentHp, 0);
        }

        private void Update() {
            //通过检测角色和groundCheck之间是否存在Ground层的物体来判断当前是否落地
            grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        }

        private void FixedUpdate() {
            Collider2D[] frontHits = Physics2D.OverlapPointAll(frontCheck.position, 1);

            //假如前方是障碍物，就只执行转向操作
            foreach (Collider2D c in frontHits) {

                if (c.tag == "Obstacle") {
                    Flip();
                    break;
                }
            }

            //站在地面上时才水平运动
            if (grounded) {
                body.velocity = new Vector2(
                    transform.localScale.x * moveSpeed, 
                    body.velocity.y
                    );
            }
        }

        public void Hurt(int damage, Vector3 hitPos, int player) {
            photonView.RPC("TakeDamage", PhotonTargets.All, damage, hitPos, player);
        }

        [PunRPC]
        private void TakeDamage(int damage, Vector3 hitPos, int player) {
            Vector3 hurtVector = transform.position - hitPos + Vector3.up * 5f;
            body.AddForce(hurtVector * hurtForce);

            if (PhotonNetwork.isMasterClient) {
                currentHp -= damage;

                photonView.RPC("UpdateHp", PhotonTargets.All, currentHp, player);
            }
        }

        [PunRPC]
        private void UpdateHp(int newHp, int player) {
            currentHp = newHp;

            if (currentHp == 1 && damagedEnemy != null) {
                spriteRenderer.sprite = damagedEnemy;
            }

            if (currentHp <= 0 && !dead) {
                Death();

                ScoreManager.sm.AddScore(player, score);
            }
        }

        private void Death() {
            SpriteRenderer[] otherRenderers = GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer s in otherRenderers) {
                s.enabled = false;
            }

            spriteRenderer.enabled = true;
            spriteRenderer.sprite = deadEnemy;

            dead = true;

            Collider2D[] cols = GetComponents<Collider2D>();

            foreach (Collider2D c in cols) {
                c.isTrigger = true;
            }

            int i = Random.Range(0, deathClips.Length);
            AudioSource.PlayClipAtPoint(deathClips[i], transform.position);

            Vector3 scorePos;
            scorePos = transform.position;

            int temp = score / 100;
            for(int index = 0; index < temp; index++) {
                scorePos.y += 1f;

                Instantiate(hundredPointsUI, scorePos, Quaternion.identity);
            }
        }

        private void Flip() {
            Vector3 enemyScale = transform.localScale;

            enemyScale.x *= -1;

            transform.localScale = enemyScale;
        }
    }

}
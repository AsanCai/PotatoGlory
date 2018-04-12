using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

using Photon;

namespace AsanCai.Competition {
    public class PlayerShoot : PunBehaviour {
        [Tooltip("发射冷却时间")]
        public float coolingTime;
        [Tooltip("子弹的发射速度")]
        public float speed = 25f;
        [Tooltip("枪口对象")]
        public GameObject gun;
        [Tooltip("初始可发射的导弹数")]
        public int initMissileNum;

        //可发射导弹数
        private int missileCount;
        //获取角色控制脚本引用
        private PlayerController playerCtrl;
        //角色的动画播放器
        private Animator animator;
        //附在枪口上的播放器
        private AudioSource audioSource;
        //火箭的实例
        private GameObject rocket;
        //用于限制玩家发射导弹
        private bool shooted = false;
        //计时器
        private float timer = 0.0f;

        private void Awake() {
            animator = GetComponent<Animator>();
            playerCtrl = GetComponent<PlayerController>();

            audioSource = gun.GetComponent<AudioSource>();
        }

        private void Start() {
            //初始化missileCount
            missileCount = initMissileNum;

            //初始化导弹数量标签
            PickupManager.pm.UpdateMissileText(missileCount);
        }


        void Update() {
            if (shooted) {
                timer += Time.deltaTime;

                if(timer > coolingTime) {
                    shooted = false;
                }
            }

            if (CrossPlatformInputManager.GetButtonDown("Fire1")) {
                if(!shooted && missileCount > 0) {
                    //避免玩家立即重复发射
                    shooted = true;
                    //重置计时器
                    timer = 0.0f;

                    //更新剩余的炸弹数
                    missileCount--;
                    PickupManager.pm.UpdateMissileText(missileCount);

                    //判断是进行技能冷却还是禁用按钮
                    if (missileCount > 0) {
                        ButtonManager.bm.fireBtn.OnBtnClickSkill(coolingTime);
                    } else {
                        ButtonManager.bm.fireBtn.SetAxisNegativeState();
                    }

                    //因为播放射击动画的状态是从AnyState触发的，所以需要使用SetTrigger，触发之后自动重置，避免多次循环
                    animator.SetTrigger("Shoot");
                    //播放射击音效
                    audioSource.Play();

                    //根据角色当前的朝向实例化导弹
                    /*******************************************************************
                     * 因为需要在所有客户端上都能看到发射的导弹
                     * 所以需要在导弹prefab上添加（Photon Transform View）并同步该脚本
                     ******************************************************************/
                    if (playerCtrl.facingRight) {
                        rocket = PhotonNetwork.Instantiate("missile", gun.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), 0);

                        rocket.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
                    } else {
                        rocket = PhotonNetwork.Instantiate("missile", gun.transform.position, Quaternion.Euler(new Vector3(0, 0, 180)), 0);

                        rocket.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0);
                    }
                }
            }
        }
    }

}
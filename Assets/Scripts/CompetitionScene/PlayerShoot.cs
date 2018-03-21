using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

using Photon;

namespace AsanCai.Competition {
    public class PlayerShoot : PunBehaviour {
        [Tooltip("子弹的发射速度")]
        public float speed = 25f;
        [Tooltip("枪口对象")]
        public GameObject gun;

        private PlayerController playerCtrl;
        //角色的动画播放器
        private Animator animator;
        //附在枪口上的播放器
        private AudioSource audioSource;
        //火箭的实例
        private GameObject rocket;

        private void Awake() {
            animator = GetComponent<Animator>();
            playerCtrl = GetComponent<PlayerController>();

            audioSource = gun.GetComponent<AudioSource>();
        }


        void Update() {
            if (CrossPlatformInputManager.GetButtonDown("Fire1")) {
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
                    rocket = PhotonNetwork.Instantiate("rocket", gun.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), 0);

                    rocket.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
                } else {
                    rocket = PhotonNetwork.Instantiate("rocket", gun.transform.position, Quaternion.Euler(new Vector3(0, 0, 180)), 0);

                    rocket.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0);
                }
            }
        }
    }

}
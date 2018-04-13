using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

using Photon;

namespace AsanCai.Competition {
    public class LayBombs : PunBehaviour{
        [Tooltip("放置炸弹的间隔时间")]
        public float coolingTime;
        [Tooltip("初始炸弹数量")]
        public int initBombNum;
        [Tooltip("放置炸弹的音效")]
        public AudioClip bombsAway;
        [Tooltip("射出炸弹的枪口")]
        public GameObject gun;
        [Tooltip("射出炸弹时的力")]
        public float force;

        //可放置炸弹的数量
        [HideInInspector]
        public int bombCount;
        //用于限制玩家放置炸弹
        private bool bombLaid = false;
        //计时器
        private float timer = 0.0f;
        //播放器
        private AudioSource audioSource;


        private void Awake() {
            audioSource = gun.GetComponent<AudioSource>();
        }

        private void Start() {
            bombCount = initBombNum;

            //同步炸弹数量，确保不同客户端的炸弹数量一样
            photonView.RPC("UpdateBombNum", PhotonTargets.All, bombCount);
        }


        private void Update() {
            if (bombLaid) {
                timer += Time.deltaTime;

                if(timer > coolingTime) {
                    bombLaid = false;
                }
            }

            //激活放置炸弹按钮
            if(ButtonManager.bm.layBombsBtn.ban && bombCount > 0) {
                ButtonManager.bm.layBombsBtn.SetAxisPositiveState();
                
            }
            //激活发射炸弹按钮
            if(ButtonManager.bm.throwBombsBtn.ban && bombCount > 0) {
                ButtonManager.bm.throwBombsBtn.SetAxisPositiveState();
            }

            //原地放置炸弹
            if (CrossPlatformInputManager.GetButtonDown("Fire2")) {
                if (!bombLaid && bombCount > 0) {
                    //不能立即放置第二颗炸弹
                    bombLaid = true;
                    //重置计时器
                    timer = 0.0f;

                    //更新剩余的炸弹数
                    ReleaseBomb(1);

                    //判断是进行技能冷却还是禁用按钮
                    if (bombCount > 0) {
                        ButtonManager.bm.layBombsBtn.OnBtnClickSkill(coolingTime);
                        ButtonManager.bm.throwBombsBtn.OnBtnClickSkill(coolingTime);
                    } else {
                        ButtonManager.bm.layBombsBtn.SetAxisNegativeState();
                        ButtonManager.bm.throwBombsBtn.SetAxisNegativeState();
                    }

                    AudioSource.PlayClipAtPoint(bombsAway, transform.position);
                    PhotonNetwork.Instantiate("bomb", transform.position, 
                        Quaternion.Euler(new Vector3(0, 0, 0)), 0);
                }
            }

            //射出炸弹
            if (CrossPlatformInputManager.GetButtonDown("Fire3")) {
                if (!bombLaid && bombCount > 0) {
                    //不能立即放置第二颗炸弹
                    bombLaid = true;
                    //重置计时器
                    timer = 0.0f;

                    //更新剩余的炸弹数
                    ReleaseBomb(1);

                    //判断是进行技能冷却还是禁用按钮
                    if (bombCount > 0) {
                        ButtonManager.bm.layBombsBtn.OnBtnClickSkill(coolingTime);
                        ButtonManager.bm.throwBombsBtn.OnBtnClickSkill(coolingTime);
                    } else {
                        ButtonManager.bm.layBombsBtn.SetAxisNegativeState();
                        ButtonManager.bm.throwBombsBtn.SetAxisNegativeState();
                    }

                    audioSource.Play();

                    GameObject bomb = PhotonNetwork.Instantiate("bomb", gun.transform.position,
                        Quaternion.Euler(new Vector3(0, 0, 0)), 0);

                    bomb.GetComponent<Rigidbody2D>().AddForce(new Vector3(
                            force * transform.localScale.x,
                            0,
                            0
                        ));
                }
            }
        }

        //拾取炸弹
        public void PickUpBomb(int num) {
            //确保无论是哪个客户端触发的，都能更新炸弹数量
            photonView.RPC("AddBomb", PhotonTargets.All, num);
        }

        //释放炸弹
        private void ReleaseBomb(int num) {
            bombCount -= num;
            photonView.RPC("UpdateBombNum", PhotonTargets.All, bombCount);
        }


        [PunRPC]
        private void AddBomb(int num) {
            if (PhotonNetwork.isMasterClient) {
                bombCount += num;

                photonView.RPC("UpdateBombNum", PhotonTargets.All, bombCount);
            }
        }

        [PunRPC]
        //更新炸弹数量
        private void UpdateBombNum(int num) {
            bombCount = num;
        }
    }
}
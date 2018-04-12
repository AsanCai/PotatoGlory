/**
 * 要注意各个Manager的初始化顺序
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Photon;

namespace AsanCai.Competition {
    public class PickupManager : PunBehaviour {
        [HideInInspector]
        public static PickupManager pm;

        [Tooltip("保存道具预设")]
        public GameObject[] pickups;
        [Tooltip("保存导弹数量提示")]
        public Text missileNumText;
        [Tooltip("保存炸弹数量提示")]
        public Text bombNumText;
        
        [Tooltip("生成第一个道具的时间间隔")]
        public float pickupDeliveryTime = 5f;
        [Tooltip("道具随机生成的范围")]
        public Transform dropRangeLeft;
        public Transform dropRangeRight;


        //获取本地玩家引用
        private GameObject player;
        //获取脚本引用
        private LayBombs layBombs;
        private PlayerShoot playerShoot;

        //获取道具名称
        private string[] pickupsName;

        void Awake() {
            pm = GetComponent<PickupManager>();
        }
        
        //初始化函数
        public void Init() {
            player = GameManager.gm.localPlayer;

            layBombs = player.GetComponent<LayBombs>();
            playerShoot = player.GetComponent<PlayerShoot>();

            //初始化数组
            pickupsName = new string[pickups.Length];
            for(int i = 0; i < pickups.Length; i++) {
                pickupsName[i] = pickups[i].name;
            }

            //让主客户端负责产生道具
            if (PhotonNetwork.isMasterClient) {
                StartCoroutine(DeliverPickup(pickupDeliveryTime));
            }
        }

        public IEnumerator DeliverPickup(float time) {
            //生成道具的间隔时间
            yield return new WaitForSeconds(time);

            float dropPosX = Random.Range(dropRangeLeft.position.x, dropRangeRight.position.x);
            Vector3 dropPos = new Vector3(dropPosX, 20f, 1f);


            int index = Random.Range(0, pickupsName.Length - 1);

            PhotonNetwork.Instantiate(pickupsName[index], dropPos, Quaternion.identity, 0);

            float nextTime = Random.Range(time, time * 2);

            StartCoroutine(DeliverPickup(nextTime));
        }

        #region 公用函数
        public void UpdateBombText(int num) {
            bombNumText.text = "X " + num;
        }

        public void UpdateMissileText(int num) {
            missileNumText.text = "X " + num;
        }

        #endregion
    }
}

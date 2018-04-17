/**
 * 要注意各个Manager的初始化顺序
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Photon;

namespace AsanCai.MPScene {
    public class PickupManager : PunBehaviour {
        [Tooltip("保存道具预设")]
        public GameObject[] pickups;
        [Tooltip("生成第一个道具的时间间隔")]
        public float pickupDeliveryTime = 5f;
        [Tooltip("道具随机生成的范围")]
        public Transform dropRangeLeft;
        public Transform dropRangeRight;

        //获取道具名称
        private string[] pickupsName;

        
        //初始化函数
        private void Start() {
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

        private IEnumerator DeliverPickup(float time) {
            //生成道具的间隔时间
            yield return new WaitForSeconds(time);

            float dropPosX = Random.Range(dropRangeLeft.position.x, dropRangeRight.position.x);
            Vector3 dropPos = new Vector3(dropPosX, 20f, 1f);


            int index = Random.Range(0, pickupsName.Length);

            PhotonNetwork.Instantiate(pickupsName[index], dropPos, Quaternion.identity, 0);

            float nextTime = Random.Range(pickupDeliveryTime, pickupDeliveryTime * 2);

            StartCoroutine(DeliverPickup(nextTime));
        }
    }
}

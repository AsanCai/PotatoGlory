using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsanCai.Competition {
    public class EnemySpawner : MonoBehaviour {
        [Tooltip("保存敌人预设")]
        public GameObject[] enemies;
        [Tooltip("生成第一个敌人的时间间隔")]
        public float enemySpawnedTime = 5f;
        [Tooltip("敌人随机生成的范围")]
        public Transform dropRangeLeft;
        public Transform dropRangeRight;

        //获取敌人名称
        private string[] enemiesName;

        void Start() {
            //初始化数组
            enemiesName = new string[enemies.Length];
            for (int i = 0; i < enemies.Length; i++) {
                enemiesName[i] = enemies[i].name;
            }

            //让主客户端负责产生敌人
            if (PhotonNetwork.isMasterClient) {
                StartCoroutine(SpawnEnemy(enemySpawnedTime));

                StartCoroutine(SpawnEnemy(enemySpawnedTime + 1f));

                StartCoroutine(SpawnEnemy(enemySpawnedTime + 2f));
            }
        }

        private IEnumerator SpawnEnemy(float time) {
            //生成道具的间隔时间
            yield return new WaitForSeconds(time);

            float dropPosX = Random.Range(dropRangeLeft.position.x, dropRangeRight.position.x);
            Vector3 dropPos = new Vector3(dropPosX, 20f, 1f);

            int index = Random.Range(0, enemiesName.Length);

            PhotonNetwork.Instantiate(enemiesName[index], dropPos, Quaternion.identity, 0);

            float nextTime = Random.Range(enemySpawnedTime, enemySpawnedTime * 2);

            StartCoroutine(SpawnEnemy(nextTime));
        }
    }
}

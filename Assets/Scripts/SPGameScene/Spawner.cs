using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public float spawnTime = 5f;
    public float spawnDelay = 3f;
    public GameObject[] enemies;


    void Start () {
        //spawnDelay秒之后调用Spawn函数，然后每隔spawnTime重复调用Spawn函数一次
        InvokeRepeating("Spawn", spawnDelay, spawnTime);
	}
	
	void Spawn() {
        //随机生成敌人的种类
        int enemyIndex = Random.Range(0, enemies.Length);
        
        //实例化相应的敌人
        Instantiate(enemies[enemyIndex], transform.position, transform.rotation);

        //依次播放子组件中所有例子效果
        foreach(ParticleSystem p in GetComponentsInChildren<ParticleSystem>()) {
            p.Play();
        }
    }
}

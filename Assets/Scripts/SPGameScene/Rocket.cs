//子弹的控制脚本

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    public GameObject explosion;

	void Start () {
        //如果在2秒内没有被销毁，就由这行代码销毁
        Destroy(gameObject, 2);
	}
	
	void OnExplode() {
        //随机生成一个四元数
        Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        //实例化爆炸对象
        Instantiate(explosion, transform.position, randomRotation);
        //销毁自身
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        //若子弹打中Enemy，那么调用Enemy的Hurt函数
        if(collision.tag == "Enemy") {

            collision.gameObject.GetComponent<Enemy>().Hurt();

            OnExplode();

        } else if(collision.tag  == "BombPickUp") {
            //collision.gameObject.GetComponent<Bomb>().Explode();

            Destroy(collision.transform.root.gameObject);

            Destroy(gameObject);
        }else if(collision.tag != "Player") {
            OnExplode();
        }
    }
}

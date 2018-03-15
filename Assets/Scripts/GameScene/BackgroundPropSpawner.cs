using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPropSpawner : MonoBehaviour {

    public Rigidbody2D backgroundProp;

    //生成的位置
    public float leftSpawnPosX;
    public float rightSpawnPosX;
    public float minSpawnPoxY;
    public float maxSpawnPoxY;

    //生成的间隔时间区间
    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;

    //生成之后运动的速度区间
    public float minSpeed;
    public float maxSpeed;

    
    void Start () {
        //设置一个seed，让随机数列不同
        Random.InitState(System.DateTime.Today.Millisecond);

        StartCoroutine(Spawn());
    }
	
	IEnumerator Spawn() {
        float waitTime = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);

        yield return new WaitForSeconds(waitTime);

        //设定面朝左边的概率
        bool facingLeft = Random.Range(0, 2) == 0;

        //判定生成的位置
        float posX = facingLeft ? rightSpawnPosX : leftSpawnPosX;
        float posY = Random.Range(minSpawnPoxY, maxSpawnPoxY);
        Vector3 spawnPos = new Vector3(posX, posY, transform.position.z);

        Rigidbody2D prop = Instantiate(backgroundProp, spawnPos, Quaternion.identity);

        //因为图片默认朝左边，所以需要进行翻转
        if (!facingLeft) {
            Vector3 scale = prop.transform.localScale;
            scale.x *= -1;
            prop.transform.localScale = scale;
        }

        float speed = Random.Range(minSpeed, maxSpeed);

        //根据朝向设定速度
        speed *= facingLeft ? -1f : 1f;

        prop.velocity = new Vector2(speed, 0);


        //递归调用后台生成函数，开始一个新的协程
        StartCoroutine(Spawn());

        //确保创造的prop被销毁之后才结束协程
        while (prop != null) {

            if (facingLeft) {

                if (prop.transform.position.x < leftSpawnPosX - 0.5f)
                    Destroy(prop.gameObject);
            } else {

                if (prop.transform.position.x > rightSpawnPosX + 0.5f)

                    Destroy(prop.gameObject);
            }

            yield return null;
        }
    }
}

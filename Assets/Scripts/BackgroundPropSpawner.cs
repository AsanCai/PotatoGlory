using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPropSpawner : MonoBehaviour {

    public Rigidbody2D backgroundProp;

    public float leftSpawnPosX;
    public float rightSpawnPosX;

    public float minSpawnPoxY;
    public float maxSpawnPoxY;

    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;

    public float minSpeed;
    public float maxSpeed;

    // Use this for initialization
    void Start () {
        Random.InitState(System.DateTime.Today.Millisecond);

        StartCoroutine(Spawn());
    }
	
	IEnumerator Spawn() {
        float waitTime = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);

        yield return new WaitForSeconds(waitTime);

        bool facingLeft = Random.Range(0, 2) == 0;

        float posX = facingLeft ? rightSpawnPosX : leftSpawnPosX;

        float posY = Random.Range(minSpawnPoxY, maxSpawnPoxY);

        Vector3 spawnPos = new Vector3(posX, posY, transform.position.z);

        Rigidbody2D prop = Instantiate(backgroundProp, spawnPos, Quaternion.identity);


        if (!facingLeft) {
            Vector3 scale = prop.transform.localScale;
            scale.x *= -1;
            prop.transform.localScale = scale;
        }

        float speed = Random.Range(minSpeed, maxSpeed);


        speed *= facingLeft ? -1f : 1f;

        prop.velocity = new Vector2(speed, 0);


        //递归调用后台生成函数
        StartCoroutine(Spawn());

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

//发射子弹脚本，需要创建一个空对象挂载这个脚本，为子弹的实例化提供位置参数

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    //获取子弹的Prefab
    public Rigidbody2D rocket;
    //public GameObject rocket;
    public float speed = 20f;

    private PlayerControl playerCtrl;
    private Animator animator;
    private AudioSource audioSource;

    private void Awake() {
        //为什么这里是transform.root.gameObject和transform.root的效果一样
        animator = transform.root.gameObject.GetComponent<Animator>();
        
        playerCtrl = transform.root.gameObject.GetComponent<PlayerControl>();

        audioSource = GetComponent<AudioSource>();
    }

    void Update () {
        if (Input.GetButtonDown("Fire1")) {
            //因为播放射击动画的状态是从AnyState触发的，所以需要使用SetTrigger，触发之后自动重置，避免多次循环
            animator.SetTrigger("Shoot");
            //播放射击音效
            audioSource.Play();

            //根据角色当前的朝向实例化导弹
            if (playerCtrl.facingRight) {
                Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                bulletInstance.velocity = new Vector2(speed, 0);
                //Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)))
                //    .GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
            } else {
                Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0, 0, 180))) as Rigidbody2D;
                bulletInstance.velocity = new Vector2(-speed, 0);
                //Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0, 0, 180)))
                //    .GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0);
            }
        }
	}
}

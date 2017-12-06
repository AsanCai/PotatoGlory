using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {
    //判断是否要在awake被销毁
    public bool destroyOnAwake;
    //销毁延迟时间，默认立即销毁
    public float awakeDestroyDelay;
    //检测是销毁子对象还是销毁自身
    public bool findChild = false;
    //保存子对象的名字
    public string namedChild;


    private void Awake() {
        //判断对象是否需要在awake里面被销毁
        if (destroyOnAwake) {
            if (findChild) {
                //销毁指定的子对象，立即销毁
                Destroy(transform.Find(namedChild).gameObject);
            } else {
                //销毁自身，awakeDestroyDelay销毁
                Destroy(gameObject, awakeDestroyDelay);
            }
        }
    }

    //下面三个函数暂时没用到
    void DestroyChildGameObject() {
        if (transform.Find(namedChild).gameObject != null)
            Destroy(transform.Find(namedChild).gameObject);
    }

    void DisableChildGameObject() {
        if(transform.Find(namedChild).gameObject.activeSelf == true) {
            transform.Find(namedChild).gameObject.SetActive(false);
        }
    }

    void DestroyGameObject() {

        Destroy(gameObject);
    }
}

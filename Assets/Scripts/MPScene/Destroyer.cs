using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsanCai.MPScene {
    public class Destroyer : MonoBehaviour {

        [Tooltip("是否要在对象Awake被销毁")]
        public bool destroyOnAwake;
        [Tooltip("销毁延迟时间，默认立即销毁")]
        public float awakeDestroyDelay;
        [Tooltip("false销毁子对象，true销毁自身")]
        public bool findChild = false;
        [Tooltip("需要销毁的子对象名字")]
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

        //下面三个函数在Animation Event中被用到
        void DestroyChildGameObject() {
            if (transform.Find(namedChild).gameObject != null)
                Destroy(transform.Find(namedChild).gameObject);
        }

        void DisableChildGameObject() {
            if (transform.Find(namedChild).gameObject.activeSelf == true) {
                transform.Find(namedChild).gameObject.SetActive(false);
            }
        }

        //销毁自身
        void DestroyGameObject() {

            Destroy(gameObject);
        }
    }
}

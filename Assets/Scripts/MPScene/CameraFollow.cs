using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsanCai.MPScene {
    public class CameraFollow : MonoBehaviour {

        //摄像机中心和角色位置的最大偏移量
        public float xMargin = 2f;
        public float yMargin = 2f;
        //在跟随角色移动时被用来进行插值
        public float xSmooth = 2f;
        public float ySmooth = 2f;
        //镜头可移动的范围
        public Vector2 maxXAndY;
        public Vector2 minXAndY;

        private Transform player;

        private void OnEnable() {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        bool CheckXMargin() {
            return Mathf.Abs(transform.position.x - player.position.x) > xMargin;
        }

        bool CheckYMargin() {
            return Mathf.Abs(transform.position.y - player.position.y) > yMargin;
        }

        private void LateUpdate() {
            TrackPlayer();
        }


        void TrackPlayer() {
            float targetX = transform.position.x;
            float targetY = transform.position.y;

            if (CheckXMargin()) {
                targetX = Mathf.Lerp(transform.position.x, player.position.x, xSmooth * Time.deltaTime);
            }

            if (CheckYMargin()) {
                targetY = Mathf.Lerp(transform.position.y, player.position.y, ySmooth * Time.deltaTime);
            }


            targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
            targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);

            transform.position = new Vector3(targetX, targetY, transform.position.z);
        }
    }
}

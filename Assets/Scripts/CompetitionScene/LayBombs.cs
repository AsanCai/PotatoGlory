using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

using Photon;

namespace AsanCai.Competition {
    public class LayBombs : PunBehaviour{

        [HideInInspector]
        public bool bombLaid = false;

        [Tooltip("可放置炸弹的数量")]
        public int bombCount = 0;
        [Tooltip("放置炸弹的音效")]
        public AudioClip bombsAway;


        private void Update() {
            if (CrossPlatformInputManager.GetButtonDown("Fire2")) {
                if (!bombLaid && bombCount > 0) {
                    bombCount--;

                    bombLaid = true;

                    AudioSource.PlayClipAtPoint(bombsAway, transform.position);

                    PhotonNetwork.Instantiate("bomb", transform.position, 
                        Quaternion.Euler(new Vector3(0, 0, 0)), 0);
                }
            }
        }
    }
}
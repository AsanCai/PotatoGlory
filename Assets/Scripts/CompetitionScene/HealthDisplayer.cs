//用于显示当前血条

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsanCai.Competition {
    public class HealthDisplayer : MonoBehaviour {
        [Tooltip("血量条图片")]
        public SpriteRenderer healthBar;

        private int previousHP;
        private int currentHP;

        private PlayerHealth playerHealth;

        // Use this for initialization
        void Start() {
            playerHealth = transform.parent.GetComponent<PlayerHealth>();
        }

        // Update is called once per frame
        void Update() {

        }
        
    }
}

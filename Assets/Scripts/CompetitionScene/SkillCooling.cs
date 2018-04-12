using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace AsanCai.Competition {
    public class SkillCooling : MonoBehaviour {
        [Tooltip("激活按钮使用的名字")]
        public string Name;
        [Tooltip("按钮松开时显示的图片")]
        public Sprite normalImg;
        [Tooltip("按钮被按下时显示的图片")]
        public Sprite activeImg;
        [Tooltip("按钮被禁用时显示的图片")]
        public Image banImg;
        [Tooltip("冷却图片")]  
        public Image coolingMask;

        //是否需要冷却
        private bool needCooled = false;
        //冷却时间
        private float coolingTime;
        //计时器
        private float timer;
        //按钮图片引用
        private Image img;
        //按钮是否被禁用
        private bool ban = false;

        void OnEnable() {
            timer = coolingTime;

            img = GetComponent<Image>();
        }

        //按钮被按下
        public void SetDownState() {
            //技能冷却或者被禁用时，按按钮应该没有反应
            if (needCooled || ban) {
                return;
            }

            CrossPlatformInputManager.SetButtonDown(Name);
            img.sprite = activeImg;
        }

        //按钮被松开
        public void SetUpState() {
            CrossPlatformInputManager.SetButtonUp(Name);
            img.sprite = normalImg;
        }

        //启用按钮
        public void SetAxisPositiveState() {
            CrossPlatformInputManager.SetAxisPositive(Name);

            banImg.enabled = false;
            ban = false;
        }


        public void SetAxisNeutralState() {
            CrossPlatformInputManager.SetAxisZero(Name);
        }

        //禁用按钮
        public void SetAxisNegativeState() {
            CrossPlatformInputManager.SetAxisNegative(Name);

            banImg.enabled = true;
            ban = true;
        }

      
        // Update is called once per frame  
        void Update() {
            if (!needCooled) {
                return;
            }

            timer += Time.deltaTime;
            //按时间比例计算出Fill Amount值  
            coolingMask.fillAmount = 1 - timer / coolingTime;

            if(timer > coolingTime) {
                needCooled = false;
            }
        }

        //开启技能冷却
        public void OnBtnClickSkill(float time) {
            needCooled = true;

            timer = 0.0f;

            coolingTime = time;

            coolingMask.fillAmount = 1.0f;

            img.sprite = normalImg;
        }
    }
    
}
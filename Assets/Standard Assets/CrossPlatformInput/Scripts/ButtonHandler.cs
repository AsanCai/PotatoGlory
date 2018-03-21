using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class ButtonHandler : MonoBehaviour
    {
        [Tooltip("激活按钮使用的名字")]
        public string Name;
        [Tooltip("按钮松开时显示的图片")]
        public Sprite normalImg;
        [Tooltip("按钮被按下时显示的图片")]
        public Sprite activeImg;

        private Image img;

        void OnEnable()
        {
            img = GetComponent<Image>();
        }

        public void SetDownState()
        {
            CrossPlatformInputManager.SetButtonDown(Name);
            img.sprite = activeImg;
        }


        public void SetUpState()
        {
            CrossPlatformInputManager.SetButtonUp(Name);
            img.sprite = normalImg;
        }


        public void SetAxisPositiveState()
        {
            CrossPlatformInputManager.SetAxisPositive(Name);
        }


        public void SetAxisNeutralState()
        {
            CrossPlatformInputManager.SetAxisZero(Name);
        }


        public void SetAxisNegativeState()
        {
            CrossPlatformInputManager.SetAxisNegative(Name);
        }

        public void Update()
        {

        }
    }
}

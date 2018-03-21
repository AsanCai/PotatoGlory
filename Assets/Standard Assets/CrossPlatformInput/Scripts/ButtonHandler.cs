using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class ButtonHandler : MonoBehaviour
    {
        [Tooltip("���ťʹ�õ�����")]
        public string Name;
        [Tooltip("��ť�ɿ�ʱ��ʾ��ͼƬ")]
        public Sprite normalImg;
        [Tooltip("��ť������ʱ��ʾ��ͼƬ")]
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;


public class VirtualJoyStick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {

    private Image bgImg;
    private Image stickImg;

    public Vector2 InputDirection { get; set; }
    

    private void Start() {
        bgImg = GetComponent<Image>();

        stickImg = transform.Find("stick").gameObject.GetComponent<Image>();

        InputDirection = Vector2.zero;

        //Jump = false;
    }

    //实现上面三个接口的函数
    public virtual void OnDrag(PointerEventData ped) {
        Vector2 pos = Vector2.zero;

        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos)) {
            pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);


            float x = pos.x;
            float y = pos.y;

            InputDirection = new Vector2(x, y);
            InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

            stickImg.rectTransform.anchoredPosition = new Vector2(InputDirection.x * (bgImg.rectTransform.sizeDelta.x / 3),
                InputDirection.y * (bgImg.rectTransform.sizeDelta.y / 3));

            //if (Mathf.Abs(InputDirection.x) < 0.1 && InputDirection.y > 0.5)
            //    Jump = true;
        }
    }

    public virtual void OnPointerDown(PointerEventData ped) {
        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped) {
        InputDirection = Vector2.zero;

        stickImg.rectTransform.anchoredPosition = Vector2.zero;
    }
}

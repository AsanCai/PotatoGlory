using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public Sprite active;
    public Sprite normal;

    private Image img;

    // Use this for initialization
    void Start() {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    public virtual void OnPointerDown(PointerEventData ped) {

        img.sprite = active;
    }

    public virtual void OnPointerUp(PointerEventData ped) {

        img.sprite = normal;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class Fire : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public Sprite active;
    public Sprite normal;

    private Gun gun;
    private Image img;

    private void Start() {
        gun = GameObject.Find("Gun").GetComponent<Gun>();

        img = GetComponent<Image>();
    }

    public virtual void OnPointerDown(PointerEventData ped) {

        img.sprite = active;
        gun.Fire();
    }

    public virtual void OnPointerUp(PointerEventData ped) {

        img.sprite = normal;

    }
}

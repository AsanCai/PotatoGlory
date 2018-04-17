using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Jump : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public bool pressDown { set; get; }

    private void Start() {
        pressDown = false;
    }

    public virtual void OnPointerDown(PointerEventData ped) {

        pressDown = true;
    }

    public virtual void OnPointerUp(PointerEventData ped) {

        pressDown = false;

    }
}

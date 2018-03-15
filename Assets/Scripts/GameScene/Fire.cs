using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class Fire : MonoBehaviour, IPointerDownHandler {

    

    private Gun gun;

    private void Start() {
        gun = GameObject.Find("Gun").GetComponent<Gun>();
    }

    public virtual void OnPointerDown(PointerEventData ped) {

        gun.Fire();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pauser : MonoBehaviour {

    private bool paused = false;

    private void Update() {
        if (Input.GetKeyUp(KeyCode.P)) {
            paused = !paused;
        }

        if (paused) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }
}

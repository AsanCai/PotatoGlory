using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreShadow : MonoBehaviour {

    private Text score;
    private Text shadow;

    private void Awake() {
        score = GameObject.Find("Score").GetComponent<Text>();

        shadow = GetComponent<Text>();
    }

    private void Update() {
        shadow.text = score.text;
    }
}

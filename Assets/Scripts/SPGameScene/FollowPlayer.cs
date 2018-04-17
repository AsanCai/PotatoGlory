using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Vector3 offset;

    private Transform player;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        //一直保持offset的距离
        transform.position = player.position + offset;
    }
}

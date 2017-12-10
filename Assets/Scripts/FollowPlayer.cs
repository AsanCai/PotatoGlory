using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Vector3 offset;

    private Transform player;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    //当角色死了之后，这里会疯狂报错
    private void Update() {
        //一直保持offset的距离
        transform.position = player.position + offset;
    }
}

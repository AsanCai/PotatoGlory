//当角色移动的时候，让背景根据镜头的偏移来左右调整位置，产生一种相对运动感

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour {

    public Transform[] backgrounds;

    //设置背景移动的幅度
    public float parallaxScale;
    //设置不同层次的背景移动的级差
    public float parallxReductionFactor;
    //用于插值
    public float smoothing;

    //用于检测摄像机的位置
    private Transform cam;
    //用于判断摄像机是否移动了
    private Vector3 previousCamPos;

    private void Awake() {
        cam = Camera.main.transform;
    }

    void Start () {
        previousCamPos = cam.position;
	}
	
	void Update () {
        
        float parallax = (previousCamPos.x - cam.position.x) * parallaxScale;

        for (int i = 0; i < backgrounds.Length; i++) {
            //背景离得越近，移动的幅度越大，产生远景效果
            //因此要注意添加顺序
            float backgroundTargetPosX = backgrounds[i].position.x + parallax * (i * parallxReductionFactor + 1);

            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
        }

        //更新摄像机的位置
        previousCamPos = cam.position;

	}
}

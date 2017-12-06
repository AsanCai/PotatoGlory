//物体掉落进河流里执行的脚本

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Remover : MonoBehaviour {

    public GameObject splash;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player") {
            //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().enable = false;
        } else {
            //实例化水花对象，水花对象会自动播放声音和动画
            Instantiate(splash, collision.transform.position, transform.rotation);
            //销毁掉下去的物体
            Destroy(collision.gameObject);
        }
    }

    IEnumerator ReloadGame() {
        yield return new WaitForSeconds(2);
        //重新加载游戏
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}

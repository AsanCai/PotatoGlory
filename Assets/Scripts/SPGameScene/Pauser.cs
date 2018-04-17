using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pauser : MonoBehaviour {
    [Tooltip("确认退出面板")]
    public GameObject confirmPanel;

    private void Start() {
        confirmPanel.SetActive(false);
    }

    public void ClickBackButton() {
        Time.timeScale = 0;

        confirmPanel.SetActive(true);
    }

    public void ClickConfirmButton() {
        SceneManager.LoadScene("StartScene");
    }

    public void ClickCancelButton() {
        Time.timeScale = 1;

        confirmPanel.SetActive(false);
    }
}

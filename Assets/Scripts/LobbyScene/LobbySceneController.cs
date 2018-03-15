using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySceneController : MonoBehaviour {
    [Tooltip("大厅面板")]
    public GameObject lobbyPanel;

	// Use this for initialization
	void Start () {
        if (!lobbyPanel.activeSelf) {
            lobbyPanel.SetActive(true);
        }
	}

}

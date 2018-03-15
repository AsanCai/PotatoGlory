/*********************
 * 管理用户登录界面 *
 *********************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon;

//需要继承PunBehaviour才能正常使用PUN提供的api
public class StartMenuController : PunBehaviour {

    [Tooltip("游戏版本号")]
    public string version;
    [Tooltip("选项面板")]
    public GameObject optionPanel;
    [Tooltip("登录面板")]
    public GameObject loginPanel;
    [Tooltip("当前网络链接状态信息")]
    public Text connectionState;
    [Tooltip("用户名")]
    public Text username;
    [Tooltip("密码")]
    public Text password;

    void Start () {
        
        if (!PhotonNetwork.connected) {
            //若当前网络尚未链接，那么显示选项面板
            SetOptionPanelActive();
        } else {
            //如果已经连接到服务器了，那么显示游戏大厅
            SceneManager.LoadScene("LobbyScene");
        }

        //初始化网络连接状态文本信息
        connectionState.text = "";  
    }

 
    void Update() {
//条件编译指令，只在Unity编辑器中（UNITY_EDITOR）编译此段代码
#if (UNITY_EDITOR)
        //在游戏画面左下角显示当前的网络连接状态
        connectionState.text = PhotonNetwork.connectionStateDetailed.ToString();
#endif

        //当链接上服务器时加载大厅场景
        if (PhotonNetwork.connected) {
            SceneManager.LoadScene("LobbyScene");
        }
    }


    //启用选项面板
    public void SetOptionPanelActive() {
        optionPanel.SetActive(true);

        loginPanel.SetActive(false);
    }

    //启用登录面板
    public void SetLoginPanelActive() {
        loginPanel.SetActive(true);

        optionPanel.SetActive(false);
    }

    


    //点击多人游戏按钮
    public void ClickMPButton() {
        SetLoginPanelActive();
    }

    //点击单人游戏按钮
    public void ClickSPButton() {
        SceneManager.LoadScene("SPGameScene");
    }

    //点击退出按钮
    public void ClickExitButton() {
        //退出游戏应用
        Application.Quit();         
    }

    //点击登录按钮
    public void ClickLoginButton() {
        //连接到客户端服务器
        if (!PhotonNetwork.connected) {
            PhotonNetwork.ConnectUsingSettings(version);
        }

        //用于获取玩家输入的用户名
        //这里使用name变量是因为好像不能直接修改username.text
        string name;

        //如果玩家未输入昵称，这里自动为其分配一个昵称
        if (username.text == "") {
            name = "游客" + Random.Range(1, 9999);
        } else {
            name = username.text;
        }

        //设置玩家昵称
        PhotonNetwork.player.NickName = name;
        //将玩家昵称保存在本地
        PlayerPrefs.SetString("Username", name);
        Debug.Log(PhotonNetwork.player.NickName);
    }

    //点击返回按钮
    public void ClickBackButton() {
        SetOptionPanelActive();
    }
}

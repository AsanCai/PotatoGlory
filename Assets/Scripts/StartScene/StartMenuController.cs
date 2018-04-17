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
    [Tooltip("注册面板")]
    public GameObject registerPanel;
    [Tooltip("展示物体")]
    public GameObject displayObject;
    [Tooltip("提示当前状态")]
    public Text resultText;

    [Tooltip("登录面板的用户输入框")]
    public InputField loginUsername;
    [Tooltip("登录面板的密码输入框")]
    public InputField loginPassword;
    [Tooltip("注册面板的用户输入框")]
    public InputField registerUsername;
    [Tooltip("注册面板的密码输入框")]
    public InputField registerPassword;
    [Tooltip("注册面板的重复密码输入框")]
    public InputField registerRepeatPassword;

    void Start() {
        if (!PhotonNetwork.connected) {
            //若当前网络尚未链接，那么显示选项面板
            SetOptionPanelActive();
            //初始化
            ClientManager.cm.Init();
        } else {
            //断开登录服务器链接
            ClientManager.cm.Disconnect();

            //已经连接到大厅服务器，显示游戏大厅
            SceneManager.LoadScene("LobbyScene");
        }

        if(ClientManager.cm.state == ClientManager.State.disconnected) {
            ClientManager.cm.Connection();
        }
    }


    void Update() {
        resultText.text = ClientManager.cm.recvStr;

        //当链接上服务器时加载大厅场景
        if (PhotonNetwork.connected) {
            ClientManager.cm.Disconnect();
            SceneManager.LoadScene("LobbyScene");
            return;
        }

        string username;
        switch (ClientManager.cm.state) {
            case ClientManager.State.login:
                username = loginUsername.text.Trim();
                Login(username);
                break;
            case ClientManager.State.register:
                username = registerUsername.text.Trim();
                Login(username);
                break;
            default:
                break;
        }
    }


    //启用选项面板
    private void SetOptionPanelActive() {
        optionPanel.SetActive(true);
        displayObject.SetActive(true);

        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
    }

    //启用登录面板
    private void SetLoginPanelActive() {
        loginPanel.SetActive(true);
        displayObject.SetActive(true);

        optionPanel.SetActive(false);
        registerPanel.SetActive(false);
    }

    private void SetRegisterPanelActive() {
        registerPanel.SetActive(true);

        displayObject.SetActive(false);
        optionPanel.SetActive(false);
        loginPanel.SetActive(false);
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

    //点击注册按钮
    public void ClickRegisterButton() {
        SetRegisterPanelActive();
    }

    //点击登录按钮
    public void ClickLoginButton() {
        string username = loginUsername.text;
        string password = loginPassword.text;

        ClientManager.cm.Login(username, password);
    }

    //点击确认按钮，确认注册
    public void ClickConfirmButton() {
        string username = registerUsername.text;
        string password = registerPassword.text;
        string repeatPassword = registerRepeatPassword.text;

        ClientManager.cm.Register(username, password, repeatPassword);
    }

    //点击返回按钮
    public void ClickBackButton() {
        //清空输入框
        loginUsername.text = "";
        loginPassword.text = "";
        registerUsername.text = "";
        registerPassword.text = "";
        registerRepeatPassword.text = "";

        SetOptionPanelActive();
    }

    private void Login(string username) {
        //连接到客户端服务器
        if (!PhotonNetwork.connected) {
            PhotonNetwork.ConnectUsingSettings(version);
        }

        //设置玩家昵称
        PhotonNetwork.player.NickName = username;
        //将玩家昵称保存在本地
        PlayerPrefs.SetString("Username", username);

        //重置为连接状态
        ClientManager.cm.ResetState();
    }
}

/*********************
 用于管理游戏大厅
**********************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon;

public class LobbyPanelController : PunBehaviour {

    [Tooltip("当前网络链接状态信息")]
    public Text connectionState;
    [Tooltip("用户信息")]
    public Text userMessage;
    [Tooltip("房间信息面板")]
    public GameObject roomMessagePanel;
    
    [Tooltip("上一页按钮")]
    public GameObject previousButton;
    [Tooltip("下一页按钮")]
    public GameObject nextButton;
    [Tooltip("快速开始按钮")]
    public Button QuickGameButton;

    [Tooltip("分页信息")]
    public Text pageMessage;

    [Tooltip("游戏大厅面板")]
    public GameObject lobbyPanel;
    [Tooltip("创建房间面板")]
    public GameObject createRoomPanel;
    [Tooltip("进入房间面板")]
    public GameObject roomPanel;


    //游戏大厅房间列表信息
    private RoomInfo[] roomInfo;
    //当前房间页
    private int currentPageNumber;
    //最大房间页
    private int maxPageNumber;
    //每页显示房间个数
    private int roomPerPage = 4;
    //游戏房间信息
    private GameObject[] roomMessage;		

    //只要是可见状态，OnEnable就会被触发
    void OnEnable () {
        //初始化网络连接状态文本信息
        connectionState.text = "";
        //初始化用户名
        userMessage.text = "";
        //初始化当前房间页
        currentPageNumber = 1;
        //初始化最大房间页	
        maxPageNumber = 1;


        //禁用创建房间面板
        if (createRoomPanel != null) {
            createRoomPanel.SetActive(false);
        }
        //禁用游戏房间面板
        if (roomPanel != null) {
            roomPanel.SetActive(false);
        }
            			

        //获取房间信息面板
        RectTransform rectTransform = roomMessagePanel.GetComponent<RectTransform>();
        //获取房间信息面板的条目数
        roomPerPage = rectTransform.childCount;


        //初始化每条房间信息条目
        roomMessage = new GameObject[roomPerPage];
        for (int i = 0; i < roomPerPage; i++) {
            roomMessage[i] = rectTransform.GetChild(i).gameObject;
            //禁用每个房间信息条目
            roomMessage[i].SetActive(false);            
        }
    }

    //条件编译指令，只在Unity编辑器中（UNITY_EDITOR）编译此段代码
#if (UNITY_EDITOR)
    void Update() {
        //在游戏画面左下角显示当前的网络连接状态
        connectionState.text = PhotonNetwork.connectionStateDetailed.ToString();
    }
#endif


    //显示房间信息
    private void ShowRoomMessage() {
        int start, end, i, j;
        //计算需要显示房间信息的起始序号
        start = (currentPageNumber - 1) * roomPerPage;
        //计算需要显示房间信息的末尾序号
        if (currentPageNumber * roomPerPage < roomInfo.Length)  
            end = currentPageNumber * roomPerPage;
        else
            end = roomInfo.Length;

        //依次显示每条房间信息
        for (i = start, j = 0; i < end; i++, j++) {
            RectTransform rectTransform = roomMessage[j].GetComponent<RectTransform>();
            //获取房间名称
            string roomName = roomInfo[i].Name;
            
            //显示房间序号
            rectTransform.GetChild(0).GetComponent<Text>().text = (i + 1).ToString();
            //显示房间名称
            rectTransform.GetChild(1).GetComponent<Text>().text = roomName;
            //显示游戏模式
            string mode = (string)roomInfo[i].CustomProperties["GameMode"];
            if(mode == "Competition") {
                rectTransform.GetChild(2).GetComponent<Text>().text = "竞技";
            } else {
                rectTransform.GetChild(2).GetComponent<Text>().text = "闯关";
            }
            


            //获取"进入房间"按钮组件
            Button button = rectTransform.GetChild(3).GetComponent<Button>();

            
            if (roomInfo[i].PlayerCount == roomInfo[i].MaxPlayers 
                || roomInfo[i].IsOpen == false) {
                //如果游戏房间人数已满，或者游戏房间的Open属性为false（房间内游戏已开始），
                //表示房间无法加入，禁用"进入房间"按钮
                button.gameObject.SetActive(false);

            } else {
                //如果房间可以加入，启用"进入房间"按钮，给按钮绑定新的监听事件，加入此房间
                button.gameObject.SetActive(true);
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate () {
                    //加入指定的房间
                    ClickJoinRoomButton(roomName);
                });
            }
            //启用房间信息条目
            roomMessage[j].SetActive(true); 
        }
        //禁用不显示的房间信息条目
        while (j < 4) {
            roomMessage[j++].SetActive(false);
        }
    }

    //翻页按钮控制函数
    private void ButtonController() {
        //如果当前页为1，禁用"上一页"按钮；否则，启用"上一页"按钮
        if (currentPageNumber == 1)
            previousButton.SetActive(false);
        else
            previousButton.SetActive(true);

        //如果当前页等于房间总页数，禁用"下一页"按钮；否则，启用"下一页"按钮
        if (currentPageNumber == maxPageNumber)
            nextButton.SetActive(false);
        else
            nextButton.SetActive(true);
    }

    #region 按钮事件
    //"返回"按钮事件处理函数
    public void ClickBackButton() {
        //断开与服务器的连接
        PhotonNetwork.Disconnect();
        //加载游戏开始界面
        SceneManager.LoadScene("StartScene");
    }

    //"上一页"按钮事件处理函数
    public void ClickPreviousButton() {
        //当前房间页减一
        currentPageNumber--;
        //更新房间页数显示
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();   
        //当前房间页更新，调动翻页控制函数
        ButtonController();
        //当前房间页更新，重新显示房间信息
        ShowRoomMessage();          
    }
    //"下一页"按钮事件处理函数
    public void ClickNextButton() {
        //当前房间页加一
        currentPageNumber++;        
        //更新房间页数显示
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();   
        //当前房间页更新，调动翻页控制函数
        ButtonController();
        //当前房间页更新，重新显示房间信息
        ShowRoomMessage();          
    }

    //"创建房间"按钮事件处理函数
    public void ClickCreateRoomButton() {
        createRoomPanel.SetActive(true);
        roomPanel.SetActive(false);
    }

    //"快速开始"按钮时间处理函数
    public void ClickQuickGameButton() {
        //随机加入房间
        PhotonNetwork.JoinRandomRoom();
    }

    //"进入房间"按钮事件处理函数
    public void ClickJoinRoomButton(string roomName) {
        //根据房间名加入游戏房间
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion


    #region IPunCallback回调函数

    /****************************************************************
     * 覆写IPunCallback回调函数，当玩家进入游戏大厅时调用显示玩家昵称
     * 需要在PhotonServerSettings里勾选Auto-join Lobby
	 ****************************************************************/
    public override void OnJoinedLobby() {
        userMessage.text = PhotonNetwork.player.NickName;
    }

    /****************************************************************
     * 覆写IPunCallback回调函数，当玩家进入游戏房间时调用
	 * 禁用游戏大厅面板，启用游戏房间面板
	 ****************************************************************/
    public override void OnJoinedRoom() {
        roomPanel.SetActive(true);

        lobbyPanel.SetActive(false);
        createRoomPanel.SetActive(false);
    }

    /*****************************************************************
     * 覆写IPunCallback回调函数，当客户端连接到MasterServer时调用
	 * 加入默认游戏大厅
	 * 效果等同于勾选PhotonServerSettings中的Auto-join Lobby
     * ***************************************************************/
    //public override void OnConnectedToMaster (){
    //	PhotonNetwork.JoinLobby ();
    //}


    /*****************************************************************
     * 覆写IPunCallback回调函数，当房间列表更新时调用
	 * 更新游戏大厅中房间列表的显示
	 *****************************************************************/
    public override void OnReceivedRoomListUpdate() {
        //获取游戏大厅中的房间列表
        roomInfo = PhotonNetwork.GetRoomList();
        //计算房间总页数
        maxPageNumber = (roomInfo.Length - 1) / roomPerPage + 1;
        //如果当前页大于房间总页数时
        if (currentPageNumber > maxPageNumber) {
            //将当前房间页设为房间总页数
            currentPageNumber = maxPageNumber;
        }

        //更新房间页数信息的显示
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();
        //翻页按钮控制
        ButtonController();
        //显示房间信息
        ShowRoomMessage();      

        
        if (roomInfo.Length == 0) {
            //如果房间数为0，禁用"随机进入房间"按钮的交互功能
            QuickGameButton.interactable = false;  
        } else {
            //如果房间数不为0，启用"随机进入房间"按钮的交互功能
            QuickGameButton.interactable = true;
        }
    }

    /*****************************************************************
     * 覆写IPunCallback回调函数，当客户端断开与Photon服务器的连接时调用
	 * 返回开始场景
	 *****************************************************************/
    public override void OnConnectionFail(DisconnectCause cause) {
        SceneManager.LoadScene("StartScene");
    }
    #endregion
}

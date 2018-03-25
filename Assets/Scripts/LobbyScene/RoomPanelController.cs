using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class RoomPanelController : PunBehaviour {
    [Tooltip("游戏大厅面板")]
    public GameObject lobbyPanel;
    [Tooltip("游戏房间面板")]
    public GameObject roomPanel;
    [Tooltip("房间名称")]
    public Text roomName;
    [Tooltip("游戏模式")]
    public Text gameMode;
    [Tooltip("准备/开始游戏面板")]
    public Button readyButton;
    [Tooltip("提示信息")]
    public Text promptMessage;
    [Tooltip("提示信息持续时间")]
    public float durationTime = 2.5f;

    [Tooltip("玩家1信息")]
    public GameObject playerMessage1;
    [Tooltip("玩家2信息")]
    public GameObject playerMessage2;


    private PhotonView pView;
    private Text[] texts;
    private string mode;
    ExitGames.Client.Photon.Hashtable costomProperties;


    private void Start() {
        pView = GetComponent<PhotonView>();
    }

    void OnEnable () {
        if (!PhotonNetwork.connected) {
            return;
        }

        //初始化提示信息
        promptMessage.text = "";
        //初始化房间名
        roomName.text = "房间名：" + PhotonNetwork.room.Name;
        //初始化游戏模式
        mode = (string)PhotonNetwork.room.CustomProperties["GameMode"];
        if(mode == "Competition") {
            gameMode.text = "模式：竞技模式";
        } else {
            gameMode.text = "模式：闯关模式";
        }

        //初始化队伍面板
        DisableTeamPanel();
        //更新队伍面板（false表示不显示本地玩家信息，只显示其他玩家的信息）
        UpdateTeamPanel(false);


        /**********************************************************
         * 从左到右寻找两边空余位置，将玩家信息放置在对应空位置中
         **********************************************************/
        //在玩家1找到空余位置
        if (!playerMessage1.activeSelf) {
            //激活对应的玩家信息UI
            playerMessage1.SetActive(true);       
            texts = playerMessage1.GetComponentsInChildren<Text>();
            //显示玩家昵称
            texts[0].text = PhotonNetwork.playerName;
            if (PhotonNetwork.isMasterClient) {
                //如果玩家是MasterClient，玩家状态显示"房主"
                texts[1].text = "房主"; 
            } else {
                //如果玩家不是MasterClient，玩家状态显示"未准备"
                texts[1].text = "未准备";                         
            }

            //初始化玩家自定义属性
            costomProperties = new ExitGames.Client.Photon.Hashtable() {	
				{ "Player","Player1" },		//玩家队伍
				{ "isReady",false },	//玩家准备状态
				{ "Score",0 },			//玩家得分
                {"isAlive", true }      //玩家是否存活
			};
            //将玩家自定义属性赋予玩家
            PhotonNetwork.player.SetCustomProperties(costomProperties); 
        } else if (!playerMessage2.activeSelf) {  //在队伍2找到空余位置
            //激活对应的队伍信息UI
            playerMessage2.SetActive(true);
            //显示玩家昵称
            texts = playerMessage2.GetComponentsInChildren<Text>();

            if (PhotonNetwork.isMasterClient) {
                //如果玩家是MasterClient，玩家状态显示"房主"
                texts[1].text = "房主"; 
            } else {
                //如果玩家不是MasterClient，玩家状态显示"未准备"
                texts[1].text = "未准备";                        
            }

            //初始化玩家自定义属性
            costomProperties = new ExitGames.Client.Photon.Hashtable() {	
				{ "Player","Player2" },		//玩家队伍
				{ "isReady",false },	//玩家准备状态
				{ "Score",0 },			//玩家得分
                {"isAlive", true }      //玩家是否存活
			};

            //将玩家自定义属性赋予玩家
            PhotonNetwork.player.SetCustomProperties(costomProperties);
        }


        ReadyButtonControl();
    }

    #region 公用函数
    //禁用玩家信息面板，清空当前玩家信息面板
    void DisableTeamPanel() {
        //禁用玩家1信息面板
        playerMessage1.SetActive(false);
        //禁用玩家2信息面板
        playerMessage2.SetActive(false);
    }

    /****************************************************************
     * 在队伍面板显示玩家信息
	 * 函数参数表示是否更新本地玩家信息
	 ****************************************************************/
    void UpdateTeamPanel(bool isUpdateSelf) {
        GameObject go;
        //获取房间里所有玩家信息
        foreach (PhotonPlayer p in PhotonNetwork.playerList) {
            //判断是否更新本地玩家信息
            if (!isUpdateSelf && p.IsLocal) {
                continue;
            }

            //获取玩家自定义属性
            costomProperties = p.CustomProperties;

            //判断玩家所属队伍
            if (costomProperties["Player"].Equals("Player1")) { 
                go = playerMessage1;
            } else {
                go = playerMessage2;
            }

            //激活显示玩家信息的UI
            go.SetActive(true);
            //获取显示玩家信息的Text组件
            texts = go.GetComponentsInChildren<Text>();

            //显示玩家姓名
            texts[0].text = p.NickName;

            //如果玩家是MasterClient
            if (p.IsMasterClient) {
                //玩家状态显示"房主"
                texts[1].text = "房主";
            } else {
                //如果玩家不是MasterClient，获取玩家的准备状态isReady
                if ((bool)costomProperties["isReady"]) {
                    //isReady为true，显示"已准备"
                    texts[1].text = "已准备";                  
                } else {
                    //isReady为false，显示"未准备"
                    texts[1].text = "未准备";
                }
            }
        }
    }

    /**********************************************************************
     * 动态给ReadyButton按钮绑定事件
     * 所以不需要在属性面板给按钮绑定事件
     **********************************************************************/
    void ReadyButtonControl() {
        //如果玩家是MasterClient
        if (PhotonNetwork.isMasterClient) {
            //ReadyButton显示"开始游戏"
            readyButton.GetComponentInChildren<Text>().text = "开始游戏";
            //移除ReadyButton所有监听事件
            readyButton.onClick.RemoveAllListeners();
            //为ReadyButton绑定新的监听事件
            readyButton.onClick.AddListener(delegate () {
                //开始游戏
                ClickStartGameButton();                                 
            });
        } else {                                                            
            if ((bool)PhotonNetwork.player.CustomProperties["isReady"]) {
                //根据玩家准备状态显示对应的文本信息
                readyButton.GetComponentInChildren<Text>().text = "取消准备";
            } else {
                readyButton.GetComponentInChildren<Text>().text = "准备";
                //移除ReadyButton所有监听事件
                readyButton.onClick.RemoveAllListeners();
                //为ReadyButton绑定新的监听事件
                readyButton.onClick.AddListener(delegate () {
                    //切换准备状态
                    ClickReadyButton();                                     
                });
            }
                
        }
    }

    /**********************************************************************
     * 在durationTime之后重置提示信息
     **********************************************************************/
    IEnumerator ResetPromtMessage() {
        yield return new WaitForSeconds(durationTime);

        promptMessage.text = "";
    }

    #endregion


    #region 按钮事件处理函数

    //准备按钮事件响应函数
    void ClickReadyButton() {
        //获取玩家准备状态
        bool isReady = (bool)PhotonNetwork.player.CustomProperties["isReady"];
        //重新设置玩家准备状态
        costomProperties = new ExitGames.Client.Photon.Hashtable() { { "isReady", !isReady } }; 
        PhotonNetwork.player.SetCustomProperties(costomProperties);

        //获取ReadyButton的按钮文本
        Text readyButtonText = readyButton.GetComponentInChildren<Text>();

        //根据玩家点击按钮后的状态，设置按钮文本的显示
        if (isReady) {
            readyButtonText.text = "准备";
        } else {
            readyButtonText.text = "取消准备";
        }
    }

    //开始游戏按钮事件响应函数
    void ClickStartGameButton() {
        if (PhotonNetwork.playerList.Length < PhotonNetwork.room.MaxPlayers) {
            //提示信息显示"有人未准备，游戏无法开始"
            promptMessage.text = "玩家数不足，游戏无法开始";
            StartCoroutine(ResetPromtMessage());
            return;
        }

        //遍历房间内所有玩家
        foreach (PhotonPlayer p in PhotonNetwork.playerList) {
            //不检查MasterClient房主的准备状态
            if (p.IsLocal) {
                continue;
            }

            //如果有其他玩家未准备
            if ((bool)p.CustomProperties["isReady"] == false) {
                //提示信息显示"有人未准备，游戏无法开始"
                promptMessage.text = "有人未准备，游戏无法开始";
                StartCoroutine(ResetPromtMessage());
                return;                                             
            }
        }

        //所有玩家都准备了，清空提示信息
        promptMessage.text = "";
        //设置房间的open属性，使游戏大厅的玩家无法加入此房间
        PhotonNetwork.room.IsOpen = false;

        //调用RPC，让游戏房间内所有玩家加载场景GameScene，开始游戏
        pView.RPC("LoadGameScene", PhotonTargets.All, mode + "Scene"); 
    }

    //点击离开房间按钮
    public void ClickExitButton() {
        //客户端离开游戏房间
        PhotonNetwork.LeaveRoom();
        //激活游戏大厅面板
        lobbyPanel.SetActive(true);
        //禁用游戏房间面板
        roomPanel.SetActive(false);					
    }
    #endregion
    


    #region RPC函数

    //RPC函数，玩家加载场景
    [PunRPC]
    public void LoadGameScene(string sceneName) {
        //加载场景名为sceneName的场景
        PhotonNetwork.LoadLevel(sceneName); 
    }

    #endregion


    #region  IPunCallback回调函数

    /*******************************************************************************
     * 覆写IPunCallback回调函数，当玩家属性更改时调用
	 * 更新队伍面板中显示的玩家信息
	 *******************************************************************************/
    public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps) {
        //禁用队伍面板
        DisableTeamPanel();
        //根据当前玩家信息在队伍面板显示所有玩家信息（true表示显示本地玩家信息）
        UpdateTeamPanel(true);
    }

    /*******************************************************************************
     * 覆写IPunCallback回调函数，当MasterClient更变时调用
	 * 设置ReadyButton的按钮事件
	 *******************************************************************************/
    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient) {
        ReadyButtonControl();
    }

    /*
     * 覆写IPunCallback回调函数，当有玩家离开房间时调用
	 * 更新队伍面板中显示的玩家信息
	 */
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        //禁用队伍面板
        DisableTeamPanel();
        //根据当前玩家信息在队伍面板显示所有玩家信息（true表示显示本地玩家信息）
        UpdateTeamPanel(true);
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

namespace AsanCai.Competition {
    public class GameManager : PunBehaviour {
        [HideInInspector]
        public static GameManager gm;

        [HideInInspector]
        public enum GameState {
            PreStart,               //游戏开始前
            Playing,                //游戏进行中
            GameWin,                //游戏胜利
            GameLose,               //游戏失败
            Tie                     //平手
        }
        [HideInInspector]
        public GameState state = GameState.PreStart;
        [Tooltip("能否对其他玩家造成伤害")]
        public bool hurtOtherPlayer = true;
        [Tooltip("玩家1出生点")]
        public Transform playerOneSpawnTransform;
        [Tooltip("玩家2出生点")]
        public Transform playerTwoSpawnTransform;


        [Tooltip("游戏开始前检测玩家是否都加载好场景的时间")]
        public float checkPlayerTime = 5.0f;
        [Tooltip("游戏时间")]
        public float gamePlayingTime = 300.0f;
        [Tooltip("游戏结束之后停留在场景里的时间")]
        public float gameOverTime = 5.0f;

        [Tooltip("显示游戏结果")]
        public Text gameResult;
        [Tooltip("倒计时图标")]
        public Text ticker;
        [Tooltip("玩家的血量条")]
        public Slider hpBar;
        [Tooltip("血量条填充图片")]
        public Image hpBarFill;
        [Tooltip("导弹数量信息")]
        public Text missileText;
        [Tooltip("炸弹数量信息")]
        public Text bombText;
        [Tooltip("游戏胜利的音效")]
        public AudioClip gameWinAudio;
        [Tooltip("游戏失败的音效")]
        public AudioClip gameLoseAudio;
        [Tooltip("游戏平手的音效")]
        public AudioClip tieAudio;

        [Tooltip("退出房间确认面板")]
        public GameObject confirmPanel;
        [Tooltip("控制面板")]
        public GameObject mobileCtrl;

        //倒计时开始时间
        private double startTimer = 0;
        //倒计时结束时间
        private double endTimer = 0;
        //倒计时
        private double countDown = 0;
        //已加载场景的玩家个数 
        private int loadedPlayerNum = 0;

        //用于获取客户端能控制的角色
        private GameObject localPlayer;
        //用于获取场景主摄像机对象
        private Camera mainCamera;
        private ExitGames.Client.Photon.Hashtable playerCustomProperties;
        //Photon服务器循环时间
        private const float photonCircleTime = 4294967.295f;
        //保存玩家的存活状态
        private bool isAliveOfPlayer1 = true;
        private bool isAliveOfPlayer2 = true;

        private PlayerHealth playerHealth;
        private LayBombs layBombs;
        private PlayerShoot playerShoot;

        private void Awake() {
            gm = GetComponent<GameManager>();
        }

        private void Start() {
            mainCamera = Camera.main;
            isAliveOfPlayer1 = true;
            isAliveOfPlayer2 = true;

            //清空游戏结果提示信息
            gameResult.text = "";

            //使用RPC,告知所有玩家有一名玩家已成功加载场景
            photonView.RPC("ConfirmLoad", PhotonTargets.All);

            //MasterClient设置游戏开始倒计时
            if (PhotonNetwork.isMasterClient) {
                photonView.RPC("SetTime", PhotonTargets.All, PhotonNetwork.time, checkPlayerTime);
            }                                 
        }

        void Update() {
            //计算倒计时
            countDown = endTimer - PhotonNetwork.time;  
            //防止entTimer值超过Photon服务器循环时间，确保倒计时能正确结束
            if (countDown >= photonCircleTime)          
                countDown -= photonCircleTime;

            //更新倒计时的显示
            UpdateTimeLabel();                          

            //游戏状态控制
            switch (state) {
                //如果游戏处于游戏开始前
                case GameState.PreStart:
                    //MasterClient检查倒计时和场景加载人数，控制游戏开始
                    if (PhotonNetwork.isMasterClient) {     
                        CheckPlayerConnected();
                    }
                    break;
                //如果游戏处于游戏进行中
                case GameState.Playing:
                    //更新玩家生命值血条的显示
                    hpBar.value = playerHealth.currentHP;
                    hpBarFill.color = Color.Lerp(
                        Color.green, Color.red, 1 - hpBar.value * 0.01f);

                    bombText.text = "X " + layBombs.bombCount;
                    
                    if (hurtOtherPlayer) {
                        missileText.text = "X " + playerShoot.missileCount;

                        //竞技模式下的游戏逻辑
                        if (PhotonNetwork.isMasterClient) {
                            if (isAliveOfPlayer1) {
                                if (!isAliveOfPlayer2) {
                                    //玩家2死亡，玩家1存活，玩家1获胜
                                    photonView.RPC("EndCompetitionGame",
                                        PhotonTargets.All, "Player1", PhotonNetwork.time);
                                } else {
                                    //玩家1、2都存活，时间耗尽，平局
                                    if(countDown < 0.0f) {
                                        photonView.RPC("EndCompetitionGame",
                                            PhotonTargets.All, "Tie", PhotonNetwork.time);
                                    }
                                }
                            } else {
                                //玩家1死亡，玩家2存活，玩家2获胜
                                if (isAliveOfPlayer2) {
                                    photonView.RPC("EndCompetitionGame",
                                        PhotonTargets.All, "Player2", PhotonNetwork.time);
                                } else {
                                    //玩家1、2都死亡，平局
                                    photonView.RPC("EndCompetitionGame",
                                        PhotonTargets.All, "Tie", PhotonNetwork.time);
                                }
                            }
                        }
                    } else {
                        //本地角色死亡，禁用控制面板
                        if(playerHealth.isAlive == false) {
                            mobileCtrl.SetActive(false);

                            gameResult.text = "观战中";
                        }

                        //闯关模式下的游戏逻辑
                        if (PhotonNetwork.isMasterClient) {                            //分数达到目标，直接获胜
                            if(ScoreManager.sm.currentScore >= ScoreManager.sm.targetScore) {
                                photonView.RPC("EndCooperationGame",
                                    PhotonTargets.All, "Win", PhotonNetwork.time);
                            } else {
                                //玩家全部死亡，失败
                                if(!isAliveOfPlayer1 && !isAliveOfPlayer2) {
                                    photonView.RPC("EndCooperationGame",
                                        PhotonTargets.All, "Lose", PhotonNetwork.time);
                                } else {
                                    //时间耗尽，失败
                                    if(countDown < 0.0f) {
                                        photonView.RPC("EndCooperationGame",
                                            PhotonTargets.All, "Lose", PhotonNetwork.time);
                                    }
                                }
                            }
                        }
                    }

                    break;
                //游戏胜利状态，倒计时结束，退出游戏房间
                case GameState.GameWin:     
                    if (countDown <= 0)
                        LeaveRoom();
                    break;
                //游戏结束状态，倒计时结束，退出游戏房间
                case GameState.GameLose:    
                    if (countDown <= 0)
                        LeaveRoom();
                    break;
                //平手状态，倒计时结束，退出游戏房间
                case GameState.Tie:
                    if (countDown <= 0)     
                        LeaveRoom();
                    break;
            }
        }

        #region RPC函数

        //增加成功加载场景的玩家个数
        [PunRPC]
        void ConfirmLoad() {
            //本地客户端成功加载场景
            loadedPlayerNum++;
        }

        //开始游戏
        [PunRPC]
        void StartGame(double timer) {
            //设置游戏进行倒计时时间
            SetTime(timer, gamePlayingTime);
            //游戏状态切换到游戏进行状态
            gm.state = GameState.Playing;
            //创建玩家对象
            InstantiatePlayer();

            ////播放游戏开始音效
            //AudioSource.PlayClipAtPoint(gameStartAudio, localPlayer.transform.position);   
        }

        //RPC函数，设置倒计时时间
        [PunRPC]
        void SetTime(double sTime, float dTime) {
            startTimer = sTime;
            endTimer = sTime + dTime;
        }

        //游戏结束，更改客户端的游戏状态
        [PunRPC]
        void EndCompetitionGame(string winTeam, double timer) {
            //如果两队打平
            if (winTeam == "Tie") {
                //游戏状态切换为平手状态
                gm.state = GameState.Tie;
                //播放平手音效
                AudioSource.PlayClipAtPoint(tieAudio, localPlayer.transform.position);

                //游戏结束信息显示"Tie!"表示平手
                gameResult.text = "Tie!";
            } else {
                if (winTeam == PhotonNetwork.player.CustomProperties["Player"].ToString()) {
                    //游戏状态切换为游戏胜利状态
                    gm.state = GameState.GameWin;
                    gameResult.text = "You Win!";
                    //播放游戏胜利音效
                    AudioSource.PlayClipAtPoint(gameWinAudio, localPlayer.transform.position);

                } else {
                    //如果玩家属于失败队伍
                    //游戏状态切换为游戏失败状态
                    gm.state = GameState.GameLose;
                    gameResult.text = "You Lose!";
                    //播放游戏失败音效
                    AudioSource.PlayClipAtPoint(gameLoseAudio, localPlayer.transform.position);
                }
            }

            //设置游戏结束倒计时时间
            SetTime(timer, gameOverTime);   
        }


        [PunRPC]
        void EndCooperationGame(string result, double timer) {

            if (result == "Win") {
                //游戏状态切换为游戏胜利状态
                gm.state = GameState.GameWin;
                gameResult.text = "Win!";
                //播放游戏胜利音效
                AudioSource.PlayClipAtPoint(gameWinAudio, localPlayer.transform.position);
            } else {
                //如果玩家属于失败队伍
                //游戏状态切换为游戏失败状态
                gm.state = GameState.GameLose;
                gameResult.text = "Lose!";
                //播放游戏失败音效
                AudioSource.PlayClipAtPoint(gameLoseAudio, localPlayer.transform.position);
            }

            //设置游戏结束倒计时时间
            SetTime(timer, gameOverTime);
        }

        [PunRPC]
        private void SetPlayerAliveState(int player, bool alive) {
            if (PhotonNetwork.isMasterClient) {
                if(player == 1) {
                    isAliveOfPlayer1 = alive;
                }

                if(player == 2) {
                    isAliveOfPlayer2 = alive;
                }
            }
        }
        #endregion


        #region 公用函数

        /***********************************************************************
         * 检查加载场景的玩家个数
	     * 该函数只由MasterClient调用
	     ***********************************************************************/
        void CheckTeamNumber() {
            //获取房间内玩家列表
            PhotonPlayer[] players = PhotonNetwork.playerList;      
            int playerOneNum = 0, playerTwoNum = 0;

            //遍历所有玩家，计算两队人数
            foreach (PhotonPlayer p in players) {                   
                if (p.CustomProperties["Player"].ToString() == "Player1")
                    playerOneNum++;

                if (p.CustomProperties["Player"].ToString() == "Player2")
                    playerTwoNum++;
            }

            //玩家1中途退出游戏，视为死亡
            if (playerOneNum == 0) {
                photonView.RPC("SetPlayerAliveState", PhotonTargets.All, 1, false);;
            }
            if (playerTwoNum == 0) {
                photonView.RPC("SetPlayerAliveState", PhotonTargets.All, 2, false);
            }
        }

        /********************************************************************
         * 更新倒计时数字
         ********************************************************************/
        void UpdateTimeLabel() {
            int minute = (int)countDown / 60;
            int second = (int)countDown % 60;
            ticker.text = minute.ToString("00") + ":" + second.ToString("00");
        }

        //检查所有玩家是否已经加载场景
        void CheckPlayerConnected() {
            //游戏开始倒计时结束，或者所有玩家加载场景
            if (countDown <= 0.0f || loadedPlayerNum == PhotonNetwork.playerList.Length) {
                //使用Photon服务器的时间设置游戏开始时间
                startTimer = PhotonNetwork.time;

                //Debug.Log(string.Format("{0}/{1}", loadedPlayerNum, PhotonNetwork.playerList.Length));
                //使用RPC，所有玩家开始游戏
                photonView.RPC("StartGame", PhotonTargets.All, startTimer);     
            }
        }

        //生成玩家对象
        void InstantiatePlayer() {
            //获取玩家自定义属性
            playerCustomProperties = PhotonNetwork.player.CustomProperties;
            int player = 0;
            
            //如果玩家是Player1，生成Player1对象
            if (playerCustomProperties["Player"].ToString().Equals("Player1")) {
                player = 1;

                localPlayer = PhotonNetwork.Instantiate("Player1",
                    playerOneSpawnTransform.position, Quaternion.identity, 0);

                //设置玩家的初始朝向
                localPlayer.GetComponent<Transform>().localScale = playerOneSpawnTransform.localScale;

                //设置头上的血量条为不可见
                localPlayer.transform.Find("healthDisplay").gameObject.SetActive(false);

                //设置摄像机的初始位置
                mainCamera.transform.position = new Vector3(
                    playerOneSpawnTransform.position.x,
                    playerOneSpawnTransform.position.y,
                    mainCamera.transform.position.z
                    );
            }
            //如果玩家是Player2，生成Player2对象
            if (playerCustomProperties["Player"].ToString().Equals("Player2")) {
                player = 2;

                localPlayer = PhotonNetwork.Instantiate("Player2", 
                    playerTwoSpawnTransform.position, Quaternion.identity, 0);

                //设置玩家的初始朝向
                localPlayer.GetComponent<Transform>().localScale = playerTwoSpawnTransform.localScale;

                //设置头上的血量条为不可见
                localPlayer.transform.Find("healthDisplay").gameObject.SetActive(false);

                //设置摄像机的初始位置
                mainCamera.transform.position = new Vector3(
                    playerTwoSpawnTransform.position.x,
                    playerTwoSpawnTransform.position.y,
                    mainCamera.transform.position.z
                    );    
            }
            //启用PlayerMove脚本，使玩家对象可以被本地客户端操控
            localPlayer.GetComponent<PlayerController>().enabled = true;
            //启用PlayerShoot脚本，使玩家对象可以射击
            playerShoot = localPlayer.GetComponent<PlayerShoot>();
            playerShoot.enabled = true;
            playerShoot.SetPlayer(player);
            //启用LayBombs脚本，使玩家可以放置炸弹
            layBombs = localPlayer.GetComponent<LayBombs>();
            layBombs.enabled = true;
            layBombs.SetPlayer(player);

            //获取玩家对象的PlayerHealth脚本
            playerHealth = localPlayer.GetComponent<PlayerHealth>();
            //设置显示玩家血量的Slider控件
            hpBar.maxValue = playerHealth.maxHP;
            hpBar.minValue = 0;
            hpBar.value = playerHealth.currentHP;

            //开启摄像机跟随
            mainCamera.GetComponent<CameraFollow>().enabled = true;
        }

        //获取当前玩家的存活状态
        public void UpdateAliveState(int player, bool alive) {
            //更新当前存活状态
            photonView.RPC("SetPlayerAliveState", PhotonTargets.All, player, alive);
        }

        //离开房间函数
        private void LeaveRoom() {
            //玩家离开游戏房间
            PhotonNetwork.LeaveRoom();
            //加载游戏大厅场景
            PhotonNetwork.LoadLevel("LobbyScene");
        }
        #endregion


        #region IPunCallback回调函数

        /*******************************************************************
         * 有玩家断开连接时（离开房间）MasterClient检查双方人数
	     *******************************************************************/
        public override void OnPhotonPlayerDisconnected(PhotonPlayer other) {
            //游戏状态不是游戏进行中，结束函数执行
            if (state != GameState.Playing) {
                return;
            }

            //MasterClient检查
            if (PhotonNetwork.isMasterClient) {
                //检查两队人数
                CheckTeamNumber();    
            }
        }

        /******************************************************************
         * 如果玩家断开与Photon服务器的连接，加载场景GameLobby
         ******************************************************************/
        public override void OnConnectionFail(DisconnectCause cause) {
            PhotonNetwork.LoadLevel("LobbyScene");
        }
        #endregion


        #region 按钮事件处理函数
        //点击退出房间按钮
        public void ClickExitBtn() {
            confirmPanel.SetActive(true);
        }

        //点击确认按钮
        public void ClickConfirmBtn() {
            LeaveRoom();
        }

        //点击取消按钮
        public void ClickCancelBtn() {
            confirmPanel.SetActive(false);
        }
        #endregion
    }
}


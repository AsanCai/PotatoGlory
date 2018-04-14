using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

namespace AsanCai.Competition {
    public class ScoreManager : PunBehaviour {
        [HideInInspector]
        public static ScoreManager sm;

        [Tooltip("玩家得分榜")]
        public GameObject scorePanel;
        [Tooltip("目标得分")]
        public int targetScore;
        [Tooltip("当前总得分")]
        public Text scoreText;
        [Tooltip("玩家1得分")]
        public Text scoreTextOfPlayer1;
        [Tooltip("玩家2得分")]
        public Text ScoreTextOfPlayer2;


        //当前的总分数
        [HideInInspector]
        public int currentScore = 0;
        private int scoreOfPlayer1 = 0;
        private int scoreOfPlayer2 = 0;

        private void Awake() {
            sm = GetComponent<ScoreManager>();
        }

        private void Start() {
            //确保分数榜一开始处于禁用状态
            scorePanel.SetActive(false);
        }

        private void Update() {
            scoreText.text = currentScore + " / " + targetScore;

            scoreTextOfPlayer1.text = scoreOfPlayer1 + "";
            ScoreTextOfPlayer2.text = scoreOfPlayer2 + "";
        }

        public void AddScore(int player, int score) {
            //photonView.RPC("AddScoreOfPlayer", PhotonTargets.All, player, score);

            if (PhotonNetwork.isMasterClient) {
                if (player == 1) {
                    scoreOfPlayer1 += score;
                    photonView.RPC("UpdateScore", PhotonTargets.All, 1, scoreOfPlayer1);
                }

                if (player == 2) {
                    scoreOfPlayer2 += score;
                    photonView.RPC("UpdateScore", PhotonTargets.All, 2, scoreOfPlayer2);
                }

            }
        }

        //玩家增加分数
        [PunRPC]
        private void AddScoreOfPlayer(int player, int score) {
            if (PhotonNetwork.isMasterClient) {
                if(player == 1) {
                    scoreOfPlayer1 += score;
                }

                if(player == 2) {
                    scoreOfPlayer2 += score;
                }

                photonView.RPC("UpdateScore", PhotonTargets.All, 1, scoreOfPlayer1);
            }
        }


        #region 按钮点击函数
        public void ClickShowScorePanelBtn() {
            scorePanel.SetActive(true);
        }


        public void ClickBackBtn() {
            scorePanel.SetActive(false);
        }
        #endregion

        [PunRPC]
        private void UpdateScore(int player, int score) {
            if(player == 1) {
                scoreOfPlayer1 = score;
            }

            if(player == 2) {
                scoreOfPlayer2 = score;
            }

            currentScore = scoreOfPlayer1 + scoreOfPlayer2;
        }
    }
}

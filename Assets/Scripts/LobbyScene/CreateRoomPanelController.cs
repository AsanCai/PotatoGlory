using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class CreateRoomPanelController : PunBehaviour {
    [Tooltip("创建房间面板")]
    public GameObject createRoomPanel;
    [Tooltip("房间名称")]
    public Text roomName;
    [Tooltip("房间名称提示信息")]
    public Text roomNameHint;

    /*************************************************************************
     *需要添加ToggleGroup脚本，并在Toggle组件那里设置Group，才会出现单选框效果 
     *************************************************************************/
    [Tooltip("游戏模式单选框组")]
    public GameObject gameModeToggles;



    private byte maxPlayers = 2;
    private string[] modes = { "对战", "闯关" };
    private string[] keys = { "GameMode" };

	void OnEnable () {
        //清空房间名称提示信息
        roomNameHint.text = "";
	}

    //"确认创建"按钮事件处理函数
    public void ClickConfirmButton() {
        //设置房间最大人数
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;

        //获取游戏模式单选框组的所有子对象
        RectTransform toggleRectTransform = gameModeToggles.GetComponent<RectTransform>();
        int childCount = toggleRectTransform.childCount;
        //确认房间的游戏模式
        for (int i = 0; i < childCount; i++) {

            if (toggleRectTransform.GetChild(i).GetComponent<Toggle>().isOn == true) {
                //设置房间自定义属性
                roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
                    {keys[0], modes[i]}
                };

                //设置哪些自定义属性要同步到大厅里
                roomOptions.CustomRoomPropertiesForLobby = keys;
                break;
            }
        }

        //获取游戏大厅内所有游戏房间
        RoomInfo[] roomInfos = PhotonNetwork.GetRoomList(); 
        bool isRoomNameRepeat = false;
        //遍历游戏房间，检查新创建的房间名是否与已有房间重复
        foreach (RoomInfo info in roomInfos) {
            if (roomName.text == info.Name) {
                isRoomNameRepeat = true;
                break;
            }
        }
        //如果房间名称重复，房间名称提示文本显示"房间名称重复！"
        if (isRoomNameRepeat) {
            roomNameHint.text = "房间名称重复!";
        }
        //否则，根据玩家设置的房间名、房间玩家人数创建房间
        else {
            //在默认游戏大厅中创建游戏房间
            PhotonNetwork.CreateRoom(roomName.text, roomOptions, TypedLobby.Default);
            //禁用创建房间面板
            createRoomPanel.SetActive(false);   
        }
    }

    //"取消创建"按钮事件处理函数
    public void ClickCancelButton() {
        //禁用创建房间面板
        createRoomPanel.SetActive(false);
        //清空房间名称提示文本
        roomNameHint.text = "";                 
    }
}

using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.UI;

public class ClientManager : MonoBehaviour {
    [HideInInspector]
    public static ClientManager cm;

    [Tooltip("主机地址")]
    public string host;
    [Tooltip("端口地址")]
    public int port;

    //用于发送心跳协议的计时器
    private System.Timers.Timer heartTimer = new System.Timers.Timer(5000);

    [HideInInspector]
    public string recvStr = "";
    [HideInInspector]
    public byte[] readBuff = new byte[BUFFER_SIZE];
    [HideInInspector]
    public enum State {
        login,
        register,
        connected,
        disconnected
    }
    [HideInInspector]
    public State state = State.disconnected;
    //提示信息显示时间
    [HideInInspector]
    public float timer = 0;

    private Socket socket;
    private const int BUFFER_SIZE = 1024;
    private string login = "LOGIN";
    private string register = "REGISTER";
    //文字提示显示的时间
    private float showTime = 1.5f;

    private int minPasswordLength = 7;
    

    private void Awake() {
        cm = GetComponent<ClientManager>();
    }

    //初始化管理器
    public void Init() {
        recvStr = "";
        state = State.disconnected;
    }


    private void Update() {
        //非主进程不能使用协程
        if(recvStr != "") {
            if (timer < showTime) {
                timer += Time.deltaTime;
            } else {
                recvStr = "";
            }
        }
    }

    public void Connection() {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //错误被catch之后还会继续往下执行
        try {
            IPAddress ipAddress = IPAddress.Parse(host);
            IPEndPoint ipPoint = new IPEndPoint(ipAddress, port);

            socket.Connect(ipPoint);
            state = State.connected;

            //开始接收服务器的信息
            socket.BeginReceive(
                readBuff,
                0,
                BUFFER_SIZE,
                SocketFlags.None,
                Receivecb,
                null
                );

            //每五秒发送一次HeartBeat
            StartCoroutine(SendHeartBeat(5.0f));

        } catch {
            recvStr = "与服务器建立连接失败，正在尝试重连";
            //重连
            StartCoroutine(Reconnect());
            timer = 0;
        }
    }

    private IEnumerator SendHeartBeat(float interval) {
        yield return new WaitForSeconds(interval);
        string str = "HeartBeat";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
        try {
            socket.Send(bytes);
            heartTimer.Start();

            //循环调用
            StartCoroutine(SendHeartBeat(interval));
        } catch {
            recvStr = "网络状态异常";
            //重连
            StartCoroutine(Reconnect());
            timer = 0;
        }
    }

    //异步接收服务器端传送回来的数据
    private void Receivecb(IAsyncResult ar) {
        try {
            int count = socket.EndReceive(ar);
            string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);

            string[] result = str.Split(' ');

            //显示操作结果
            if(result[0] == login) {
                if (result[1] == "SUCCESS") {
                    recvStr = "登录成功";
                    state = State.login;
                } else {
                    recvStr = "登录失败，请检查用户名和密码重试";
                    state = State.connected;
                }
            } else {
                if(result[0] == register){
                    if (result[1] == "REPEAT") {
                        recvStr = "用户名已存在";
                        state = State.connected;
                    } else if(result[1] == "SUCCESS") {
                        recvStr = "注册成功";
                        state = State.register;
                    } else {
                        recvStr = "注册失败";
                        state = State.connected;
                    }
                }
            }

            socket.BeginReceive(
                    readBuff,
                    0,
                    BUFFER_SIZE,
                    SocketFlags.None,
                    Receivecb,
                    null);
        } catch {
            recvStr = "服务器异常";
            timer = 0;
        }
    }

    //登录
    public void Login(string un, string pwd) {
        string username = un.Trim();
        string password = pwd.Trim();

        if (!IsSafeStr(username)) {
            recvStr = "用户名包含非法字符";
            timer = 0;
            return;
        } else {
            if (!IsSafeStr(password)) {
                recvStr = "密码包含非法字符";
                timer = 0;
                return;
            }
        }

        if(username.Length == 0) {
            recvStr = "请输入用户名";
            timer = 0;
            return;
        } else {
            if(password.Length == 0) {
                recvStr = "请输入密码";
                timer = 0;
                return;
            } else if(password.Length < minPasswordLength) {
                recvStr = "密码长度过短";
                timer = 0;
                return;
            }
        }

        string str = login + " " + username + " " + password;
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
        try {
            socket.Send(bytes);
        } catch (Exception e) {
            recvStr = "登录失败，请检查网络状态重试";
            //重连
            StartCoroutine(Reconnect());
            timer = 0;
        }
    }

    //注册
    public void Register(string um, string pwd, string repwd) {
        string username = um.Trim();
        string password = pwd.Trim();
        string repeatPassword = repwd.Trim();

        if (!IsSafeStr(username)) {
            recvStr = "用户名包含非法字符";
            timer = 0;
            return;
        } else {
            if (!IsSafeStr(password) || !IsSafeStr(repeatPassword)) {
                recvStr = "密码包含非法字符";
                timer = 0;
                return;
            }
        }

        if (username.Length == 0) {
            recvStr = "请输入用户名";
            timer = 0;
            return;
        } else {
            if (password.Length == 0) {
                recvStr = "请输入密码";
                timer = 0;
                return;
            } else if (password.Length < minPasswordLength) {
                recvStr = "密码长度过短";
                timer = 0;
                return;
            }
        }

        if (password != repeatPassword) {
            recvStr = "两次输入密码不一致";
            timer = 0;
            return;
        }

        string str = register + " " + username + " " + password;
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
        try {
            socket.Send(bytes);
        } catch (Exception e) {
            recvStr = "注册失败，请检查网络之后重试";

            //重连
            StartCoroutine(Reconnect());
            timer = 0;
        }
    }

    private bool IsSafeStr(string str) {
        //[]表示匹配其中的单个字符，空格、- ; , | / () [] {} % * ! \都为非法字符
        return !Regex.IsMatch(str, @"[- | ; | , | \/ | \( | \) | \[ | \] | \{ | \} | % | \* | ! | \']");
    }

    public void Disconnect() {
        //如果当前有socket实例，就释放
        if(socket != null) {
            socket.Close();
        }
        state = State.disconnected;
    }

    public void ResetState() {
        state = State.connected;
    }

    private IEnumerator Reconnect() {
        yield return new WaitForSeconds(showTime);
        ClientManager.cm.Connection();
    }
}

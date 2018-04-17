# 2D-Shooter

---
1. 项目

    本项目是一个基于[Unity3D游戏引擎](https://unity3d.com/cn/)和[Photon网络引擎](https://www.photonengine.com/zh-CN/Photon)开发的双人射击手游，大部分素材来源于Unity Assets Store上一个免费的单机demo[2D Platformer](https://assetstore.unity.com/packages/essentials/tutorial-projects/2d-platformer-11228)。有兴趣可以下载[游戏安装包](http://download-1253364479.cosgz.myqcloud.com/PotatoGlory.apk)进行体验，如遇到什么问题欢迎发邮件给我进行交流。

---


2. 游戏功能

    游戏的主要功能包括：
* 用户注册登录功能（*因为Photon不提供登录服务器，因此需要自己搭建数据库并编写服务端程序，本项目的服务端程序仓库为[PotatoGloryGameServer](https://github.com/AsanCai/PotatoGloryGameServer)*）
* 创建游戏房间，进入多人游戏场景功能
* 同步游戏数据，实现玩家之间的交互功能
* 技能冷却效果
* 实现虚拟摇杆控制人物移动功能

---

3. 项目运行说明

* clone服务器项目到本地，创建数据库之后，设置数据库连接参数、主机ip（建议在本地运行，ip填127.0.0.1）和端口
* clone本项目到本地，然后打开StartScene，将ClientManager.cs绑定到ClientManager上，填写主机ip和端口
* 运行服务器程序（因为是.net Core程序，所以需要安装.net Core和相应的.net framework）


---

4. 游戏截图

![游戏主界面](https://github.com/AsanCai/2D-Shooter/raw/android/Images/img1.png)

![游戏大厅](https://github.com/AsanCai/2D-Shooter/raw/android/Images/img2.png)

![创建房间](https://github.com/AsanCai/2D-Shooter/raw/android/Images/img3.png)

![创建房间成功](https://github.com/AsanCai/2D-Shooter/raw/android/Images/img4.png)

![加入房间](https://github.com/AsanCai/2D-Shooter/raw/android/Images/img5.png)

![准备开始](https://github.com/AsanCai/2D-Shooter/raw/android/Images/img6.png)

![开始游戏](https://github.com/AsanCai/2D-Shooter/raw/android/Images/img7.png)

![游戏失败](https://github.com/AsanCai/2D-Shooter/raw/android/Images/img8.png)

---

5. 项目感想

    因为Photon的官网上没有对Api的详细说明，只能自己看着官方demo自己研究每个api的用法，踩了无数个坑。此外，由于一开始对Unity3d的运用也不是很熟练，我花费了很多时间解决了很多之前没想过的小问题。再加上最后自己在服务器上搭建数据库编写服务器程序的时候也踩了很多坑，这个项目的制作过程可以说是十分艰难。但踩完这么多坑之后，我也收获颇丰，因此我打算重新整理整个项目之后，在[个人博客](asancai.github.io)写一个系列教程，与大家分享交流。


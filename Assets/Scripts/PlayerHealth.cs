using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    //角色的生命值
    public float health = 100f;
    //角色受到伤害后的免伤时间
    public float repeatDamagePeriod = 2f;
    //角色受到伤害时受到的力，制造击退效果
    public float hurtForce = 10f;
    //每次受伤害时减少的血量
    public float damageAmount = 10f;
    //用于保存受伤音效
    public AudioClip[] ouchClips;

    //保存相关的引用
    private SpriteRenderer healthBar;
    private Vector3 healthScale;
    private PlayerControl playCtrl;
    private Animator anim;
    //用于检测当前是否处于免伤状态
    private float lastHitTime;


    private void Awake() {
        playCtrl = GetComponent<PlayerControl>();

        healthBar = GameObject.Find("HealthBar").GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();


        healthScale = healthBar.transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //假如撞到怪物
        if(collision.gameObject.tag == "Enemy") {
            //判断此时是否处于免伤状态
            if(Time.time > lastHitTime + repeatDamagePeriod) {
                //检测当前血量
                if(health > 0f) {
                    //调用受伤函数
                    TakeDamage(collision.transform);
                    //更新时间
                    lastHitTime = Time.time;
                } else {
                    //角色死亡
                    Collider2D[] cols = GetComponents<Collider2D>();
                    foreach(Collider2D c in cols) {
                        c.isTrigger = true;
                    }

                    //把sortingLayer改为UI，下落的时候可以看到
                    SpriteRenderer[] spr = GetComponentsInChildren<SpriteRenderer>();
                    foreach(SpriteRenderer s in spr) {
                        s.sortingLayerName = "UI";
                    }

                    //禁用脚本
                    GetComponent<PlayerControl>().enabled = false;
                    GetComponentInChildren<Gun>().enabled = false;

                    //播放死亡动画
                    anim.SetTrigger("Die");
                }
            }
        }
    }

    //受伤函数
    void TakeDamage(Transform enemy) {
        //角色不能跳跃
        playCtrl.jump = false;

        //给角色加上后退的力，制造击退效果
        Vector3 hurtVector = transform.position - enemy.position + Vector3.up * 5f;
        GetComponent<Rigidbody2D>().AddForce(hurtVector * hurtForce);

        //更新角色的生命值
        health -= damageAmount;

        //更新生命条
        UpdateHealthBar();

        //随机播放音频
        int i = Random.Range(0, ouchClips.Length);
        AudioSource.PlayClipAtPoint(ouchClips[i], transform.position);
    }

    public void UpdateHealthBar() {
        //把生命条的颜色逐渐变红
        healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - health * 0.01f);
        //缩短生命条
        healthBar.transform.localScale = new Vector3(healthScale.x * health * 0.01f, 1, 1);
    }
}

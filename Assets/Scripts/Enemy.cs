//控制怪物的脚本

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    //怪物移动的速度
    public float moveSpeed = 2f;
    //怪物的血量，也就是要被子弹打几下才死
    public int HP = 2;
    //保存死亡的图片
    public Sprite deadEnemy;
    //保存手上的图片
    public Sprite damagedEnemy;
    //保存死亡的音效
    public AudioClip[] deathClips;
    //保存得分的UI
    public GameObject hundredPointsUI;
    //保存死亡时的旋转范围
    public float deathSpinMin = -100f;
    public float deathSpinMax = 100f;

    //用于设置怪物对象显示的Sprite
    private SpriteRenderer spriteRenderer;
    //用于检测怪物前方是否有障碍物
    private Transform frontCheck;
    //检测怪物当前是否死亡
    private bool dead = false;
    //获取Score脚本的引用
    //private Score score;
    //用于设置怪物对象的物理属性
    private Rigidbody2D body;

    private void Awake() {
        spriteRenderer = transform.Find("body").GetComponent<SpriteRenderer>();

        frontCheck = transform.Find("frontCheck").transform;

        //score = GameObject.Find("Score").GetComponent<Score>();

        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        Collider2D[] frontHits = Physics2D.OverlapPointAll(frontCheck.position, 1);

        //假如前方是障碍物，就只执行转向操作
        foreach(Collider2D c in frontHits) {
            if(c.tag == "Obstacle") {
                Flip();
                break;
            }
        }


        body.velocity = new Vector2(transform.localScale.x * moveSpeed, body.velocity.y);

        if (HP == 1 && damagedEnemy != null)
            spriteRenderer.sprite = damagedEnemy;

        if(HP <= 0 && !dead) {
            Death();
        }
    }

    public void Hurt() {
        HP--;
    }

    void Death() {
        SpriteRenderer[] otherRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach(SpriteRenderer s in otherRenderers) {
            s.enabled = false;
        }

        spriteRenderer.enabled = true;
        spriteRenderer.sprite = deadEnemy;

        //score.score += 100;

        dead = true;

        Collider2D[] cols = GetComponents<Collider2D>();

        foreach(Collider2D c in cols) {
            c.isTrigger = true;
        }

        int i = Random.Range(0, deathClips.Length);
        AudioSource.PlayClipAtPoint(deathClips[i], transform.position);

        Vector3 scorePos;
        scorePos = transform.position;

        scorePos.y += 1.5f;

        //Instantiate(hundredPointsUI, scorePos, Quaternion.identity);
    }

    public void Flip() {
        Vector3 enemyScale = transform.localScale;

        enemyScale.x *= -1;

        transform.localScale = enemyScale;
    }
}

//角色移动控制脚本

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    [HideInInspector]
    //监测角色的朝向
    public bool facingRight = true;
    [HideInInspector]
    //监测角色是否处于跳跃状态
    public bool jump = false;


    public float moveForce = 365f;
    public float maxSpeed = 5f;
    public float jumpForce = 1000f;

    //保存跳跃的音效
    public AudioClip[] jumpClips;
    //保存嘲讽音效
    public AudioClip[] taunts;
    //得分之后播放嘲讽音效的概率
    public float tauntProbaility = 50f;
    //嘲讽间隔
    public float tauntDelay = 1f;


    private int tauntIndex;
    //groundCheck用于检测是否落地
    private Transform groundCheck;
    //记录当前的落地状态
    private bool grounded = false;
    private Animator animator;
    private Rigidbody2D body;
    private AudioSource audioSource;

    private void Awake() {
        //寻找名为groundCheck的子组件
        groundCheck = transform.Find("groundCheck");
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();
    }

	
	void Update () {
        //通过检测角色和groundCheck之间是否存在Ground层的物体来判断当前是否落地
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        if (Input.GetButtonDown("Jump") && grounded)
            jump = true;
	}

    private void FixedUpdate() {
        //获取水平输入
        float h = Input.GetAxis("Horizontal");
        //播放行走动画
        animator.SetFloat("Speed", Mathf.Abs(h));

        if(h * body.velocity.x < maxSpeed) {
            //给角色添加行走的力
            body.AddForce(Vector2.right * h * moveForce);
        }

        //设置物体速度的阈值
        if(Mathf.Abs(body.velocity.x) > maxSpeed) {
            body.velocity = new Vector2(Mathf.Sign(body.velocity.x) * maxSpeed, body.velocity.y);
        }

        //判断当前是否需要转向
        if(h > 0 && !facingRight) {
            Flip();
        }else if(h < 0 && facingRight) {
            Flip();
        }


        if (jump) {
            //这个和普通的设置值有什么区别？
            animator.SetTrigger("Jump");

            //随机在角色当前所处的位置播放一个跳跃的音频
            int i = Random.Range(0, jumpClips.Length);
            AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

            //设置一个竖直向上的力
            body.AddForce(new Vector2(0f, jumpForce));

            // 只跳跃一次，避免重复跳跃
            jump = false;
        }
    }
    

    void Flip() {
        //修改当前朝向的记录值
        facingRight = !facingRight;

        //修改scale的x分量实现转向
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    //触发嘲讽效果的协程
    public IEnumerator Taunt() {
        float tauntChance = Random.Range(0f, 100f);

        if(tauntChance > tauntProbaility) {
            yield return new WaitForSeconds(tauntDelay);


            if (!audioSource.isPlaying) {
                tauntIndex = TauntRandom();

                audioSource.clip = taunts[tauntIndex];
                audioSource.Play();
            }
        }
    }

    //确保相邻两次嘲讽音效不相同
    int TauntRandom() {
        int i = Random.Range(0, taunts.Length);

        if (i == tauntIndex)
            return TauntRandom();
        else
            return i;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPickup : MonoBehaviour {
    public AudioClip pickupClip;

    private Animator anim;
    private bool landed = false;

    private void Awake() {
        anim = transform.root.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player") {
            AudioSource.PlayClipAtPoint(pickupClip, transform.position);

            collision.GetComponent<LayBombs>().bombCount ++;

            Destroy(transform.root.gameObject);
        } else {
            if(collision.tag == "ground" && !landed) {
                anim.SetTrigger("Land");

                transform.parent = null;

                gameObject.AddComponent<Rigidbody2D>();
                landed = true;
            }
        }
    }
}

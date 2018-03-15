using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {

    public float healthBouns;
    public AudioClip collect;

    private PickupSpawner pickupSpawner;
    private Animator anim;
    private bool landed;

    private void Awake() {
        pickupSpawner = GameObject.Find("pickupManager").GetComponent<PickupSpawner>();

        anim = transform.root.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player") {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            playerHealth.health += healthBouns;
            playerHealth.health = Mathf.Clamp(playerHealth.health, 0f, 100f);

            playerHealth.UpdateHealthBar();

            pickupSpawner.StartCoroutine(pickupSpawner.DeliverPickup());

            AudioSource.PlayClipAtPoint(collect, transform.position);

            Destroy(transform.root.gameObject);
        } else {
            if (collision.tag == "ground" && !landed) {
                anim.SetTrigger("Land");

                transform.parent = null;
                gameObject.AddComponent<Rigidbody2D>();
                landed = true;
            }
        }

        
    }
}

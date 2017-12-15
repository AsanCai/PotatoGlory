using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public float bombRadius = 10f;
    public float bombForce = 100f;
    public AudioClip boom;
    public AudioClip fuse;
    public float fuseTime = 1.5f;
    public GameObject explosion;

    private LayBombs layBombs;
    private PickupSpawner pickupSpawner;
    private ParticleSystem explosionFX;

    private void Awake() {
        explosionFX = GameObject.FindGameObjectWithTag("ExplosionFX").GetComponent<ParticleSystem>();

        pickupSpawner = GameObject.Find("pickupManager").GetComponent<PickupSpawner>();

        layBombs = GameObject.Find("LayBombs").GetComponent<LayBombs>();
    }

    private void Start() {
        if (transform.root == transform)
            StartCoroutine(BombDetonation());
    }

    IEnumerator BombDetonation() {
        AudioSource.PlayClipAtPoint(fuse, transform.position);

        yield return new WaitForSeconds(fuseTime);

        Explode();
    }

    public void Explode() {
        layBombs.bombLaid = false;

        //当炸弹爆炸的时候生成新的道具
        pickupSpawner.StartCoroutine(pickupSpawner.DeliverPickup());

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, bombRadius, 1 << LayerMask.NameToLayer("Enemies"));

        foreach(Collider2D en in enemies) {
            Rigidbody2D rb = en.GetComponent<Rigidbody2D>();

            if(rb != null && rb.tag == "Enemy") {
                rb.gameObject.GetComponent<Enemy>().HP = 0;

                Vector3 deltaPos = rb.transform.position - transform.position;

                Vector3 force = deltaPos.normalized * bombForce;
                rb.AddForce(force);
            }
        }

        explosionFX.transform.position = transform.position;
        explosionFX.Play();

        Instantiate(explosion, transform.position, Quaternion.identity);

        AudioSource.PlayClipAtPoint(boom, transform.position);

        Destroy(gameObject);
    }
}

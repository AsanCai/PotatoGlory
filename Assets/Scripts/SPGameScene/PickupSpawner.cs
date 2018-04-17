using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour {

    public GameObject[] pickups;

    public float pickupDeliveryTime = 5f;
    public float dropRangeLeft;
    public float dropRangeRight;
    public float highHealthThreshold = 75f;
    public float lowHealthThreshold = 25f;

    private PlayerHealth playerHealth;

    void Awake() {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    void Start () {
        StartCoroutine(DeliverPickup());
	}
	

    public IEnumerator DeliverPickup() {
        //生成道具的间隔时间
        yield return new WaitForSeconds(pickupDeliveryTime);

        float dropPosX = Random.Range(dropRangeLeft, dropRangeRight);

        Vector3 dropPos = new Vector3(dropPosX, 15f, 1f);


        if (playerHealth.health >= highHealthThreshold)
            //血量较高，生成炸弹道具
            Instantiate(pickups[0], dropPos, Quaternion.identity);

        else if (playerHealth.health <= lowHealthThreshold)
            //血量较低，生成补血包道具
            Instantiate(pickups[1], dropPos, Quaternion.identity);

        else {
            //血量不高不低时，随机生成
            int pickupIndex = Random.Range(0, pickups.Length);
            Instantiate(pickups[pickupIndex], dropPos, Quaternion.identity);
        }

    }
}

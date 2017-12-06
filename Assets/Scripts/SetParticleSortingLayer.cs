using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParticleSortingLayer : MonoBehaviour {

    public string sortingLayerName;

	// Use this for initialization
	void Start () {
        GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = sortingLayerName;
	}

}

// Script simple que destruye un sistema de partículas al finalizar

using UnityEngine;
using System.Collections;

public class ParticleDestroyer : MonoBehaviour {
	private ParticleSystem ps;

	void Start () {
		ps = GetComponent<ParticleSystem> ();
	}

	void Update () {
		if (!ps.IsAlive()) {
			Destroy(gameObject);
		}
	}
}
// Script para manejar los efectos de una caja en estado de destrucción
using UnityEngine;

public class DestroyedCrate : MonoBehaviour {
	[SerializeField] float lifeTime = 2f;	// Tiempo que durará el efecto
	private float timer;					// Temporizador

	// Al comienzo inicializa el temporizador y aplica una fuerza a cada fragmento de la caja
	// según su posición
	void Start () {
		timer = lifeTime;

		foreach (Transform child in (transform.GetChild (0))) {
			(child.GetComponent<Rigidbody> ()).AddForce ((child.position - transform.position) * 400f);
		}
	}

	// En cada momento actualiza el temporizador y destruye la caja si éste llega a 0
	void Update () {
		timer -= Time.deltaTime;

		if (timer <= 0)
			GameObject.Destroy (this.gameObject);
	}
}
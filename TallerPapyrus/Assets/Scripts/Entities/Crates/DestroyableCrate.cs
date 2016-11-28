// Script para una caja destruíble
using UnityEngine;
using System.Collections;

public class DestroyableCrate : MonoBehaviour {
	// La caja destruída
	public GameObject destroyedCrate;

	// Destuye la caja
	public void destroyCrate () {
		if (destroyedCrate) {
			// Instancia la caja destruída
			GameObject destroyed = (GameObject) Instantiate(destroyedCrate, transform.position, transform.rotation);
			destroyed.transform.localScale = transform.localScale;	// La escala ha de ser la misma
		}

		Destroy(gameObject);	// Destruye esta caja
	}
}
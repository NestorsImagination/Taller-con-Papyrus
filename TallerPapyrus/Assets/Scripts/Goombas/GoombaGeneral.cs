using UnityEngine;
using System.Collections;

public class GoombaGeneral : MonoBehaviour {
	[SerializeField] GameObject particles;	// Particulas al ser destruído el Goomba
	[SerializeField] float particlesSize = 1;

	private bool dead = false;
	private AudioSource stepSound;	// Sonido de pasos

	void Start () {
		stepSound = GetComponent <AudioSource> ();
	}

	public void die () {
		dead = true;
	}

	void OnCollisionEnter (Collision collision) {
		if (dead) {
			GameObject collidedEntity = collision.collider.gameObject;	// Objeto con el que ha colisionado

			// Si es un objeto sólido
			if (collidedEntity.CompareTag ("Wall") ||
				collidedEntity.CompareTag ("Crate") || 
				collidedEntity.CompareTag ("MovingPlatform")) {

				// Instancia las partículas
				GameObject particlesInst = (GameObject) GameObject.Instantiate (particles, transform.position, Quaternion.identity);
				particlesInst.GetComponent <ParticleSystem> ().scalingMode = ParticleSystemScalingMode.Hierarchy;
				particlesInst.transform.localScale = new Vector3 (particlesSize, particlesSize, particlesSize);

				// Destruye el Goomba
				GameObject.Destroy (this.gameObject);
			}
		}
	}

	// Hace el sonido de pasos
	private void playStepSound () {
		stepSound.Play ();
	}
}

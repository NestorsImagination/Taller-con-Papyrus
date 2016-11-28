// Script para un ataque hueso
using UnityEngine;

public class AttackScript : MonoBehaviour {
	[SerializeField] float rotSpeed = 500;		// Velocidad de rotación del ataque
	[SerializeField] float lifeTime = 1f;		// Tiempo de vida del ataque
	[SerializeField] GameObject particles;		// Particulas al ser destruído el ataque
	[SerializeField] AudioClip sound;			// Sonido al ser destruído el ataque
	[SerializeField] float throwingForceH;		// Fuerza horizontal con la que se lanzará por los aires a un enemigo derrotado
	[SerializeField] float throwingForceV;		// Fuerza vertical con la que se lanzará por los aires a un enemigo derrotado
	[SerializeField] float maxThrowingAngularS;	// Máxima velocidad de giro que se conferirá a un enemigo al derrotarlo

	void Start () {
		AudioSource.PlayClipAtPoint (sound, transform.position);
	}

	// En cada momento
	void FixedUpdate () {
		transform.Rotate (Vector3.right, Time.deltaTime * rotSpeed);	// Rota el ataque

		lifeTime -= Time.deltaTime;					// Resta tiempo de vida
		if (lifeTime <= 0)							// Destruye el ataque si ha existido demasiado tiempo
			GameObject.Destroy (this.gameObject);
	}

	// Al colisionar
	void OnCollisionEnter (Collision collision) {
		GameObject collidedEntity = collision.collider.gameObject;	// Objeto con el que ha colisionado
		if (collidedEntity.CompareTag ("Wall") || 					// Si es un objeto sólido
			collidedEntity.CompareTag ("Crate") || 
			collidedEntity.CompareTag ("MovingPlatform") ||
			collidedEntity.CompareTag ("Enemy")) {

			// Instancia las partículas de destrucción del ataque
			GameObject.Instantiate (particles, transform.position, Quaternion.identity);
			GameObject.Destroy (this.gameObject);	// Destruye el ataque

			// Si ha colisionado con una caja, la intenta destruir
			if (collidedEntity.CompareTag ("Crate")) {
				DestroyableCrate crate = collidedEntity.GetComponent <DestroyableCrate> ();
				if (crate)
					crate.destroyCrate ();
			} else if (collidedEntity.CompareTag ("Enemy")) { // Si colisiona con un enemigo, lo lanza por los aires
				// Mueve el collider para envolver el cuerpo del Goomba, útil en el caso de que el
				// Goomba estuviera atacando cuando recibió el ataque (al atacar, su cuerpo se adelanta
				// respecto a su posición real)
				SphereCollider theCollider = collidedEntity.GetComponent <SphereCollider> ();
				Transform actualBody = collidedEntity.transform.Find ("Armature/TopN/TransN/RotN");
				Vector3 actualPosition = actualBody.position;
				(collidedEntity.GetComponent <SphereCollider> ()).center = 
					collidedEntity.transform.InverseTransformPoint (actualPosition);

				// Desactiva los componentes principales del enemigo para evitar que se siga moviendo
				(collidedEntity.GetComponent <Animator> ()).enabled = false;
				(collidedEntity.GetComponent <GoombaAI> ()).enabled = false;

				// Hace que el Goomba se destruya al colisionar
				(collidedEntity.GetComponent <GoombaGeneral> ()).die ();

				if (collidedEntity.GetComponent <NavMeshAgent> () != null) {
					(collidedEntity.GetComponent <NavMeshAgent> ()).enabled = false;
					(collidedEntity.GetComponent <NavMeshGoomba> ()).enabled = false;
				}

				// Lanza al enemigo por los aires
				Rigidbody enemyRigidbody = collidedEntity.GetComponent <Rigidbody> ();
				enemyRigidbody.constraints = RigidbodyConstraints.None;
				enemyRigidbody.drag = 0;

				Vector3 throwDir = (enemyRigidbody.velocity).normalized * throwingForceH;
				enemyRigidbody.velocity = new Vector3 (throwDir.x, throwingForceV, throwDir.z);
				enemyRigidbody.angularVelocity = 
					new Vector3 (Random.Range (-maxThrowingAngularS, maxThrowingAngularS),
					Random.Range (-maxThrowingAngularS, maxThrowingAngularS),
					Random.Range (-maxThrowingAngularS, maxThrowingAngularS));
			}
		}
	}
}

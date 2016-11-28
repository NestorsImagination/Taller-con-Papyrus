// Script que controla las acciones del personaje

using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson {
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]

	public class ThirdPersonCharacter : MonoBehaviour {
		[SerializeField] private float airborneAcceleration = 20;		// Aceleración en el aire
		[SerializeField] private float maxSpeed = 10;					// Velocidad máxima alcanzable
		[SerializeField] private float charTurnSpeed = 720;				// Velocidad de giro del personaje
		[SerializeField] private float jumpPower = 15f;					// Fuerza de salto
		[SerializeField] private float gravityMultiplier = 2.5f;		// Multiplicador de gravedad
		[SerializeField] private float animSpeedMultiplier = 1f;		// Multiplicador de velocidad de las animaciones
		[SerializeField] private float groundCheckDistance = 1f;		// Altura a superar para considerar que el personaje está
																		// en el aire

		[SerializeField] private GameObject attackPrefab;				// Referencia al objeto que se lanzará al atacar
		[SerializeField] private GameObject attackGenerator;			// Punto en el que se generará el ataque
		[SerializeField] private float attackSpeed = 15f;				// Velocidad a la que se lanzará el ataque

		[SerializeField] private float fallSoundSpeed = -1;				// Velocidad vertical a la que debe caer el personaje
																		// para reproducir el sonido de caída

		private int attackState = Animator.StringToHash("Attack.Attacking");	// Estado de ataque del personaje

		private Rigidbody rb;
		private Animator animator;
		private bool isGrounded = true;				// True si el personaje está sobre el suelo
		private float originGroundCheckDistance;	// Distancia para comprobar si el personaje toca sobre el suelo mientras cae
		private float turnAmount;					// Cantidad de giro que el personaje debe realizar para encarar la dirección
													// objetivo
		private float forwardAmount;				// Velocidad del personaje al seguir la dirección objetivo
		private Vector3 groundNormal;				// Vector normal del suelo bajo el personaje
		private Vector3 lastMovPlatformPosition;	// Posición última de una plataforma móvil
		private GameObject movingPlatform;			// Si no es null guarda la plataforma móvil sobre la que se encuentra el personaje

		private bool jumpFinished;

		private CharacterSounds papSounds;			// Componente de reproducción de los efectos de sonido

		// llamado al comienzo
		void Start() {
			animator = GetComponent<Animator>();
			rb = GetComponent<Rigidbody>();

			originGroundCheckDistance = groundCheckDistance;

			papSounds = GetComponent<CharacterSounds> ();
		}

		// Método llamado en cada frame físico por ThirdPersonUserControl para comunicar los inputs
		public void Move(Vector3 move, bool jump, bool attack) {
			// Convierte el vector move (global) a la rotación requerida para que el personaje lo
			// encare (turnAmount) y la velocidad a la que se tendrá que mover el personaje (fowardAmount)

			// Se transforma move para ser relativo a la dirección encarada por el personaje
			move = transform.InverseTransformDirection(move);
			// Se comprueba si el personaje está sobre el suelo
			CheckGroundStatus ();
			// Proyecta el vector de movimiento según la normal del suelo para adaptar el movimiento 
			move = Vector3.ProjectOnPlane(move, groundNormal);
			// Rotación que debe realizar el personaje para encarar la dirección objetivo
			turnAmount = Mathf.Atan2(move.x, move.z);
			// Velocidad del personaje
			forwardAmount = move.z;

			// Ayuda al personaje a girar más rápido (adicional a la aplicada por el Root Motion de
			// la animación de giro del personaje, si existe
			ApplyExtraTurnRotation();

			// Se controla de forma diferente el personaje según si está sobre tierra o en el aire
			if (isGrounded)
			{
				HandleGroundedMovement(jump);
			}
			else
			{
				HandleAirborneMovement(jump);
			}

			// Actualiza la animación del personaje
			UpdateAnimator(move, attack);
		}

		// Método que actualiza la animación del personaje
		void UpdateAnimator(Vector3 move, bool attack) {
			// Velocidad del personaje
			animator.SetFloat("Forward", forwardAmount, 0.05f, Time.deltaTime);
			// animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
			// Personaje en tierra o en el aire
			animator.SetBool("OnGround", isGrounded);

			// Si se desea atacar
			if (attack) {
				// Si el personaje no está ya atacando
				if ((animator.GetCurrentAnimatorStateInfo (1)).fullPathHash != attackState) {
					animator.SetTrigger ("Attack");	// Ataca
				}
			}
				
			// Actualiza la velocidad de las animaciones
			animator.speed = animSpeedMultiplier;
		}

		// Control del personaje en el aire
		void HandleAirborneMovement(bool jump) {
			// Aplica un extra de gravedad
			Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
			rb.AddForce(extraGravityForce);

			// Comprueba si está en tierra (diferentes distancias según esté o no cayendo)
			groundCheckDistance = rb.velocity.y < 0 ? originGroundCheckDistance : 0.1f;

			// Se controla que al soltar el botón de salto se frente el personaje (al pulsar más tiempo
			// el botón el personaje saltará más alto)
			if (!jumpFinished && rb.velocity.y > 0) {
				if (!jump) {	// Si se acaba de soltar el botón de salto
					// Frena la velocidad vertical del personaje a la mitad
					rb.velocity = new Vector3 (rb.velocity.x, rb.velocity.y / 2, rb.velocity.z);
					jumpFinished = true;	// No se desea seguir saltando
				}
			}

			// Aplica una fuerza en la dirección deseada por el jugador para controlar mejor el personaje en el aire
			rb.AddForce(forwardAmount * transform.forward * airborneAcceleration);

			// Asegura que la velocidad del personaje no exceda la velocidad máxima dada
			Vector3 newVel = rb.velocity;
			newVel.y = 0;
			newVel = Vector3.ClampMagnitude (newVel, maxSpeed);
			rb.velocity = new Vector3 (newVel.x, rb.velocity.y, newVel.z);
		}

		// Controla el movimiento del personaje en tierra
		void HandleGroundedMovement(bool jump) {
			// Si se desea saltar y el personaje está en tierra...
			if (jump && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				// Reproduce el sonido de saltar
				papSounds.makeSound (CharacterSounds.CharSoundsEnum.Jump);
				// El personaje salta
				rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
				isGrounded = false;
				animator.applyRootMotion = false;	// Se deja de aplicar Root Motion
				jumpFinished = false;				// Si el jugador suelta el botón de salto esta variable se pondrá a
													// true, acortando la duración del salto
				groundCheckDistance = 0.1f;			// Evita que se considere al personaje como "en tierra" en los próximos frames
			}
		}

		// Ayuda a rotar el personaje en la dirección objetivo (adicional a la producida por el Root Motion si existe)
		void ApplyExtraTurnRotation() {
			// Se incrementa la velocidad de rotación en aire para facilitar el manejo
			float turnSpeed = charTurnSpeed;
			if (!isGrounded)
				turnSpeed *= 3;

			// Se rota el personaje hacia la dirección indicada
			transform.Rotate (0, turnAmount * turnSpeed * Time.deltaTime, 0);
		}

		// Se obtiene la información relacionada con el suelo que se encuentra bajo el personaje
		void CheckGroundStatus() {
			// Información del rayo
			RaycastHit hitInfo;

			// Se lanza un rayo desde un poco más arriba del origen del personaje hacia abajo
			// (suponiendo que su origen está a la altura de sus pies)
			if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
			{
				//Si se ha detectado suelo...

				// Se obtiene el vector normal del suelo bajo el personaje
				groundNormal = hitInfo.normal;
				// Si el vector normal del suelo no está inclinado a más de 45º
				if (groundNormal.y > .707f) {
					// Si antes estaba en el aire, ahora está en suelo
					if (!isGrounded) {
						isGrounded = true;

						// Si la velocidad era mayor que la dada en fallSoundSpeed, hace el sonido de caída
						if (rb.velocity.y < fallSoundSpeed)
							papSounds.makeSound (CharacterSounds.CharSoundsEnum.Fall);
					}

					animator.applyRootMotion = true;	// Aplica Root Motion a partir de ahora
				} else {								// Si era mayor de 45º, se considera que está en el aire
					isGrounded = false;
					groundNormal = Vector3.up;
					animator.applyRootMotion = false;
				}
			}
			else
			{
				// Está en el aire
				isGrounded = false;
				groundNormal = Vector3.up;
				animator.applyRootMotion = false;
			}
		}

		// Justo al colisionar
		void OnCollisionEnter(Collision other) {
			// Si el objeto colisionado es una plataforma que se mueve
			if(other.gameObject.tag == "MovingPlatform"){
				lastMovPlatformPosition = other.transform.position;	// Guarda la posición actual de la plataforma
				movingPlatform = other.gameObject;					// Guarda la plataforma
			}
		}
			
		// Al dejar de colisionar
		void OnCollisionExit(Collision other) {
			// Si el objeto con el que se deja de colisionar es una plataforma que se mueve
			if(other.gameObject.tag == "MovingPlatform") {
				// Actualiza la posición del personaje según la última variación de posiciones de la plataforma
				Vector3 currentMovement = (movingPlatform.transform.position - lastMovPlatformPosition);
				currentMovement.y = 0;
				rb.MovePosition (transform.position + currentMovement);
				// Olvida la plataforma
				movingPlatform = null;
			}
		}    

		// Llamado en cada frame físico
		void FixedUpdate () {
			// Si se está colisionando con una plataforma que se mueve...
			if (movingPlatform != null) {
				// Actualiza la posición del personaje según la última variación de posiciones de la plataforma
				Vector3 currentMovement = (movingPlatform.transform.position - lastMovPlatformPosition);
				currentMovement.y = 0;
				rb.MovePosition (transform.position + currentMovement);

				// Actualiza la última posición de la plataforma
				lastMovPlatformPosition = movingPlatform.transform.position;
			}
		}

		// Método que para que el personaje lanze un ataque desde su mano
		public void throwAttack () {
			// Instancia el ataque y le da velocidad
			GameObject attack = (GameObject) GameObject.Instantiate (attackPrefab, attackGenerator.transform.position, transform.rotation);
			(attack.transform.GetComponent<Rigidbody> ()).velocity = transform.forward * attackSpeed;
		}
	}
}
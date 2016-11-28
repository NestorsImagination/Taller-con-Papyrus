// Clase que contiene la inteligencia artificial de los Goomba

using UnityEngine;

public class GoombaAI : MonoBehaviour {
	[SerializeField] private float distanceToDetect;	// Distancia a la que detecta al jugador
	[SerializeField] private float timeToDetect;		// Tiempo que pasa entre intentos de detección del jugador
	[SerializeField] private float timeToAttack;		// Tiempo que pasa entre ataques
	[SerializeField] private float attackRange;			// Alcance de los ataques

	private float detectTimer;	// Temporizador para intentar detectar al jugador
	private float attackTimer;	// Temporizador para volver a atacar
	private GameObject player;	// El jugador
	private NavMeshGoomba agent;	// El script de control del movimiento del Goomba

	void Start () {
		detectTimer = timeToDetect;
		attackTimer = timeToAttack;
		player = GameObject.FindGameObjectWithTag ("Player");
		agent = GetComponent <NavMeshGoomba> ();
	}

	void Update () {
		detect ();
		updateAttack ();
	}

	private void detect () {
		detectTimer -= Time.deltaTime;

		// Distancia al jugador
		float distanceToPlayer = Vector3.Distance (transform.position, player.transform.position);

		// Si la distancia es menor que el rango de ataque, gira el Goomba para encarar al jugador
		if (distanceToPlayer < attackRange)
			agent.lookAt (player.transform.position);

		// Si el temporizador de detección ha llegado a 0
		if (detectTimer <= 0) {
			detectTimer = timeToDetect;	// Restaura el temporizador de detección

			// Si la distancia al jugador es suficientemente pequeña para detectarlo
			if (distanceToPlayer < distanceToDetect) {
				// Si el jugador está suficientemente cerca como para atacarlo
				if (distanceToPlayer < attackRange) {
					// Si el temporizador de ataque ha llegado a 0
					if (attackTimer <= 0) {
						attackTimer = timeToAttack;	// Restaura el temporizador de ataque
						agent.attack ();			// Ataca
					}
				} else 	// Si el jugador está demasiado lejos para atacarlo, dirige el Goomba hacia el jugador
					agent.setDestination (player.transform.position);
			}
		}
	}

	// Actualiza el temporizador de ataque
	private void updateAttack () {
		if (attackTimer > 0)
			attackTimer -= Time.deltaTime;
	}
}

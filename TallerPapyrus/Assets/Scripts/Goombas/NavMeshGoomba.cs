// Clase que contiene las funcionalidades de pathfinding del Goomba
using UnityEngine;

public class NavMeshGoomba : MonoBehaviour {
	private NavMeshAgent agent;	// El agente tipo NavMeshAgent
	private Animator animator;	// El Animator Controller del Goomba

	void Start () {
		agent = GetComponent <NavMeshAgent> ();
		animator = GetComponent <Animator> ();

		animator.applyRootMotion = false;	// No se aplica root motion
	}

	// Actualiza la velocidad de la animación de movimiento según la velocidad deseada por el agente
	void Update () {
		animator.SetFloat ("Speed", agent.desiredVelocity.magnitude);
	}

	// Destino del pathfinding
	public void setDestination (Vector3 dest) {
		agent.enabled = true;
		agent.destination = dest;
	}

	// Rota el personaje para que encare la posición dada
	public void lookAt (Vector3 target) {
		Vector3 relativePos = target - transform.position;
		relativePos.y = 0;
		Quaternion rotation = Quaternion.LookRotation (relativePos);
		transform.rotation = rotation;
	}

	// Ataca
	public void attack () {
		agent.enabled = false;
		animator.SetFloat ("Speed", 0f);
		animator.SetTrigger ("Attack");
	}
}
// Script que maneja el movimiento de una plataforma móvil que va y viene entre dos posiciones
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
	[SerializeField] Vector3 targetPosition;
	[SerializeField] float movTime;
	[SerializeField] float idleTime;
	[SerializeField] float nearEnough;
	[SerializeField] float maxSpeed;

	private Vector3 originPosition;
	private float idleTimer;
	private Rigidbody rb;
	private bool returning = false;
	private bool movementStart;
	private Vector3 speed;
	private bool moving;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		speed = Vector3.zero;
		moving = false;
		movementStart = false;
		idleTimer = idleTime;
		originPosition = transform.position;
	}

	void Update () {
		if (movementStart && speed == Vector3.zero) {
			moving = false;
			movementStart = false;
			idleTimer = idleTime;
		}

		if (idleTimer > 0) {
			idleTimer -= Time.deltaTime;
			if (idleTimer <= 0) {
				idleTimer = 0;
				returning = !returning;
				moving = true;
			}
		}
	}

	void FixedUpdate () {
		if (moving) {
			if (!returning) {
				rb.MovePosition (Vector3.SmoothDamp (transform.position, targetPosition, ref speed, movTime, maxSpeed));
				if (Vector3.Distance (transform.position, targetPosition) < nearEnough) {
					rb.MovePosition (targetPosition);
					speed = Vector3.zero;
					moving = false;
				}
			} else {
				rb.MovePosition (Vector3.SmoothDamp (transform.position, originPosition, ref speed, movTime, maxSpeed));
				if (Vector3.Distance (transform.position, originPosition) < nearEnough) {
					rb.MovePosition (originPosition);
					speed = Vector3.zero;
					moving = false;
				}
			}

			if (movementStart == false)
				movementStart = true;
		}
	}
}
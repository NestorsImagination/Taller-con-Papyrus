// Espaguetis dorados

using UnityEngine;

public class SpaghettiScript : MonoBehaviour {
	[SerializeField] private float rotSpeed;
	[SerializeField] private float maxUpAndDown = 1.0f;
	[SerializeField] private float upAndDownSpeed = 200.0f;
	[SerializeField] private GameObject winText;

	private Vector3 initialPos;
	private float angle = 0.0f;
	private const float toDegrees = Mathf.PI/180;

	void Start () {
		initialPos = transform.position;
	}

	// Se otorga un movimiento suave de arriba a abajo a los espaguetis con la función seno
	// para hacerlos más apetitosos
	void Update () {
		transform.Rotate (new Vector3 (0, rotSpeed, 0));

		angle += upAndDownSpeed * Time.deltaTime;
		if (angle > 360) angle -= 360;
		transform.position = new Vector3 (initialPos.x, initialPos.y + maxUpAndDown * Mathf.Sin(angle * toDegrees), initialPos.z);
	}

	// El jugador consigue los espaguetis
	void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.tag == "Player") {
			GameObject.Destroy (this.gameObject);
			winText.SetActive (true);
		}
	}
}
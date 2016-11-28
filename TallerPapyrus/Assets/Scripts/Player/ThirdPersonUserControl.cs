// Script que procesa los imputs del jugador

using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson {
	
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour {
		private ThirdPersonCharacter character; 	// Referencia al script ThirdPersonCharacter del objeto
		private Transform theCamera;                  	// Referencia a la cámara principal
		private Vector3 camFoward;             		// Dirección a donde está mirando la camara

		private Vector3 movement;					// Dirección a donde ha de moverse el personaje
		private bool jump;							// Trigger saltar
		private bool attack; 						// Trigger atacar

		// Obtiene objetos al comenzar
        private void Start() {
            // Obtiene la cámara principal
            if (Camera.main != null) {
                theCamera = Camera.main.transform;
            } else {
                Debug.LogWarning("Warning: No camera found");
            }

			// Obtiene el script ThirdPersonCharacter
            character = GetComponent<ThirdPersonCharacter>();
        }
			
		// Llamado en cada frame
        private void Update() {
			if (Input.GetButtonDown ("Fire"))	// Trigger de atacar
				attack = true;
        }

		// Llamado en cada cálculo físico
        private void FixedUpdate() {
            // Lee inputs de movimiento
			jump = Input.GetButton ("Jump");
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");

            // Calcula la dirección a la que se moverá el personaje
            if (theCamera != null) {
                // Calcula la dirección relativa a la cámara

				// camFoward es el vector de dirección de la cámara eliminando 
				// la componente vertical y normalizado
                camFoward = Vector3.Scale(theCamera.forward, new Vector3(1, 0, 1)).normalized;

				// Dirección de movimiento objetivo según la cámara
                movement = v*camFoward + h*theCamera.right;

				// Normaliza el vector de movimiento
				if (movement != Vector3.zero)
					movement = movement.normalized;
            }
				
			// Si está pulsado shift, el personaje se moverá a la mitad de velocidad (andará)
	        if (Input.GetKey(KeyCode.LeftShift)) movement *= 0.5f;

            // Pasa los parámetros al script ThirdPersonCharacter
            character.Move(movement, jump, attack);

			// Limpia el trigger de ataque
			if (attack)
				attack = false;
        }
    }
}
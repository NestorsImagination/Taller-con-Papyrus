using UnityEngine;
using System.Collections;

public class Script_0 : MonoBehaviour {
	[SerializeField] private Material newMat;

	private bool surprise = false;
	private MeshRenderer mRenderer;

	void Start () {
		mRenderer = GetComponent<MeshRenderer> ();
	}

	void OnCollisionEnter (Collision col) {
		if (!surprise) {
			if (col.gameObject.tag == "Player") {
				surprise = true;
				Material[] mats = mRenderer.materials;
				mats [0] = newMat;
				mRenderer.materials = mats;
				(GetComponent<AudioSource> ()).Play ();
			}
		}
	}

	void FixedUpdate () {
		if (surprise) {
			Material[] mats = mRenderer.materials;
			Color col = (mats [0]).color;
			col.a -= 0.01f;
			mats [0].color = col;
			mRenderer.materials = mats;

			if (col.a <= 0) {
				GameObject.Destroy (this.gameObject);
			}
		}
	}
}

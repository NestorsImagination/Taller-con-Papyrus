// Script que maneja los efectos de sonido del personaje jugador
using UnityEngine;

public class CharacterSounds : MonoBehaviour {
	public enum CharSoundsEnum {Jump, Fall, Step}	// Lista de sonidos disponibles

	private AudioSource jumpAudio, fallAudio, stepAudio;	// Los sonidos disponibles

	// Obtiene los sonidos al comenzar
	void Start () {
		AudioSource[] sounds = GetComponents<AudioSource> ();

		fallAudio = sounds [0];
		jumpAudio = sounds [1];
		stepAudio = sounds [2];
	}

	// Método público para reproducir el sonido de las pisadas al andar
	public void stepSound () {
		makeSound (CharSoundsEnum.Step);
	}

	// Método para reproducir un efecto de sonido
	public void makeSound (CharSoundsEnum sound) {
		switch (sound) {
		case CharSoundsEnum.Jump:
			jumpAudio.Play ();
			break;
		case CharSoundsEnum.Fall:
			fallAudio.Play ();
			break;
		case CharSoundsEnum.Step:
			stepAudio.Play ();
			break;
		}
	}
}
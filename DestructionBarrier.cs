using UnityEngine;
using System.Collections;

public class DestructionBarrier : MonoBehaviour {

	// destroy or alter things if they cross this barrier
	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Player") {
			StartCoroutine (Camera.main.GetComponent<GameManager> ().OnPlayerDead (this.gameObject, other.gameObject));
		} else if (other.gameObject.tag == "Spectrum") {
			// spectrum callback check actions (DO NOT destroy the spectrum if it moves into this barrier!)
		} else {
			Destroy (other.gameObject);
		}
	}

}

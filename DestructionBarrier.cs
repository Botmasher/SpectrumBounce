using UnityEngine;
using System.Collections;

public class DestructionBarrier : MonoBehaviour {

	// destroy or alter things if they cross this barrier
	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Player") {
			// do not attack player
		} else if (other.gameObject.tag == "Spectrum") {
			// spectrum callback check actions (DO NOT destroy the spectrum if it moves into this barrier!)
		} else {
			Destroy (other.gameObject);
		}
	}

}

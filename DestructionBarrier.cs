using UnityEngine;
using System.Collections;

public class DestructionBarrier : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D other) {
		if (gameObject.tag == "Player") {
			GameManager.OnPlayerDead (this.gameObject, other.gameObject);
		} else {
			Destroy (other.gameObject);
		}
	}

}

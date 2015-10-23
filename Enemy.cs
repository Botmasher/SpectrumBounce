using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {


	void Update() {
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Player") {
			GameManager.OnPlayerDead (this.gameObject, other.gameObject);
		}
	}

	IEnumerator SelfDestruct () {
	}
}

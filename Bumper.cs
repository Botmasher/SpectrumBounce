﻿using UnityEngine;
using System.Collections;

public class Bumper : MonoBehaviour {

	public float lifetime;
	public bool isEnemy;
	private bool blinking = false;
	
	void Start () {
		// notify player inventory if this was placed by player
		if (!isEnemy) {
			BallPlay.bumpersDeployed ++;
		}
		StartCoroutine ("SelfDestruct");
	}
	
	void Update () {
		// count until self destruction
		lifetime -= Time.deltaTime;
		if (lifetime <= 1f && !blinking) {
			StartCoroutine (Blink ());
		}
	}

	// blink rapidly before self destruction
	IEnumerator Blink () {
		blinking = true;
		this.GetComponent<SpriteRenderer> ().enabled = false;
		yield return new WaitForSeconds (lifetime * 0.1f);
		this.GetComponent<SpriteRenderer> ().enabled = true;
		blinking = false;
	}

	IEnumerator SelfDestruct () {
		yield return new WaitForSeconds (lifetime);
		// notify player inventory if this was placed by player
		if (!isEnemy) {
			BallPlay.bumpersDeployed --;
		}
		Destroy (this.gameObject);
	}

	// push player upwards 
	void OnCollisionEnter2D (Collision2D collision) {
		if (collision.collider.gameObject.tag == "Player" && collision.contacts[0].point.y > this.transform.position.y) {
			// check that player is moving slow enough to add upward force
			if(Mathf.Abs(collision.collider.GetComponent<Rigidbody2D>().velocity.y) < 10f)
			{
				collision.collider.GetComponent<Rigidbody2D> ().AddForce (Vector2.up * 60f);
			}
		}
	}

}
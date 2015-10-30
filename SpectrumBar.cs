using UnityEngine;
using System.Collections;

public class SpectrumBar : MonoBehaviour {

	public AudioClip hurtSound;
	public GameObject explosion;


	void OnCollisionEnter2D (Collision2D hit) {

		// check if this is an enemy - kill it and take a hit if it is
		if (hit.collider.gameObject.tag == "Enemy") {
			Instantiate (explosion, hit.collider.gameObject.transform.position, Quaternion.identity);
			Destroy (hit.collider.gameObject);

			// get hurt and tell music to take off a health
			GetComponent<AudioSource>().clip = hurtSound;
			GetComponent<AudioSource>().Play();
			WorldMusic.TakeOneHealth();
		}

	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Enemy") {
			Instantiate (explosion, other.gameObject.transform.position, Quaternion.identity);
			Destroy (other.gameObject);
			
			// get hurt and tell music to take off a health
			GetComponent<AudioSource>().clip = hurtSound;
			GetComponent<AudioSource>().Play();
			WorldMusic.TakeOneHealth();
		}
	}

}
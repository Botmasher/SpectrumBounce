using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public AudioClip explosionSound;
	
	void Start () {
		// play that booming sound
		GetComponent<AudioSource>().clip = explosionSound;
		GetComponent<AudioSource>().Play ();

		// start self destruct sequence
		StartCoroutine ("SelfDestruct");
	}

	// wait then destroy self
	IEnumerator SelfDestruct () {
		yield return new WaitForSeconds (3f);
		Destroy (this.gameObject);
	}

}

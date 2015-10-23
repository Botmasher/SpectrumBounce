using UnityEngine;
using System.Collections;

public class Bumper : MonoBehaviour {

	public float lifetime;
	private bool blinking = false;

	void Start () {
		BallPlay.bumpersDeployed ++;
		StartCoroutine ("SelfDestruct");
	}
	
	void Update () {
		lifetime -= Time.deltaTime;
		if (lifetime <= 1f && !blinking) {
			StartCoroutine (Blink ());
		}
	}

	IEnumerator Blink () {
		blinking = true;
		this.GetComponent<SpriteRenderer> ().enabled = false;
		yield return new WaitForSeconds (lifetime * 0.1f);
		this.GetComponent<SpriteRenderer> ().enabled = true;
		blinking = false;
	}

	IEnumerator SelfDestruct () {
		yield return new WaitForSeconds (lifetime);
		BallPlay.bumpersDeployed --;
		Destroy (this.gameObject);
	}

}
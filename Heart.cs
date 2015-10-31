using UnityEngine;
using System.Collections;

public class Heart : MonoBehaviour {

	float randomX;
	float randomY;
	
	void Start () {
		StartCoroutine (SelfDestruct());
		randomX = Random.Range (10f,10f);
		randomY = Random.Range (0f,1f)>0.5f ? 4.4f : -4.4f;
		this.transform.position = new Vector3 (randomX, randomY, transform.position.z);
	}

	void Update () {
		transform.Rotate (Vector3.forward * 100f * Time.deltaTime);
	}

	// add health if touched by the music spectrum
	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Spectrum") {
			WorldMusic.AddOneHealth();
			GetComponent<AudioSource>().Play ();
			Destroy (this.gameObject);
		}
	}

	IEnumerator SelfDestruct () {
		yield return new WaitForSeconds (4f);
		Destroy (this.gameObject);
	}

}
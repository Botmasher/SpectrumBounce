using UnityEngine;
using System.Collections;

public class LightTestSpectrum : MonoBehaviour {

	public GameObject explosion;

	public static Color spectrumColor;

	// Use this for initialization
	void Start () {
		spectrumColor = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
		// update every spectrum block's color
		GetComponent<MeshRenderer>().material.color = spectrumColor;
		spectrumColor = Color.Lerp (GetComponent<MeshRenderer>().material.color, Color.white, 0.05f*Time.deltaTime);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Enemy") {
			spectrumColor = Color.red;
			Instantiate (explosion, this.transform.position, this.transform.rotation);
			LightTestPlayer.gameOver = true;
		}
	}
}

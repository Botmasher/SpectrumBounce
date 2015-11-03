using UnityEngine;
using System.Collections;

public class PaddleSpectrum : MonoBehaviour {

	public GameObject explosion;
	public PaddleTestMusicManager musicScript;

	public static Color spectrumColor;

	private bool pitchingDown;

	void Start () {
		spectrumColor = Color.white;
		pitchingDown = false;
	}

	void Update () {
		if (pitchingDown) {
			if (PaddleTestMusicManager.songPitch < 0.5f) {
				StartCoroutine ("GameOver");
			} else {
				GameObject.FindGameObjectWithTag("Music").GetComponent<PaddleTestMusicManager>().PitchDown();
			}
		}

		if (!Paddle.gameOver) {
			spectrumColor = Color.Lerp (spectrumColor, Color.white, 0.01f * Time.deltaTime);
			GetComponent<SpriteRenderer>().color = spectrumColor;
		}

	}

	void OnCollisionEnter2D (Collision2D other) {
		Destroy (other.collider.gameObject);
		//Instantiate (explosion, other.collider.transform.position, Quaternion.identity);
		StartCoroutine ("GetHit");
	}

	IEnumerator GetHit () {
		pitchingDown = true;
		spectrumColor = Color.red;
		yield return new WaitForSeconds (0.2f);
		pitchingDown = false;
	}

	IEnumerator GameOver () {
		Paddle.gameOver = true;
		yield return new WaitForSeconds (4f);
		Application.LoadLevel (Application.loadedLevel);
	}

}

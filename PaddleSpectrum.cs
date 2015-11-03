using UnityEngine;
using System.Collections;

public class PaddleSpectrum : MonoBehaviour {

	// objects to spawn and manipulate
	public GameObject explosion;

	// changing color of all spectrum blocks on collision
	public static Color spectrumColor;

	// for notifying music manager of pitch change on collision
	private bool pitchingDown;
	private bool pitchingUp;
	
	void Start () {
		spectrumColor = Color.white;
		pitchingDown = false;
		pitchingUp = false;
	}

	void Update () {
		// speed pitch down to 0, or do gameover if getting close to 0
		if (pitchingDown) {
			if (PaddleTestMusicManager.songPitch < 0.40f) {
				StartCoroutine ("GameOver");
			} else {
				GameObject.FindGameObjectWithTag("Music").GetComponent<PaddleTestMusicManager>().PitchDown();
			}
		}

		// turn the speed pitch up
		if (pitchingUp) {
			GameObject.FindGameObjectWithTag("Music").GetComponent<PaddleTestMusicManager>().PitchUp();
		}

		// change while game is running
		if (!Paddle.gameOver) {
			spectrumColor = Color.Lerp (spectrumColor, Color.white, 0.01f * Time.deltaTime);
			GetComponent<SpriteRenderer>().color = spectrumColor;
		}

	}

	void OnCollisionEnter2D (Collision2D other) {
		if (other.collider.gameObject.tag == "Enemy") {
			Destroy (other.collider.gameObject);
			//Instantiate (explosion, other.collider.transform.position, Quaternion.identity);
			StartCoroutine ("GetHit");
		} else if (other.collider.gameObject.tag == "Pickup") {
			Destroy (other.collider.gameObject);
			StartCoroutine ("GetHelped");
		}
	}

	IEnumerator GetHit () {
		pitchingDown = true;
		spectrumColor = Color.red;
		yield return new WaitForSeconds (0.2f);
		pitchingDown = false;
	}

	IEnumerator GetHelped () {
		pitchingUp = true;
		spectrumColor = Color.blue;
		yield return new WaitForSeconds (0.4f);
		pitchingUp = false;
	}

	IEnumerator GameOver () {
		Paddle.gameOver = true;
		yield return new WaitForSeconds (4f);
		Application.LoadLevel (Application.loadedLevel);
	}

}

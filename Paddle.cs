using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Paddle : MonoBehaviour {

	// objects to use
	public GameObject backgroundImage;
	public List<GameObject> enemies = new List<GameObject>();
	public GameObject explosion;

	// player movement
	public float moveSpeed;
	private float horiz;

	// starting size for beat thumping
	private Vector2 initScale;
	private Vector2 backgroundInitScale;

	// gamewide player stuff
	public static bool gameOver;
	public static int score;


	void Start () {
		gameOver = false;
		score = 0;

		// starting scale for thumping objects
		initScale = this.transform.localScale;
		backgroundInitScale = backgroundImage.transform.localScale;

		// start periodically spawning enemies
		StartCoroutine (SpawnEnemies());
	}


	void Update () {
		// move player on input
		horiz = Input.GetAxis ("Horizontal");
		transform.Translate (Vector2.right * horiz * moveSpeed * Time.deltaTime);

		// bounce objects to the music
		BounceWithMusic (this.transform, initScale);
		BounceWithBeat (backgroundImage.transform, backgroundInitScale);

		// do game over stuff
		if (gameOver) {
			GameObject.FindGameObjectWithTag("Music").GetComponent<PaddleTestMusicManager>().PitchDown();
		}
	}


	void OnCollisionEnter2D (Collision2D other) {
		if (other.collider.gameObject.tag == "Enemy") {
			Destroy (other.collider.gameObject);
			Instantiate (explosion, other.collider.transform.position, Quaternion.identity);
			score ++;
		}
	}


	// change object scale as music spectrum is averaged
	void BounceWithMusic (Transform obj, Vector2 initialScale) {
		obj.localScale = Vector2.Lerp (initialScale, new Vector2 (initialScale.x + 100f*PaddleTestMusicManager.GetSpectrumAverage(), initialScale.y + 100f * PaddleTestMusicManager.GetSpectrumAverage()), Time.deltaTime);
	}

	// change object scale as music beat is counted
	void BounceWithBeat (Transform obj, Vector2 initialScale) {
		if (PaddleTestMusicManager.isOnBeat == true) {
			obj.localScale = initialScale*1.07f;
		} else {
			obj.localScale = Vector2.Lerp (obj.localScale, initialScale, Time.deltaTime);
		}
	}

	IEnumerator SpawnEnemies () {
		yield return new WaitForSeconds (Random.Range (0.2f,1f));
		Instantiate ( enemies[Random.Range(0,enemies.Count)], new Vector3 ( Random.Range(6f,-6f), 6f, 0f), Quaternion.Euler(new Vector3 (0f,0f, Random.Range (0f, 270f)) ) );
		if (!gameOver) {
			StartCoroutine (SpawnEnemies());
		}
		yield return null;
	}

}
using UnityEngine;
using System.Collections;

public class Level01Manager : MonoBehaviour {

	// for enemy during normal gameplay
	public GameObject enemy;
	private bool enemySpawned;

	// for boss when timer runs low
	public GameObject boss;
	private bool isBoss;
	private bool levelOver;

	// level sfx
	public AudioClip victorySound;


	void Start () {
		enemySpawned = false;
		isBoss = false;
		levelOver = false;
	}

	void Update () {

		// decide if time for enemy to spawn
		if (!MusicManager.almostOver && !enemySpawned) {
			StartCoroutine (SpawnEnemy ());
		}

		// spawn boss when song reaches almost over mark
		if (MusicManager.almostOver && !isBoss) {
			Instantiate (boss, new Vector2(14f, 4f), Quaternion.identity);
			isBoss = true;
		} else if (isBoss) {
			// play boss music distortion
			GameObject.Find("Music Manager").GetComponent<MusicManager>().DistortionUp();
		}

		// win if you outlast the time
		if (MusicManager.almostOver && MusicManager.songLeft < 35f && !levelOver && !GameManager.gameOver) {
			// run gamemanager win routines
			StartCoroutine(Camera.main.GetComponent<GameManager>().YouWin());
			// play the sound of a winner
			GetComponent<AudioSource>().clip = victorySound;
			GetComponent<AudioSource>().Play ();
			// do not access this branch to doublewin
			levelOver = true;
		}
	}

	// instantiate an enemy
	IEnumerator SpawnEnemy () {
		enemySpawned = true;
		yield return new WaitForSeconds (Random.Range (6f, 11f));
		// spawn enemy just offscreen x and at a y that can attack player
		Instantiate (enemy, new Vector3 (Camera.main.ViewportToWorldPoint(Vector3.one).x, Random.Range(-0.5f,6.0f), 0f), Quaternion.identity);
		enemySpawned = false;
	}

}

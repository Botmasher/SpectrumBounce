using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldOverseer : MonoBehaviour {
	
	// objects referenced in script
	public GameObject ball;			// main player bouncing ball
	public GameObject enemy;		// main grunt enemy
	public GameObject explosion;	// particle system to instantiate on hits
	private CameraCrew director;	// crew instance for directing camera and lighting
	
	// gamewide control flow
	public static bool gameOver;
	private bool enemySpawned;
	
	// UI updates
	public Image screenFader;
	public Text centerText;
	
	
	// crew class for directing camera and lighting
	private class CameraCrew {
		public Camera mainCamera;
		public Light mainLight;
		public WorldMusic mainMusic;
		
		// instance constructors
		public CameraCrew (Camera cam, Light light, WorldMusic music) {
			mainCamera = cam;
			mainLight = light;
			mainMusic = music;
		}
		public CameraCrew () {
			mainCamera = Camera.main;
			mainLight = GameObject.Find ("Directional Light").GetComponent<Light> ();
			mainMusic = GameObject.FindGameObjectWithTag ("Music").GetComponent<WorldMusic> ();
		}
		
		// smooth follow the x and y position of a target object, e.g. player
		public void TrackObject (GameObject obj) {
			mainCamera.transform.position = Vector3.Lerp (mainCamera.transform.position, new Vector3(obj.transform.position.x, obj.transform.position.y, mainCamera.transform.position.z), Time.deltaTime);
		}
	}
	
	// Use this for initialization
	void Start () {
		gameOver = false;
		enemySpawned = false;
		director = new CameraCrew ();
		
		// make UI screen invisible
		screenFader.CrossFadeAlpha (0f, 1f, false);
		centerText.CrossFadeAlpha (0f, 0f, true);
	}
	
	void Update () {
		//director.TrackObject (ball);
		
		// do general gameover stuff (also see OnPlayerDead)
		if (gameOver) {
			// change music mood to disappointment
			director.mainMusic.PitchDown();
			// gameover text screen fade
			screenFader.CrossFadeAlpha (1f, 2f, false);
			centerText.text = "You Are Dead";
			centerText.CrossFadeAlpha (1f, 2f, false);
		}
		
		// decide if time for enemy to spawn
		if (!enemySpawned) {
			StartCoroutine (SpawnEnemy ());
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
	
	// do scary things when player dies
	public IEnumerator OnPlayerDead (GameObject killer, GameObject player) {
		// tell update to perform gameover actions over time
		gameOver = true;
		Instantiate (explosion, player.transform.position, Quaternion.identity);
		
		// turn off the player
		player.GetComponent<Collider2D>().enabled = false;
		player.GetComponent<SpriteRenderer>().enabled = false;
		
		// wait and reset level (fade scene and music actions happen in update)
		yield return new WaitForSeconds (3f);
		Application.LoadLevel (Application.loadedLevel);
		yield return null;
	}
	
	// do fun things when player wins
	public IEnumerator YouWin () {
		Debug.Log ("You Win!");
		yield return null;
	}
	
	/**
	 *	Check if the current mouse position is not cluttered by collider objects
	 *	Use this to find out if the spot you're clicking is a good place for a new object
	 */
	public static bool ThisSpotHasObjects (Vector2 position) {
		RaycastHit2D hit = Physics2D.Raycast (position, Vector2.zero);
		if (hit.collider != null) {
			return true;
		} else {
			return false;
		}
	}
	
}
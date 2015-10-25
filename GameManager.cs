using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {

	// objects referenced in script
	public GameObject ball;			// main player bouncing ball
	public GameObject explosion;	// particle system to instantiate on hits
	private CameraCrew director;	// crew instance for directing camera and lighting

	// gamewide control flow
	public static bool gameOver;

	// UI updates
	public Image screenFader;
	public Text centerText;


	// crew class for directing camera and lighting
	private class CameraCrew {
		public Camera mainCamera;
		public Light mainLight;
		public MusicManager mainMusic;

		// instance constructors
		public CameraCrew (Camera cam, Light light, MusicManager music) {
			mainCamera = cam;
			mainLight = light;
			mainMusic = music;
		}
		public CameraCrew () {
			mainCamera = Camera.main;
			mainLight = GameObject.Find ("Directional Light").GetComponent<Light> ();
			mainMusic = GameObject.Find ("Music Manager").GetComponent<MusicManager> ();
		}

		// smooth follow the x and y position of a target object, e.g. player
		public void TrackObject (GameObject obj) {
			mainCamera.transform.position = Vector3.Lerp (mainCamera.transform.position, new Vector3(obj.transform.position.x, obj.transform.position.y, mainCamera.transform.position.z), Time.deltaTime);
		}
	}

	// Use this for initialization
	void Start () {
		gameOver = false;
		director = new CameraCrew ();

		screenFader.CrossFadeAlpha (0f, 1f, false);
		centerText.CrossFadeAlpha (0f, 0f, true);
	}
	
	void Update () {
		//director.TrackObject (ball);

		// do general gameover stuff (also see OnPlayerDead)
		if (gameOver) {
			director.mainMusic.PitchDown();
			screenFader.CrossFadeAlpha (1f, 2f, false);
			centerText.text = "You Are Dead";
			centerText.CrossFadeAlpha (1f, 2f, false);
		}

	}

	// called when player dies
	public IEnumerator OnPlayerDead (GameObject killer, GameObject player) {
		// tell update to perform gameover actions over time
		gameOver = true;
		Instantiate (explosion, player.transform.position, Quaternion.identity);
		Destroy (player.gameObject);
		// wait and reset level (fade scene and music actions happen in update)
		yield return new WaitForSeconds (4f);
		Application.LoadLevel (Application.loadedLevel);
	}

}

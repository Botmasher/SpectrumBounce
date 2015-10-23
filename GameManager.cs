using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject ball;			// reference to bouncing ball
	private CameraCrew director;	// crew instance for directing camera and lighting


	// crew class for directing camera and lighting
	private class CameraCrew {
		public Camera mainCamera;
		public Light mainLight;

		// instance constructors
		public CameraCrew (Camera cam, Light light) {
			mainCamera = cam;
			mainLight = light;
		}
		public CameraCrew () {
			mainCamera = Camera.main;
			mainLight = GameObject.Find ("Directional Light").GetComponent<Light> ();
		}

		// smooth follow the x and y position of a target object, e.g. player
		public void TrackObject (GameObject obj) {
			mainCamera.transform.position = Vector3.Lerp (mainCamera.transform.position, new Vector3(obj.transform.position.x, obj.transform.position.y, mainCamera.transform.position.z), Time.deltaTime);
		}
	}

	// Use this for initialization
	void Start () {
		director = new CameraCrew ();
	}

	void Update () {
		//director.TrackObject (ball);
	}

	// called when player dies
	public static void OnPlayerDead (GameObject killer, GameObject player) {
		Debug.Log ("You died!");
		Destroy (killer.gameObject);
		Destroy (player.gameObject);
		Application.LoadLevel (Application.loadedLevel);
	}

}

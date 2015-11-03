using UnityEngine;
using System.Collections;

public class LightTestPlayer : MonoBehaviour {
	
	public GameObject laser;

	public static int score;
	public static bool gameOver;
	private float gameOverTimer;

	// brightness control
	private Light myLight;
	

	void Start () {
		// 
		myLight = GetComponent<Light>();

		// control flow for gameover state
		gameOver = false;
		gameOverTimer = 0f;

		// spawn enemy laser over time
		StartCoroutine (BeamLaser(false));
	}


	void Update () {
	
		if (LightTestMusicManager.songVolume > 0f && myLight.intensity < 3f) { 
			myLight.intensity += (Mathf.Clamp(LightTestMusicManager.songVolume,-0.5f,0.5f)) * Time.deltaTime;
		} else {
			myLight.intensity -= 0.5f * Time.deltaTime;
		}

		if (myLight.intensity >= 2.5f) {
			GetComponent<AudioSource>().volume = 1f;
			score ++;
		} else {
			GetComponent<AudioSource>().volume = 0f;
		}

		if (gameOver) {
			GameObject.FindGameObjectWithTag("Music").GetComponent<LightTestMusicManager>().PitchDown();
			gameOverTimer += Time.deltaTime;
			if (gameOverTimer >= 4f) {
				Application.LoadLevel (Application.loadedLevel);
			}
		}

	}


	/**
	 * 	Laser spawn, tease and shoot
	 */
	IEnumerator BeamLaser (bool fakeout) {
		// reference to instantiated laser
		GameObject thisBeam;
		bool spawningAbove = Random.Range(0f,1f) > 0.5f ? true : false;

		// random respawn wait timer
		yield return new WaitForSeconds (Random.Range (1f, 7f));

		// spawn the laser either a little above or a bit below the spectrum
		if (spawningAbove) {
			thisBeam = Instantiate (laser, new Vector3 (0f, Random.Range(0.7f, 1.6f), 0f), laser.transform.rotation) as GameObject;
		} else {
			thisBeam = Instantiate (laser, new Vector3 (0f, Random.Range(-3.6f, -2.6f), 0f), laser.transform.rotation) as GameObject;
		}

		// turn off collider so can't hit player during warning
		thisBeam.GetComponent<BoxCollider2D>().enabled = false;

		// flash laser off and on as a warning
		yield return new WaitForSeconds (0.04f);
		thisBeam.GetComponent<MeshRenderer>().enabled = false;
		thisBeam.GetComponent<AudioSource>().enabled = false;
		yield return new WaitForSeconds (0.03f);
		thisBeam.GetComponent<MeshRenderer>().enabled = true;
		thisBeam.GetComponent<AudioSource>().enabled = true;
		yield return new WaitForSeconds (0.03f);
		thisBeam.GetComponent<MeshRenderer>().enabled = false;
		thisBeam.GetComponent<AudioSource>().enabled = false;
		yield return new WaitForSeconds (0.03f);
		thisBeam.GetComponent<MeshRenderer>().enabled = true;
		thisBeam.GetComponent<AudioSource>().enabled = true;
		yield return new WaitForSeconds (0.03f);
		thisBeam.GetComponent<MeshRenderer>().enabled = false;
		thisBeam.GetComponent<AudioSource>().enabled = false;

		// wait before activating dangerous beam
		yield return new WaitForSeconds (Random.Range (1f, 2f));

		if (!fakeout) {
			thisBeam.GetComponent<MeshRenderer>().enabled = true;
			thisBeam.GetComponent<BoxCollider2D>().enabled = true;
			thisBeam.GetComponent<AudioSource>().enabled = true;
		}

		yield return new WaitForSeconds (Random.Range(0.8f,3f) );
		Destroy (thisBeam.gameObject);

		fakeout = Random.Range(0,5) > 4 ? true : false;

		StartCoroutine (BeamLaser(fakeout));

		yield return null;
	}

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BumperBoard : MonoBehaviour {

	// vars to reference bumper object and store currently instantiated instances
	public Transform bumper;
	private List<Transform> bumpers = new List<Transform>();
	public float numberOfBumpers;
	private Vector2 bumperPosition;

	// control flow
	private bool spawnBumper;
	private float spawnFrequency=7f;
	private float spawnCountup=0f;


	void Update () {

		// set spawn state when it's time
		if (spawnCountup >= spawnFrequency && !spawnBumper) {
			spawnBumper = true;
		// spawn bumper and reset counter
		} else if (spawnBumper) {
			spawnBumper = false;
			spawnCountup = 0f;
			StartCoroutine(SpawnBumper());
		} else {
			// count up to bumper spawn time
			spawnCountup += Time.deltaTime;
		}
	}


	/**
	 * 	Spawn sequence for bumpers
	 */
	IEnumerator SpawnBumper () {

		// delete current bumpers and dump out the list of them
		for (int i=0; i < bumpers.Count; i++) {
			if (bumpers[i] != null) {
				Destroy (bumpers[i].gameObject);
			}
		}
		bumpers.Clear();

		yield return new WaitForSeconds (0.2f);

		// spawn the expected number of bumpers
		for (int i=0; i < numberOfBumpers; i++) {
			// choose new x-y coordinates on the screen above the spectrumline
			bumperPosition = new Vector2 ( Random.Range (-13f,13f), Random.Range (0f,6.5f) );

			// choose new positions if randomly chosen spawnpoint is taken
			do {
				bumperPosition = new Vector2 ( Random.Range (-13f,13f), Random.Range (0f,6.5f) );
			} while (GameManager.ThisSpotHasObjects(bumperPosition));

			// spawn and add them to list
			bumpers.Add (Instantiate (bumper, bumperPosition, Quaternion.identity) as Transform);
		}

	}

}

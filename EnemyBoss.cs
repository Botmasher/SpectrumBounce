using UnityEngine;
using System.Collections;

public class EnemyBoss : MonoBehaviour {

	// randomly count up to blink anim for looks
	private float blinkTimer;
	private float blinkCountup=0f;

	// boss sfx
	public AudioClip entranceSound;
	public AudioClip attackSound;

	// control boss movement states
	private bool isEntering;
	private bool isSlamming;
	private bool isMoving;
	private float targetX;

	
	void Start () {
		blinkTimer = Random.Range (1f, 5f);
		StartCoroutine(MoveToCenter());

		GetComponent<AudioSource>().clip = entranceSound;
		GetComponent<AudioSource>().Play();
	}
	

	void Update () {

		// blink if it's time to blink
		blinkCountup += Time.deltaTime;
		if (blinkCountup >= blinkTimer) {
			StartCoroutine (Blink());
			// reset blink counter
			blinkTimer = Random.Range (1f, 5f);
			blinkCountup = 0f;
		}

		// do entering the stage stuff
		if (isEntering) {
			transform.position = Vector2.Lerp (transform.position, new Vector2 (0f, transform.position.y), Time.deltaTime);
		// do slamming stuff
		} else if (isSlamming) {
			StartCoroutine (Slam ());
		} else if (isMoving) {
			// go back up
			if (transform.position.y < 4f || transform.position.y > 5f) {
				transform.position = Vector2.Lerp (transform.position, new Vector2(transform.position.x, 4.1f), 1.5f*Time.deltaTime);
			// go left or right
			} else if (Mathf.Abs(transform.position.x) < Mathf.Abs(targetX)-0.1f) {
				transform.position = Vector2.Lerp (transform.position, new Vector2(targetX, transform.position.y), Time.deltaTime);
			// choose a new x direction to go towards
			} else {
				targetX = Random.Range(-10f,10f);
			}
		}

	}


	/**
	 * 	Trigger the blink animation for this sprite
	 */
	IEnumerator Blink () {
		GetComponent<Animator>().SetTrigger("isBlinking");
		yield return new WaitForEndOfFrame ();
	}


	/**
	 * 	Toggle the update to enter the stage like a boss. Slowly. And from the right.
	 */
	IEnumerator MoveToCenter () {
		isEntering = true;

		// turn off collider and move to center
		GetComponent<BoxCollider2D>().enabled = false;
		yield return new WaitForSeconds (3f);
		GetComponent<BoxCollider2D>().enabled = true;

		// set initial slam and movement variables
		isSlamming = true;		// set to slam so that random slam loop starts
		isEntering = false;		// turn off entering update movements
		yield return null;
	}


	/**
	 * 	Fall into the ground, wait, then toggle back to regular movement, then wait, then toggle availability for next slam
	 */
	IEnumerator Slam () {
		// avoid multiple calls for a single slam
		isSlamming = false;

		// turn behavior to slamming
		GetComponent<Rigidbody2D>().isKinematic=false;

		// play attack audio
		GetComponent<AudioSource>().clip = attackSound;
		GetComponent<AudioSource>().Play ();

		// wait then reset behavior to moving
		yield return new WaitForSeconds (1.3f);
		GetComponent<Rigidbody2D>().isKinematic=true;
		isMoving = true;

		// give time for movement to end, then reset slam action
		yield return new WaitForSeconds (Random.Range (4f, 7f));
		isSlamming = true;
		yield return null;
	}

}

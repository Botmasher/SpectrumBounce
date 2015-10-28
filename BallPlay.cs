﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BallPlay : MonoBehaviour {

	public string spectrumBlockTag;		// tag for audio spectrum blocks checked on collision

	// sfx to play
	public AudioClip ballHit;

	// actions or items on input
	public int numberOfBumpers;			// total bumpers the player has
	public static int bumpersDeployed;	// count of bumpers in the world
	public GameObject bumper;			// ball bumper that stays in world for limited time

	public float timeBetweenNudges;		// how often player can apply a nudge force
	public float nudgeForce;			// how much the nudge nudges
	private float nudgeCountup;			// counter for counting up to next nudge

	private Vector2 axes;				// for storing horizontal and vertical input axes

	// UI elements
	public Text itemsText;

	void Start () {
		// initialize 
		axes = Vector2.zero;
	}

	void Update () {
		// update UI text
		itemsText.text = ("Bumpers Left: "+(numberOfBumpers-bumpersDeployed));

		// ensure ball always stays along 2D z collision plane
		if (Mathf.Abs(this.transform.position.z) > 1f) {
			transform.Translate (new Vector3 (this.transform.position.x, this.transform.position.y, 0f));
		}

		// add a bumper to world if clicked on free space
		// check for player, enemies, and other barriers to placement
		if (Input.GetButtonDown ("Fire1") && bumpersDeployed < numberOfBumpers) {
			if (!GameManager.ThisSpotHasObjects(Camera.main.ScreenToWorldPoint(Input.mousePosition))) {
				Instantiate (bumper, Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z)), Quaternion.identity);
			}
		}

		nudgeCountup += Time.deltaTime;

		if (nudgeCountup >= timeBetweenNudges) {
			axes = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			if (axes != Vector2.zero) {
				StartCoroutine (NudgePlayer());
			}
		}
	
	}


	void OnCollisionEnter2D (Collision2D collision) {
		// play a hit sound
		GetComponent<AudioSource>().clip = ballHit;
		GetComponent<AudioSource>().Play ();

		// add sidespin if collide with the spectrum
		//if (collision.collider.gameObject.tag == spectrumBlockTag) {
		//	AddRandomSideSpin ();
		//}
	}


	// nudge ball right or left on impact
	void AddRandomSideSpin () {
		this.GetComponent<Rigidbody2D>().AddForce (new Vector2 (Random.Range (-4f, 4f), 0f));
	}

	IEnumerator NudgePlayer () {
		// reset cooldown counter so player doesn't access again
		nudgeCountup = 0f;

		//GetComponent<Rigidbody2D> ().AddForce (axes * nudgeForce);

		/*while (axes != Vector2.zero) {
			GetComponent<Rigidbody2D> ().AddForce (axes * nudgeForce);
			axes = Vector2.Lerp (axes, Vector2.zero, Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}*/

		while () {
			transform.Translate (Vector2.Lerp (, new Vector2(transform.position.x + axes.x, transform.position.y + axes.y), Time.deltaTime);
			yield return new WaitForEndOfFrame ();
		}
		// reinforce the reset
		nudgeCountup=0f;

		yield return null;
	}

}

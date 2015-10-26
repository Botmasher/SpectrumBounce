using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BallPlay : MonoBehaviour {

	public string spectrumBlockTag;		// tag for audio spectrum blocks checked on collision

	// sfx to play
	public AudioClip ballHit;

	// item placement on input
	public int numberOfBumpers;			// total bumpers the player has
	public static int bumpersDeployed;	// count of bumpers in the world
	public GameObject bumper;			// ball bumper that stays in world for limited time

	// UI elements
	public Text itemsText;
	

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
			if (Camera.main.GetComponent<GameManager>().ClickRaycast() == null || Camera.main.GetComponent<GameManager>().ClickRaycast().collider.tag != this.gameObject.tag) {
				Instantiate (bumper, Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z)), Quaternion.identity);
			}
		}
	
	}


	void OnCollisionEnter2D (Collision2D collision) {
		// play a hit sound
		GetComponent<AudioSource>().clip = ballHit;
		GetComponent<AudioSource>().Play ();
		// possibly add sidespin if collide with the spectrum
		//if (collision.collider.gameObject.tag == spectrumBlockTag) {
		//	AddRandomSideSpin ();
		//}
	}


	// nudge ball right or left on impact
	void AddRandomSideSpin () {
		this.GetComponent<Rigidbody2D>().AddForce (new Vector2 (Random.Range (-4f, 4f), 0f));
	}


}

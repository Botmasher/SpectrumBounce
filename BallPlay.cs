using UnityEngine;
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
	
	public float nudgeForce;			// how much the nudge nudges
	public float nudgeFrequency;		// how often player can apply a nudge force
	private float nudgeCountup;			// counter for counting up to next nudge

	// input axes
	private float nudgeDir;				// input axes for nudge direction
	private float horiz;
	private float vert;

	// UI elements
	public Text itemsText;

	void Start () {
		bumpersDeployed=0;
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


		// ability to periodically nudge ball along x axis
		nudgeCountup += Time.deltaTime;

		if (nudgeCountup >= nudgeFrequency) {
			nudgeDir = Input.GetAxisRaw("Horizontal");
			if (nudgeDir != 0f) {
				StartCoroutine (NudgePlayer (nudgeDir, nudgeForce));
			}
		}

//		// input to nudge ball sideways
//		horiz = Input.GetAxis ("Horizontal");
//		if (Mathf.Abs(horiz) > 0f) {
//			GetComponent<Rigidbody2D>().AddForce(Vector2.right * horiz * nudgeForce * 2f*Time.deltaTime);
//		}

//		// input to send ball flying upwards or not move much at all
//		if (Input.GetButtonDown ("Jump")){
//			GetComponent<Rigidbody2D>().AddForce(Vector2.up*50f);
//		}

	}
	
	
	void OnCollisionEnter2D (Collision2D collision) {
		// play a hit sound
		GetComponent<AudioSource>().clip = ballHit;
		GetComponent<AudioSource>().Play ();

		// if it's an enemy, take the hit
		if (collision.collider.gameObject.tag == "Enemy") {
			StartCoroutine (Camera.main.GetComponent<GameManager> ().OnPlayerDead (collision.collider.gameObject, this.gameObject));
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		// if hit an enemy, take the hit
		if (other.gameObject.tag == "Enemy") {
			StartCoroutine (Camera.main.GetComponent<GameManager> ().OnPlayerDead (other.gameObject, this.gameObject));
		}
	}



	IEnumerator NudgePlayer (float Xdir, float force) {

		GetComponent<Rigidbody2D> ().AddForce (Vector2.right * Xdir * force);

		yield return new WaitForSeconds (0.1f);

		nudgeCountup = 0f;

		yield return null;
	}

}

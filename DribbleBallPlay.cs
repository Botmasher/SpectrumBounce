using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DribbleBallPlay : MonoBehaviour {
	
	// sfx to play
	public AudioClip ballHit;
	
	// actions or items on input
	public int numberOfBumpers;			// total bumpers the player has
	public static int bumpersDeployed;	// count of bumpers in the world
	public GameObject bumper;			// ball bumper that stays in world for limited time
	
	public float nudgeForce;			// how much the nudge nudges
	public float nudgeCooldown;			// how often player can apply a nudge force
	private bool canNudge;				// control flow for entering nudge state
	
	// movement input
	public float speed;
	private float horiz;
	private float vert;
	
	// UI elements
	public Text itemsText;
	
	void Start () {
		bumpersDeployed = 0;
		canNudge = true;
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
		
		if (canNudge) {
			vert = Input.GetAxisRaw ("Vertical");
			if (vert != 0f) {
				StartCoroutine (Nudge (vert, nudgeForce));
			}
		}

		horiz = Input.GetAxisRaw("Horizontal");
		if (horiz!=0f) {
			GetComponent<Rigidbody2D>().AddForce(Vector2.right*horiz*speed*Time.deltaTime);
		}
		
	}
	
	
	void OnCollisionEnter2D (Collision2D collision) {
		// play a hit sound
		GetComponent<AudioSource>().clip = ballHit;
		GetComponent<AudioSource>().Play ();
		
		// if it's an enemy, take the hit
		if (collision.collider.gameObject.tag == "Enemy") {
			StartCoroutine (Camera.main.GetComponent<DribbleTestGameManager> ().OnPlayerDead (collision.collider.gameObject, this.gameObject));
		}
	}
	
	void OnTriggerEnter2D (Collider2D other) {
		// if hit an enemy, take the hit
		if (other.gameObject.tag == "Enemy") {
			StartCoroutine (Camera.main.GetComponent<DribbleTestGameManager> ().OnPlayerDead (other.gameObject, this.gameObject));
		}
	}
	
	
	IEnumerator Nudge (float Ydir, float force){
		canNudge = false;
		GetComponent<Rigidbody2D>().AddForce (new Vector2(0f,Ydir) * 8f * force);
		yield return new WaitForSeconds (nudgeCooldown);
		canNudge = true;
	}
	
}
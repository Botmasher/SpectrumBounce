using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public float lifetime;
	public float speed;
	private bool movingRight;

	public AudioClip birdScreech;

	void Start () {
		// flip coin for facing left right
		movingRight = Mathf.Round (Random.Range (0f,1f)) == 1 ? true : false;

		// play bird sound
		GetComponent<AudioSource>().clip = birdScreech;
		GetComponent<AudioSource>().Play();

		// face the correct way and start off opposite side of screen
		if (movingRight) {
			this.transform.position = new Vector3 (-16f, this.transform.position.y, this.transform.position.z);
			// flip along y axis
			this.transform.rotation = Quaternion.AngleAxis(180f, Vector3.up);
		} else {
			this.transform.position = new Vector3 (16f, this.transform.position.y, this.transform.position.z);
		}
	}

	void Update() {
		// move linearly over time
		this.transform.Translate (Vector3.left * speed * Time.deltaTime);

		// self destruct if exits screen
		if (movingRight && this.transform.position.x > Camera.main.ViewportToWorldPoint(Vector3.one).x) {
			Destroy (this.gameObject);
		} else if (!movingRight && this.transform.position.x < Camera.main.ViewportToWorldPoint (Vector3.zero).x) {
			Destroy (this.gameObject);
		}

	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Player") {
			StartCoroutine (Camera.main.GetComponent<GameManager>().OnPlayerDead (this.gameObject, other.gameObject));
			Destroy (this.gameObject);
		}
	}
	
}

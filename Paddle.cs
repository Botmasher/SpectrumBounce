using UnityEngine;
using System.Collections;

public class Paddle : MonoBehaviour {
	
	public float moveSpeed;
	private float horiz;

	void Update () {
		horiz = Input.GetAxis ("Horizontal");
		transform.Translate (Vector2.right * horiz * moveSpeed * Time.deltaTime);
	}

	void OnCollisionEnter2D (Collision2D other) {
		if (other.collider.gameObject.tag == "Player") {
			other.collider.GetComponent<Rigidbody2D>().AddForce(new Vector2((transform.position.x-other.contacts[0].point.x)*moveSpeed, -50f));
		}
	}
}
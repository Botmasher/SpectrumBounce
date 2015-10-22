using UnityEngine;
using System.Collections;

public class BallPlay : MonoBehaviour {

	public string spectrumBlockTag;		// tag for audio spectrum blocks checked on collision

	// for adding random spin on impact
	public float randomSideForce;

	void AddRandomSideSpin () {
		this.GetComponent<Rigidbody2D>().AddForce (new Vector2 (Random.Range (-5f,5f), 0f));
	}

	void Update () {
		// ensure ball always stays along 2D z collision plane
		if (Mathf.Abs(this.transform.position.z) > 1f) {
			transform.Translate (new Vector3 (this.transform.position.x, this.transform.position.y, 0f));
		}
	}

	void OnCollisionEnter2D (Collision2D collision) {
		// possibly add sidespin if collide with the spectrum
		if (collision.collider.gameObject.tag == spectrumBlockTag) {
			AddRandomSideSpin ();

		}
	}
}

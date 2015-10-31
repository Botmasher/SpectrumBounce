using UnityEngine;
using System.Collections;

public class DribbleDanceCube : MonoBehaviour {

	// take an average of a sample of the spectrum for bouncing along to music
	public int sliceSpectrumMin;
	public int sliceSpectrumMax;

	public float bounceMultiplier;		// how much to bounce to the music
	public bool isSpecial;

	void Start () {
		isSpecial = false;
	}

	void Update () {
	
		// bounce with the music
		transform.localScale = Vector3.Lerp( transform.localScale, new Vector3( transform.localScale.x, 1f + (bounceMultiplier*DribbleTestMusicManager.SpectrumAverage(0,64)), 1f), 4f * Time.deltaTime );
	
		if (isSpecial) {
			GetComponent<MeshRenderer>().material.color = Color.red;
		} else {
			GetComponent<MeshRenderer>().material.color = Color.black;
		}

	}


	void OnCollisionEnter2D (Collision2D collision) {
		if (isSpecial && collision.collider.gameObject.tag == "Player") {
			Debug.Log ("These things are trueeee!");
			StartCoroutine(gameObject.GetComponentInParent<DribbleDanceCubeMaster>().ChooseNewSpecialChild());
		}
	}

}

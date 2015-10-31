using UnityEngine;
using System.Collections;

public class DribbleDanceCubeMaster : MonoBehaviour {

	private bool firstTimeAround;


	void Start () {
		firstTimeAround = true;
	}


	void Update () {

		if (firstTimeAround) {
			StartCoroutine(ChooseNewSpecialChild());
			firstTimeAround = false;
		}

	}
	

	public IEnumerator ChooseNewSpecialChild () {
		Debug.Log ("choosing");
		// tell all of the dancing cubes they are not special
		for (int i=0; i < GameObject.FindGameObjectsWithTag("Spectrum").Length; i++) {
			GameObject.FindGameObjectsWithTag("Spectrum")[i].GetComponent<DribbleDanceCube>().isSpecial = false;
		}

		yield return new WaitForSeconds (1f);
		// but then tell one of the dancing cubes it is special
		GetComponent<AudioSource>().Play();
		GameObject.FindGameObjectsWithTag("Spectrum")[Mathf.RoundToInt(Random.Range (0f,2f))].GetComponent<DribbleDanceCube>().isSpecial = true;
	}

}

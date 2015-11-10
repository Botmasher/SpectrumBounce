using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestColorPaint : MonoBehaviour {

	// brush variables
	public static Color brushColor;
	public int size;
	private int brushSize;

	// for changing specific clicked pixels
	Ray ray;
	RaycastHit hit;
	Texture2D tex;

	// display text to user
	public UnityEngine.UI.Text screenText;
	
	// circles for AI to paint
	public List<GameObject> paintTargets = new List<GameObject>();


	void Start () {
		// set up AI color picking
		StartCoroutine ( PaintRandomValue (0) );
		screenText.text = "";

		// initial brush state
		brushColor = Color.black;
		brushSize = (int)(size*0.5f);

		// generate texture for this object
		tex = new Texture2D(600, 800);
		GetComponent<Renderer>().material.mainTexture = tex;
	}


	void Update () {
	
		// raycast check that this gameobject was hit
		if (Input.GetMouseButton(0)) {
			ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			Physics.Raycast(ray, out hit);

			if (hit.collider != null && hit.collider.gameObject == this.gameObject) {

				/**
				 * iterate over a box of rows and columns, coloring each pixel in that square
				 * 	- iterator starts neg and ends at half of public size var to keep brush edges equidistant from cursor
				 *  - texture.Apply() stores the changes, but is expensive
				 */
				for (int i=(int)(-brushSize); i<=brushSize; i++) {
					for (int j=(int)(-brushSize); j<=brushSize; j++) {
						// use the x/y of the exact raycast pixel hit as reference; set it and surrounding pixels to color
						tex.SetPixel ((int)(tex.width*hit.textureCoord.x+i), (int)(tex.height*hit.textureCoord.y+j), brushColor);
					}
				}
				tex.Apply();
			}

		}

	}


	IEnumerator PaintRandomValue (int index) {
		yield return new WaitForSeconds (1f);
		screenText.text = "Choosing a color";
		yield return new WaitForSeconds (3f);
		screenText.text = "";
		
		float thisValue = Random.Range (0f, 1f);
		paintTargets[index].GetComponent<MeshRenderer>().material.color = new Color (thisValue, thisValue, thisValue);

		index ++;
		if (index < paintTargets.Count) {
			StartCoroutine (PaintRandomValue (index));
		}
		yield return null;
	}

}
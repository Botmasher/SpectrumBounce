﻿using UnityEngine;
using System.Collections;

public class TestColorPicker : MonoBehaviour {

	// for pixel color data from hit object
	RaycastHit hit;
	Ray ray;
	Texture2D tex;
	Color pixelColor;

	// gameobjects to change
	public GameObject target;
	public UnityEngine.UI.InputField textBox;
	private Color inputColor;


	void Update () {

		if (Input.GetMouseButton(0)) {
			// cast ray looking for collider with texture
			ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			Physics.Raycast(ray, out hit);
			// /!\ make sure collider is mesh collider otherwise hit.textureCoord returns Vector2.zero
			if (hit.collider.name == this.name) {
				//Debug.Log (hit.collider.name);
				tex = this.GetComponent<MeshRenderer> ().material.mainTexture as Texture2D;
				//Debug.Log (hit.textureCoord);

				pixelColor = tex.GetPixel((int)(hit.textureCoord.x*tex.width), (int)(hit.textureCoord.y*tex.height));

				// change color of pixel painter in color paint script
				TestColorPaint.brushColor = pixelColor;

				// change color of target object using tex data
				target.GetComponent<MeshRenderer> ().material.color = pixelColor;

				// update color code in input field
				textBox.text = Mathf.Round(pixelColor[1]*255).ToString();
			}
				
		}

		// update value to text input if user manually inputs a value integer
		int colorValue;
		// check that input text is a valid integer
		if (int.TryParse(textBox.text, out colorValue)) {
			// convert color to this grayscale value
			inputColor = new Color (colorValue, colorValue, colorValue);
			// change the brush color and target material color
			if (inputColor != TestColorPaint.brushColor) {
				TestColorPaint.brushColor = inputColor;
				target.GetComponent<MeshRenderer> ().material.color = inputColor;
			}
		}

	}

}

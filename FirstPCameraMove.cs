using UnityEngine;
using System.Collections;

public class FirstPCameraMove : MonoBehaviour {

	// turning speed factor used in look rotation calculations
	public float sensitivity;
	

	// Update is called once per frame
	void Update () {

		// WASD or arrow key strafe movement
		if (Input.GetAxis("Horizontal") != 0f) {
			transform.Translate(Vector3.right*10f*Input.GetAxis("Horizontal")*Time.deltaTime);
		}
		if (Input.GetAxis("Vertical") != 0f) {
			transform.Translate(Vector3.forward*10f*Input.GetAxis("Vertical")*Time.deltaTime);
		}


		// rotate when moving mouse to look left or right
		if (Input.mousePosition.x >= Camera.main.pixelRect.width*0.6f) {
			transform.Rotate (Vector3.up * (Input.mousePosition.x - Camera.main.pixelRect.width*0.6f) * sensitivity * Time.deltaTime);
		} else if (Input.mousePosition.x <= Camera.main.pixelRect.width*0.4f) {
			transform.Rotate (Vector3.up * -(Camera.main.pixelRect.width*0.4f - Input.mousePosition.x) * sensitivity * Time.deltaTime);
		}

		// rotate when moving mouse to look up or down
		//  /!\ currently using this with above block causes side lean
//		if (Input.mousePosition.y >= Camera.main.pixelRect.height*0.6f && transform.rotation.x <= 50f) {
//			transform.Rotate (Vector3.right * (Camera.main.pixelRect.height*0.6f - Input.mousePosition.y) * sensitivity * Time.deltaTime);
//		} else if (Input.mousePosition.y <= Camera.main.pixelRect.width*0.4f && transform.rotation.x >= -24f) {
//			transform.Rotate (Vector3.right * (Camera.main.pixelRect.height*0.4f - Input.mousePosition.y) * sensitivity * Time.deltaTime);
//		}
	
	}


}
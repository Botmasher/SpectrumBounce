using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour {

	// for lining up objects and moving them with spectrum while playing song
	public GameObject spectrumBlock;
	private List<GameObject> spectrumBlocks = new List<GameObject>();
	public float spectrumXOrigin;					// where to start laying blocks, from left to right along world x-axis

	// for listening to, behaving with and FXing music
	public AudioMixer mixer;						// main mixer in scene
	public AudioMixerGroup master;					// all channels
	public AudioMixerGroup music;					// music channels
	[Range (64, 8192)] public int sampleSlices;		// break spectrum data into n samples (spectrumdata allows min=64,max=8192)
	private float[] currentSpectrumData;			// preallocated array to hold spectrum data for current frame

	// music to play through mixer
	public List<AudioClip> playlist = new List<AudioClip>();
	private AudioClip song;							// current music playing


	void Start () {
		// ensure samples is an even integer between 64 and 8192 as expected by spectrumdata
		sampleSlices = Mathf.Clamp (sampleSlices, 64, 8192);
		if (sampleSlices%2 == 1) {
			sampleSlices += 1;
		}

		// initialize spectrumdata array to how many samples we'll take each frame
		currentSpectrumData = new float[sampleSlices];

		// lineup blocks in spectrum from origin until placing total number of sample slices
		for (int i = 0; i < sampleSlices; i++) {
			spectrumBlocks.Add (Instantiate (spectrumBlock, new Vector3 (spectrumXOrigin+i*0.2f, -3f, 0f), Quaternion.identity) as GameObject);
		}

		// load and play first song
		song = playlist[0];
		GetComponent<AudioSource> ().clip = song;
		GetComponent<AudioSource> ().Play ();

		// disable last block, since it currently gets only value 0 from spectrumData
		spectrumBlocks[spectrumBlocks.Count-1].SetActive (false);
		spectrumBlocks[0].SetActive (false);
	}
	

	void Update () {
		// call function to adjust spectrum this frame
		MoveSpectrumWithMusic ();

		// switch up songs
		if (Input.GetButtonDown ("Jump")) {
			GetComponent<AudioSource> ().clip = playlist[Random.Range(0, playlist.Count)];
			GetComponent<AudioSource> ().Play ();
		}
	}

	void MoveSpectrumWithMusic () {
		currentSpectrumData = GetComponent<AudioSource> ().GetSpectrumData (sampleSlices, 0, FFTWindow.Blackman);
		for (int i = 1; i < currentSpectrumData.Length-1; i++) {
			// change position according to spectrum data
			//spectrumBlocks[i].transform.position = Vector3.Lerp (spectrumBlocks[i].transform.position, new Vector3 (spectrumBlocks[i].transform.position.x, 1f + 0.04f * i + Mathf.Log (currentSpectrumData[i]*i), spectrumBlocks[i].transform.position.z), 8f * Time.deltaTime);
			// change y-scale according to spectrum data
			spectrumBlocks[i].transform.localScale = Vector3.Lerp (spectrumBlocks[i].transform.localScale, new Vector3 (spectrumBlocks[i].transform.localScale.x, 0.2f + 5f*currentSpectrumData[i]*(i+i)*Mathf.Log(currentSpectrumData[i]*currentSpectrumData[i]*i*i),spectrumBlocks[i].transform.localScale.z), 8f * Time.deltaTime);
		}
	}

	// music slowly goes to pitch 0 - the sound of failure
	public void PitchDown () {
		float thisPitch;
		mixer.GetFloat ("MusicPitch", out thisPitch);
		mixer.SetFloat ("MusicPitch", thisPitch - (0.5f*Time.deltaTime));
	}

}

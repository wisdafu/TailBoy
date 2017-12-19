using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour {

	public AudioClip pop1, pop2, pop3;

	AudioSource[] sourceArray;

	// Use this for initialization
	void Start () {

		sourceArray = new AudioSource[3];

		sourceArray [0] = AddAudio (pop1, 1);
		sourceArray [1] = AddAudio (pop2, 1);
		sourceArray [2] = AddAudio (pop3, 1);

		print (sourceArray[0]);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

  void OnTriggerEnter(Collider other)
  {
    if(other.tag == "Player")
    {
      other.gameObject.GetComponent<PlayerMovement>().ScoreAdd();
			int selection = Random.Range (0, 3);
			sourceArray [selection].Play ();
      Destroy(this.gameObject, 0.1f);
    }
  }

	AudioSource AddAudio(AudioClip clip, float vol){
		//see https://answers.unity.com/questions/240468/how-to-play-multiple-audioclips-from-the-same-obje.html

		AudioSource newAudio = gameObject.AddComponent<AudioSource>() as AudioSource;

		newAudio.clip = clip;
		newAudio.volume = vol;
		newAudio.playOnAwake = false;

		return newAudio;
	}
}

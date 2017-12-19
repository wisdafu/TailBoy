using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUI : MonoBehaviour {

    public UnityEngine.UI.Text scoreText;

	// Use this for initialization
	void Start () {
    scoreText.text = "Score: ";
	}
	
	// Update is called once per frame
	void Update () {
      
	}

    public void updateText(int score)
  {
    scoreText = this.GetComponent<UnityEngine.UI.Text>();
    scoreText.text = "Score: " + score;
  }
}

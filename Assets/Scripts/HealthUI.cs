using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour {

  public UnityEngine.UI.Text healthText;

  // Use this for initialization
  void Start () {
    healthText.text = "Tail Boy is good to go!";
  }
	
	// Update is called once per frame
	void Update () {

  }

  public void invulnText()
  {
    healthText = this.GetComponent<UnityEngine.UI.Text>();
    healthText.text = "Ouch!! Hurry get back!!";
  }

  public void regenText()
  {
    healthText = this.GetComponent<UnityEngine.UI.Text>();
    healthText.text = "Hold off until Tail Boy can regenerate.";
  }

  public void healthyText()
  {
    healthText = this.GetComponent<UnityEngine.UI.Text>();
    healthText.text = "Tail Boy is good to go!";
  }
}

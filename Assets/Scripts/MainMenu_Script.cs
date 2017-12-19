using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu_Script : MonoBehaviour
{
    public Canvas uiCanvas;
    public Button yourButton;
    public TextMesh story_text;
    public float textLength;

    private bool text_moving = false;

    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void Update()
    {
      if (text_moving)
        {
            if(Input.GetButton("Fire1")) Application.LoadLevel("IntroScene");

            story_text.transform.Translate(0, .1f, 0);
        }

    if (story_text.transform.position.y > textLength)
        {
            Application.LoadLevel("IntroScene");
        }
    }

    void TaskOnClick()
    {
        start_story_animation();
    }

    void start_story_animation()
    {
        uiCanvas.gameObject.SetActive(false);
        text_moving = true;
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonRowController : MonoBehaviour {

    public Button[] buttons; 

	// Use this for initialization
	void Start () {
        /*
        int size = buttons.Length;
        float halfButtonWidth = Screen.width / (size + 0.0f) / 2;
        float buttonHeight = 30f;
        for(int i = 0; i < size - 1; i++)
        {
            buttons[i].GetComponent<RectTransform>().localPosition = new Vector3(halfButtonWidth + halfButtonWidth * 2 * i - Screen.width / 2, 0, 0);
            buttons[i].GetComponent<RectTransform>().sizeDelta = new Vector2(halfButtonWidth * 2, buttonHeight);
        }
        buttons[size - 1].GetComponent<RectTransform>().localPosition = new Vector3(Screen.width / 2 - halfButtonWidth, 0, 0);
        buttons[size - 1].GetComponent<RectTransform>().sizeDelta = new Vector2(halfButtonWidth * 2, buttonHeight);*/
    }

    // Update is called once per frame
    void Update () {
	
	}
}

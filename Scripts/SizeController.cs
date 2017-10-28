using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SizeController : MonoBehaviour {

    public bool AutoFontSize;
    public bool AutoPosition;
    public bool AutoSize;

    private float defaultWidth = 250;
    private float defaultHeight = 445;

    // Use this for initialization
    void Start ()
    {
        float width = (float)Screen.width;
        float height = (float)Screen.height;

        float multiplier = Mathf.Sqrt((width / defaultWidth) * (height / defaultHeight));
        float multiplierX = width / defaultWidth;
        float multiplierY = height / defaultHeight;
        if (AutoFontSize)
        {
            GetComponentInChildren<Text>().fontSize = (int)(multiplier * GetComponentInChildren<Text>().fontSize + 0.5);
        }
        if (AutoPosition)
        {
            float newWidth = GetComponent<RectTransform>().localPosition.x * multiplierX;
            float newHeight = GetComponent<RectTransform>().localPosition.y * multiplierY;
            GetComponent<RectTransform>().localPosition = new Vector3(newWidth, newHeight, 0);
        }
        if (AutoSize)
        {
            float newWidth = GetComponent<RectTransform>().sizeDelta.x * multiplierX;
            float newHeight = GetComponent<RectTransform>().sizeDelta.y * multiplierY;
            if (this.gameObject.GetComponent<InputField>())
            {
                //RectTransform textChildRect = this.gameObject.transform.Find("Text").GetComponent<RectTransform>();
                //float dHeight = Mathf.Abs(textChildRect.offsetMax.y) + Mathf.Abs(textChildRect.offsetMin.y);
                newHeight = 16 * multiplier + GetComponent<RectTransform>().sizeDelta.y - 16;
            }
            GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, newHeight);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RankLineManager : MonoBehaviour {
    public GameObject GUIScore;
    public GameObject GUILabel;
    public GameObject GUIDate;
    public GameObject GUILabelInput;

    private string textRank = "0";
    private bool editing = false;

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (editing)
        {
            GUILabelInput.SetActive(true);
            GUILabel.SetActive(false);
        }
        else
        {
            GUILabelInput.SetActive(false);
            GUILabel.SetActive(true);
        }
    }

    public void InitRankData(int rank)
    {
        textRank = rank.ToString();
        Score = PlayerPrefs.GetString("Score_" + textRank, "");
        Label = PlayerPrefs.GetString("Label_" + textRank, "");
        Date = PlayerPrefs.GetString("Date_" + textRank, "");
    }

    public void InitPosition(int rank)
    {
        int lineWidth = (int)(Screen.width * 0.8 / 6);
        int linePositionHeight = (int)(Screen.height * 0.7 / 2);
        int lineHeight = (int)(Screen.height * 1.0 / 445f * 16.0);

        //Width
        GetComponent<RectTransform>().sizeDelta = new Vector2(lineWidth * 6, lineHeight);

        GUIScore.GetComponent<RectTransform>().localPosition = new Vector3(-lineWidth * 2, 0, 0);
        GUIScore.GetComponent<RectTransform>().sizeDelta = new Vector2(lineWidth * 2, lineHeight);

        GUILabel.GetComponent<RectTransform>().sizeDelta = new Vector2(lineWidth * 2, lineHeight);
        GUILabelInput.GetComponent<RectTransform>().sizeDelta = new Vector2(lineWidth * 2, lineHeight + 10);

        GUIDate.GetComponent<RectTransform>().localPosition = new Vector3(lineWidth * 2, 0, 0);
        GUIDate.GetComponent<RectTransform>().sizeDelta = new Vector2(lineWidth * 2, lineHeight);

        //Height
        GetComponent<RectTransform>().localPosition = new Vector3(0, linePositionHeight - rank * lineHeight * 1.8f, 0);
    }

    public void SetEdit(string score)
    {
        Score = score;
        Date = System.DateTime.Now.ToString("yyyy-MM-dd");
        if (PlayerPrefs.HasKey("DefaultName"))
        {
            GUILabelInput.GetComponent<InputField>().text = PlayerPrefs.GetString("DefaultName");
        }
        editing = true;
    }

    public void EndEdit()
    {
        Label = GUILabelInput.GetComponentInChildren<InputField>().textComponent.text;
        editing = false;
    }

    public string Score
    {
        get { return GUIScore.GetComponent<Text>().text; }
        set
        {
            GUIScore.GetComponent<Text>().text = value;
            PlayerPrefs.SetString("Score_" + textRank, value);
        }
    }

    public string Label
    {
        get { return GUILabel.GetComponent<Text>().text; }
        set
        {
            GUILabel.GetComponent<Text>().text = value;
            PlayerPrefs.SetString("Label_" + textRank, value);
        }
    }

    public string Date
    {
        get { return GUIDate.GetComponent<Text>().text; }
        set
        {
            GUIDate.GetComponent<Text>().text = value;
            PlayerPrefs.SetString("Date_" + textRank, value);
        }
    }
}

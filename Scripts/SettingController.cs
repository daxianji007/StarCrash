using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingController : MonoBehaviour {

    public GameObject[] Languages;
    public GameObject[] Audio;
    public GameObject InputField;
    public GameObject Plane;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void InitSetting()
    {
        if (!PlayerPrefs.HasKey("Language"))
        {
            //default is 0, English
            PlayerPrefs.SetInt("Language", 0);
        }

        for (int i = 0; i < Languages.Length; i++)
        {
            if (PlayerPrefs.GetInt("Language") != i)
            {
                Languages[i].GetComponent<Toggle>().isOn = false;
            }
            else
            {
                Languages[i].GetComponent<Toggle>().isOn = true;
            }
        }

        if (!PlayerPrefs.HasKey("Audio"))
        {
            PlayerPrefs.SetInt("Audio", 0);
        }

        for (int i = 0; i < Audio.Length; i++)
        {
            if (PlayerPrefs.GetInt("Audio") != i)
            {
                Audio[i].GetComponent<Toggle>().isOn = false;
            }
            else
            {
                Audio[i].GetComponent<Toggle>().isOn = true;
            }
        }

        if (PlayerPrefs.HasKey("DefaultName"))
        {
            InputField.GetComponentInChildren<InputField>().text = PlayerPrefs.GetString("DefaultName");
        }
    }

    public void SetDefaultName()
    {
        string name = InputField.GetComponentInChildren<InputField>().text;
        PlayerPrefs.SetString("DefaultName", name);
        Plane.GetComponent<GameController>().SetHelpButton();
    }

    public void SetChosenLanguage(int x)
    {
        if (Languages[x].GetComponent<Toggle>().isOn == true)
        {
            for (int i = 0; i < Languages.Length; i++)
            {
                if (x != i)
                {
                    Languages[i].GetComponent<Toggle>().isOn = false;
                }
            }
        }
        PlayerPrefs.SetInt("Language", x);
        Plane.GetComponent<GameController>().SetAllLanguage();
    }

    public void SetChosenAudio(int x)
    {
        if (Audio[x].GetComponent<Toggle>().isOn == true)
        {
            for (int i = 0; i < Audio.Length; i++)
            {
                if (x != i)
                {
                    Audio[i].GetComponent<Toggle>().isOn = false;
                }
            }
        }

        //0 is on, 1 is off
        PlayerPrefs.SetInt("Audio", x);
        Plane.GetComponent<AudioController>().StartBackgroundAudio();
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LanguageController : MonoBehaviour {

    public GameObject Buttons;
    public GameObject GamePanel;
    public GameObject RankPanel;
    public GameObject SettingPanel;
    public GameObject HelpPanel;
    public GameObject UIPanel;

    int language = 0;
    
    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public string GetString(string name)
    {
        if(language == 0)
        {
            if (name == "help") return "Help";
            if (name == "score") return "Score ";

        }
        else if(language == 1)
        {
            if (name == "help") return "求助";
            if (name == "score") return "分数 ";

        }
        return "Null";
    }

    //This did not contain the question language and help button
    public void SetLanguage(int language)
    {
        this.language = language;
        if (language == 0)
        {
            //English
            //Menus
            Buttons.transform.Find("Button_reStart").GetComponentInChildren<Text>().text = "Start";
            Buttons.transform.Find("Button_regret").GetComponentInChildren<Text>().text = "Undo";
            Buttons.transform.Find("Button_scoreRank").GetComponentInChildren<Text>().text = "Rank";
            Buttons.transform.Find("Button_settings").GetComponentInChildren<Text>().text = "Settings";
            //Buttons.transform.Find("Button_help").GetComponentInChildren<Text>().text = "Help";
            Buttons.transform.Find("Button_quit").GetComponentInChildren<Text>().text = "Exit";

            //Score

            //restore
            UIPanel.transform.Find("RestoreText").GetComponentInChildren<Text>().text = "Automatically restore unsaved game data...";            

            //Rank
            RankPanel.transform.GetChild(0).transform.Find("ScoreText").GetComponentInChildren<Text>().text = "Score";
            RankPanel.transform.GetChild(0).transform.Find("LabelText").GetComponentInChildren<Text>().text = "Label";
            RankPanel.transform.GetChild(0).transform.Find("DateText").GetComponentInChildren<Text>().text = "Date";

            //Setting
            SettingPanel.transform.Find("SettingName").transform.Find("DefaultName").GetComponentInChildren<Text>().text = "Default Name:";
            SettingPanel.transform.Find("SettingLanguage").transform.Find("DefaultLanguage").GetComponentInChildren<Text>().text = "Language:";
            SettingPanel.transform.Find("Audio").Find("DefaultAudio").GetComponentInChildren<Text>().text = "Audio:";
            SettingPanel.transform.Find("Audio").Find("On").GetComponentInChildren<Text>().text = "On";
            SettingPanel.transform.Find("Audio").Find("Off").GetComponentInChildren<Text>().text = "Off";
            SettingPanel.transform.Find("Author").GetComponentInChildren<Text>().text = "Author:\nJiangshuang007@126.com";

            //Help
            HelpPanel.transform.Find("Description").GetComponentInChildren<Text>().text = "Since this game is your favourite. How do you know about my favourite?";
            HelpPanel.transform.Find("Button_Submit").GetComponentInChildren<Text>().text = "Submit!";
        }
        else if(language == 1)
        {
            //Chinese
            //Menus
            Buttons.transform.Find("Button_reStart").GetComponentInChildren<Text>().text = "开始";
            Buttons.transform.Find("Button_regret").GetComponentInChildren<Text>().text = "撤销";
            Buttons.transform.Find("Button_scoreRank").GetComponentInChildren<Text>().text = "排行";
            Buttons.transform.Find("Button_settings").GetComponentInChildren<Text>().text = "设置";
            //Buttons.transform.Find("Button_help").GetComponentInChildren<Text>().text = "求助";
            Buttons.transform.Find("Button_quit").GetComponentInChildren<Text>().text = "退出";

            //Score

            //restore
            UIPanel.transform.Find("RestoreText").GetComponentInChildren<Text>().text = "自动恢复上次未保存的游戏...";

            //Rank
            RankPanel.transform.GetChild(0).transform.Find("ScoreText").GetComponentInChildren<Text>().text = "得分";
            RankPanel.transform.GetChild(0).transform.Find("LabelText").GetComponentInChildren<Text>().text = "标识";
            RankPanel.transform.GetChild(0).transform.Find("DateText").GetComponentInChildren<Text>().text = "日期";

            //Setting
            SettingPanel.transform.Find("SettingName").transform.Find("DefaultName").GetComponentInChildren<Text>().text = "默认用户名:";
            SettingPanel.transform.Find("SettingLanguage").transform.Find("DefaultLanguage").GetComponentInChildren<Text>().text = "语言:";
            SettingPanel.transform.Find("Audio").Find("DefaultAudio").GetComponentInChildren<Text>().text = "音效:";
            SettingPanel.transform.Find("Audio").Find("On").GetComponentInChildren<Text>().text = "开";
            SettingPanel.transform.Find("Audio").Find("Off").GetComponentInChildren<Text>().text = "关";
            SettingPanel.transform.Find("Author").GetComponentInChildren<Text>().text = "作者:\nJiangshuang007@126.com";

            //Help
            HelpPanel.transform.Find("Description").GetComponentInChildren<Text>().text = "既然这是你最喜欢的游戏，你知道我喜欢的是什么吗?";
            HelpPanel.transform.Find("Button_Submit").GetComponentInChildren<Text>().text = "确定!";
        }
    }
}

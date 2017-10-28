using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestionController : MonoBehaviour {

    public GameObject gameController;
    public GameObject helpButton;
    public GameObject question;
    public GameObject[] answers;
    public int helpCount = 2;

    //Private usgage
    private System.Random random = new System.Random();

    private string[,] questionData;

    private string[,] questionData_enlish = {
        { "Which flavor is my favourite?", "Spicy", "Sweet","Sour", "Spicy" },
        { "Which number is my favourite?", "7", "3","5", "7" },
        { "Which hero in dota is my favourite?", "UNDYING", "UNDYING","JUGGERNAUT", "WARLOCK" },
        { "Which season is my favourite?", "Autumn", "Spring", "Autumn", "Winter" },
        { "Which company is my favourite?", "Google", "Apple", "Google", "Microsoft" },
        { "Which sport is my favourite?", "Basketball", "Basketball", "running", "sleep" },
        { "Which singer is my favourite?", "Jam Hsiao", "G.D", "Adam Mitchel Lambert", "Jam Hsiao" },
        { "Which actress do I prefer?", "Angelababy", "Anne Hathaway", "Angelababy", "Angelina Jolie" },
        { "Which NBA team do I prefer?", "Los Angeles Clippers", "Los Angeles Clippers","Houston Rockets", "Miami Heat" },
        { "Which programming language do I prefer when developing small tools?", "JavaScript", "C#","JavaScript", "PHP" },
        { "Which drink do I prefer?", "Coke", "Coffee","Green Tea", "Coke" },
        { "Which snack do I prefer?", "Daguokui", "Dumplings","Daguokui", "Fried Chicken" },
        { "Which is my favorite movie？", "The Prestige", "The Prestige","Titanic", "Forrest Gump" },
        { "Which of the following is my favorite dish？", "Twice-cooked pork", "Steak","Twice-cooked pork", "Fish and Chips" },
        { "I prefer which of the following colors？", "Blue", "Red","Yellow", "Blue" },
        { "I prefer to wear clothes of what color？", "White", "White","Red", "Black" },
        { "I like which planet in solar system？", "Mars", "Mercury","Mars", "Pluto" },
        { "What is my favorite fruit？", "Lychee", "mango","apple", "Lychee" },
        { "Which of the following online games is my favorite？", "Stone Age", "Stone Age","Legends of the Wulin", "World of Warcraft" },
        { "Which of the following PC game is my favorite？", "Legends of the Wulin", "Stone Age","Legends of the Wulin", "World of Warcraft" },
        { "I like what kind of animal?", "Alpaca", "Panda","Lion", "Alpaca" },
        { "To go shopping I prefer which transpotation？", "Bicycle", "Bicycle","Metro", "Car" },
        { "I prefer which of the following points of interest？", "West Lake", "Grand Canyon","West Lake", "Mount Fuji" },
        { "I prefer to play which kind of board game？", "Mahjong", "Texas Hold'em","Legends of the Three Kingdoms", "Mahjong" },
        { "What is my favorite snack？", "Potato Chips", "Potato Chips","Beef Jerky", "Chocolate" },
        { "Normally I like to sing which song in KTV？", "Empress", "Take Me to Your Heart","Empress", "Happy Worship" },
        { "Which weather is my favorite？", "Sunny", "Rainy","Cloudy", "Sunny" },
        { "I prefer to use what kind of cell phone？", "Android phone", "iPhone","Android phone", "WinPhone" },
        { "What kind of OS do I prefer？", "Windows", "Windows","Mac OS", "Ubuntu" },
        { "What vegetable is my favorite？", "Potato", "eggplant","Potato", "broccoli" },
        { "Which meat do I prefer to eat？", "Beef", "Fish","Chicken", "Beef" },
        { "How do you know about my favorite flower？", "Gardenia", "Gardenia","Rose", "Epiphyllum" },
        { "I tend to use which side of teeth for eating？", "Right Side", "Right Side","Middle Side", "Left Side" },
        { "Who is my favorite NBA player？", "Michael Jordan", "Yao Ming","Michael Jordan", "LeBron James" },
        { "Who is my favorite historical figure in the Three Kingdoms period？", "Zhuge Liang", "Zhao Yun","Sheldon", "Zhuge Liang" },
    };
    private string[,] questionData_chinese = {
        { "我最喜欢哪种味道?", "辣", "甜","酸", "辣" },
        { "我最喜欢什么数字?", "7", "3","5", "7" },
        { "在dota游戏里，我最喜欢使用哪位英雄?", "不朽尸王", "不朽尸王","剑圣", "术士" },
        { "我最喜欢哪个季节?", "秋", "春", "秋", "冬" },
        { "哪家公司是我最喜欢的?", "谷歌", "苹果", "谷歌", "微软" },
        { "我最喜欢哪项运动?", "篮球", "篮球", "跑步", "睡觉" },
        { "我最喜爱哪位歌星?", "萧敬腾", "权志龙", "亚当·兰伯特", "萧敬腾" },
        { "我更喜欢以下哪位女星?", "Angelababy", "安妮海瑟薇", "Angelababy", "安吉丽娜朱莉" },
        { "我更喜欢以下哪支NBA球队?", "洛杉矶快船", "洛杉矶快船","休斯顿火箭", "迈阿密热火" },
        { "我开发小程序更喜欢使用哪个编程语言?", "JavaScript", "C#","JavaScript", "PHP" },

        { "我更倾向于喝哪种饮料?", "可乐", "咖啡","绿茶", "可乐" },
        { "我更喜欢哪个小吃?", "锅盔", "小笼包","锅盔", "炸鸡" },
        { "以下哪部电影是我的最爱？", "致命魔术", "致命魔术","泰坦尼克号", "阿甘正传" },
        { "以下哪道菜是我的最爱？", "回锅肉", "水煮鱼","回锅肉", "糖醋排骨" },
        { "我更喜欢以下哪个颜色？", "蓝", "红","黄", "蓝" },
        { "我更喜欢穿哪种颜色的衣服？", "白色", "白色","红色", "黑色" },
        { "太阳系的行星我更喜欢哪个？", "火星", "水星","火星", "冥王星" },
        { "我最爱吃什么水果？", "荔枝", "芒果","苹果", "荔枝" },
        { "以下哪个网络游戏是我的最爱？", "石器时代", "石器时代","武林群侠传", "魔兽世界" },
        { "以下哪个单机游戏是我的最爱？", "武林群侠传", "石器时代","武林群侠传", "魔兽世界" },
        { "我更喜欢哪种动物？", "羊驼", "熊猫","狮子", "羊驼" },
        { "去逛街我更喜欢使用什么交通工具？", "自行车", "自行车","地铁", "开车" },
        { "我更喜欢以下哪个景点？", "西湖", "大峡谷","西湖", "富士山" },
        { "我更喜欢玩哪种桌游？", "麻将", "德州扑克","三国杀", "麻将" },
        { "我最爱吃什么零食？", "薯片", "薯片","牛肉干", "巧克力" },
        { "我去KTV一般更喜欢唱哪首歌？", "王妃", "吻别","王妃", "快乐崇拜" },
        { "我最喜欢什么天气？", "晴空万里", "绵绵细雨","白云朵朵", "晴空万里" },
        { "我更喜欢使用哪种类型的手机？", "安卓手机", "苹果手机","安卓手机", "WinPhone手机" },
        { "我更在PC上使用哪种操作系统？", "Windows", "Windows","Mac OS", "Ubuntu" },
        { "我最喜欢吃哪种蔬菜？", "土豆", "茄子","土豆", "西蓝花" },
        { "我最喜欢吃哪种肉类？", "牛肉", "鱼肉","鸡肉", "牛肉" },
        { "我最喜欢什么花？", "栀子花", "栀子花","玫瑰", "昙花" },
        { "我吃饭倾向于使用哪一边的牙？", "右边", "右边","中间", "左边" },
        { "我最喜欢的NBA球员是谁？", "迈克尔乔丹", "姚明","迈克尔乔丹", "勒布朗詹姆斯" },
        { "我最喜欢三国时期的历史人物是？", "诸葛亮", "赵云","夏侯惇", "诸葛亮" },
    };
    private int questionIndex;
    private string questionAnswer;

    QuestionController()
    {
        SetQuestionData(0);
    }

    // Use this for initialization
    void Start () {
        //InitQuestion();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetQuestionData(int language)
    {
        if(language == 1)
        {
            questionData = questionData_chinese;
        }
        else
        {
            questionData = questionData_enlish;
        }
    }

    public void Submit()
    {
        for (int i = 0; i < answers.Length; i++)
        {
            if (answers[i].GetComponent<Toggle>().isOn == true)
            {
                helpCount--;
                helpButton.GetComponentInChildren<Text>().text = gameController.GetComponent<LanguageController>().GetString("help") + string.Format("({0})", helpCount);
                if (helpCount == 0)
                {
                    helpButton.SetActive(false);
                }
                if (answers[i].GetComponentInChildren<Text>().text == questionAnswer)
                {
                    gameController.GetComponent<GameController>().SwitchPanel(0);
                    gameController.GetComponent<GameController>().StartExposion();
                }
                else
                {
                    gameController.GetComponent<GameController>().SwitchPanel(0);
                }
                return;
            }
        }
    }

    public void InitQuestion()
    {
        questionIndex = random.Next(questionData.GetLength(0));
        question.GetComponentInChildren<Text>().text = questionData[questionIndex,0];
        questionAnswer = questionData[questionIndex, 1];
        answers[0].GetComponentInChildren<Text>().text = questionData[questionIndex, 2];
        answers[1].GetComponentInChildren<Text>().text = questionData[questionIndex, 3];
        answers[2].GetComponentInChildren<Text>().text = questionData[questionIndex, 4];
    }

    public void InitMarryQuestion()
    {
        questionIndex = 0;
        question.GetComponentInChildren<Text>().text = "So... Will you marry me?";
        questionAnswer = "Yes, I will";
        answers[0].GetComponentInChildren<Text>().text = "Yes, I will";
        answers[1].GetComponentInChildren<Text>().text = "Yes, I will";
        answers[2].GetComponentInChildren<Text>().text = "Yes, I will";
    }

    public void SetChosen(int x)
    {
        if(answers[x].GetComponent<Toggle>().isOn == true)
        {
            for (int i = 0; i < answers.Length; i++)
            {
                if (x != i)
                {
                    answers[i].GetComponent<Toggle>().isOn = false;
                }
            }
        }        
    }
}

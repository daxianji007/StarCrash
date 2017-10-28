using UnityEngine;
using System.Collections;
using Assets.Script;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    //references
    public int size;
    public GameObject spherePrefab;
    public GameObject matrixSphereCenter;
    public GameObject maskCenter;
    public GameObject mask;
    public Material[] materials;
    public Texture[] textures;
    public GameObject[] panels;
    public Text scoreGUI;
    public Text restoreGUI;
    public RawImage upperStar;
    public RawImage lowerStar;
    public float starSpeed;
    public GameObject GUIGameParent;
    public GameObject GUIRankParent;
    public GameObject GUIHelpParent;
    public GameObject GUIRankLine;
    public GameObject helpButton;

    //the balls GameObject
    private GameObject[,] spheres;
    private GameObject[,] masks;

    //Core game data
    private GameData gameData;
    private GameObject[] rankLines;
    private int currentPanel;
    private int marryActiveCount = 0;

    //star dx
    private float dx;

    // Use this for initialization
    void Start () {
        Init();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && gameData.gameStatus == GameStatus.IDLE)
        {
            Vector3 screenWithZ = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5);
            Vector3 worldPosistion = Camera.main.ScreenToWorldPoint(screenWithZ);
            int x = Mathf.FloorToInt(worldPosistion.x + ((float)size) / 2);
            int y = Mathf.FloorToInt(worldPosistion.y + ((float)size) / 2);
            gameData.SelectBall(x, y);
        }
        scoreGUI.text = this.GetComponent<LanguageController>().GetString("score") + ":" + gameData.score.ToString();

        //For the stars, the image position should be int?? so that it can be transparent??
        dx = dx + starSpeed * Time.deltaTime;
        while (dx > 12.0f) dx -= 12.0f;
        upperStar.transform.position = GetIntVector3(Camera.main.WorldToScreenPoint(new Vector3(-6f + dx, 4.5f)));
        lowerStar.transform.position = GetIntVector3(Camera.main.WorldToScreenPoint(new Vector3( 6f - dx,-4.5f)));

        //Animations
        if(gameData.gameStatus == GameStatus.ANIMATING)
        {
            if(gameData.animation.animationType == AnimationType.MOVING)
            {
                //Audio
                GetComponent<AudioController>().StartAudioEffect(AudioController.Audio.MOVING, id: "Moving");

                //Set position
                KeyValuePair<int, int> startPostion = gameData.animation.movingPath[0];
                GameObject movingBall = spheres[startPostion.Key, startPostion.Value];
                float stepTimeMs = 500 / (gameData.animation.movingPath.Count - 1);
                float dtime = (float) (System.DateTime.Now - gameData.animation.startDate).TotalMilliseconds;
                int index = 0;
                while(dtime > stepTimeMs)
                {
                    dtime -= stepTimeMs;
                    index++;
                }
                if(index < gameData.animation.movingPath.Count - 1)
                {
                    KeyValuePair<int, int> p1 = gameData.animation.movingPath[index];
                    KeyValuePair<int, int> p2 = gameData.animation.movingPath[index + 1];
                    float tx = (p1.Key * (stepTimeMs - dtime) + p2.Key * dtime) / stepTimeMs;
                    float ty = (p1.Value * (stepTimeMs - dtime) + p2.Value * dtime) / stepTimeMs;
                    movingBall.transform.position = spherePrefab.transform.position + new Vector3(0, 0, -1) + new Vector3(tx - (size - 1f) / 2, ty - (size - 1f) / 2, 0);
                }
                else
                {
                    //Stop the animation and callback
                    gameData.gameStatus = GameStatus.IDLE;
                    movingBall.transform.position = spherePrefab.transform.position + new Vector3(0, 0, -1) + new Vector3(startPostion.Key - (size - 1f) / 2, startPostion.Value - (size - 1f) / 2, 0);
                    gameData.animation.callBack();
                    GetComponent<AudioController>().ClearId();
                }
            }
            else if (gameData.animation.animationType == AnimationType.GROWING)
            {
                //Audio
                GetComponent<AudioController>().StartAudioEffect(AudioController.Audio.GROWING, id: "Growing");

                float stepTimeMs = 250;
                float dtime = (float)(System.DateTime.Now - gameData.animation.startDate).TotalMilliseconds;
                if (dtime < stepTimeMs)
                {
                    foreach (var pair in gameData.animation.growList)
                    {
                        spheres[pair.Key, pair.Value].transform.localScale = (new Vector3(0.5f, 0.5f, 0.5f) + new Vector3(0.5f, 0.5f, 0.5f) * dtime / stepTimeMs) * 0.95f;                        
                    }
                }
                else
                {
                    //Stop the animation and callback
                    gameData.gameStatus = GameStatus.IDLE;
                    gameData.animation.callBack();
                    GetComponent<AudioController>().ClearId();
                }
            }
            else if (gameData.animation.animationType == AnimationType.COLLAPSING)
            {
                float stepTimeMs = 100;
                float dtime = (float)(System.DateTime.Now - gameData.animation.startDate).TotalMilliseconds;
                int index = 0;
                while (dtime > stepTimeMs)
                {
                    dtime -= stepTimeMs;
                    index++;
                }
                if (index < 5)
                {
                    //Audio
                    GetComponent<AudioController>().StartAudioEffect(AudioController.Audio.FLASH, id: "Flash");

                    foreach (var pair in gameData.animation.collapseList)
                    {
                        if (index == 1 || index == 3)
                        {
                            spheres[pair.Key, pair.Value].SetActive(false);
                        }
                        else
                        {
                            spheres[pair.Key, pair.Value].SetActive(true);
                        }
                    }
                }
                else
                {
                    //Stop the animation and callback
                    gameData.gameStatus = GameStatus.IDLE;
                    gameData.animation.callBack();
                    GetComponent<AudioController>().ClearId();
                }
            }
            else if (gameData.animation.animationType == AnimationType.EXPOSION)
            {
                //Stage 1. exposion x trigger dealine
                float step1TimeMs = 1000;
                //Stage 2. exposion x effect dealine
                float step2TimeMs = 1500;
                //Stage 3. exposion y trigger dealine
                float step3TimeMs = 2500;
                //Stage 4. exposion y effect dealine
                float step4TimeMs = 3500;

                float dtime = (float)(System.DateTime.Now - gameData.animation.startDate).TotalMilliseconds;

                if (dtime < step1TimeMs)
                {
                    float stepTimeMs = step1TimeMs / size;
                    float dtimeMs = dtime;
                    int index = 0;
                    while (dtimeMs > stepTimeMs)
                    {
                        dtimeMs -= stepTimeMs;
                        index++;
                    }
                    if(index < size)
                    {
                        spheres[gameData.animation.exposionX, index].transform.localScale = new Vector3(0f, 0f, 0f);
                        spheres[gameData.animation.exposionX, index].GetComponent<RotationController>().exposion.GetComponent<ParticleSystem>().Play();
                        //Audio
                        if (spheres[gameData.animation.exposionX, index].active == true) {
                            GetComponent<AudioController>().StartAudioEffect(AudioController.Audio.EXPOSING, id: "X" + index.ToString());
                        }
                    }
                }
                if (dtime < step2TimeMs)
                {
                    //step2
                }
                else if (dtime < step3TimeMs)
                {
                    float stepTimeMs = (step3TimeMs - step2TimeMs) / size;
                    float dtimeMs = dtime - step2TimeMs;
                    int index = 0;
                    while (dtimeMs > stepTimeMs)
                    {
                        dtimeMs -= stepTimeMs;
                        index++;
                    }
                    if (index < size)
                    {
                        spheres[index, gameData.animation.exposionY].transform.localScale = new Vector3(0f, 0f, 0f);
                        spheres[index, gameData.animation.exposionY].GetComponent<RotationController>().exposion.GetComponent<ParticleSystem>().Play();
                        //Audio
                        if (spheres[index, gameData.animation.exposionY].active == true)
                        {
                            GetComponent<AudioController>().StartAudioEffect(AudioController.Audio.EXPOSING, id: "Y" + index.ToString());
                        }
                    }
                }
                if (dtime < step4TimeMs)
                {
                    //step4
                }
                else
                {
                    //Stop the animation and callback
                    gameData.gameStatus = GameStatus.IDLE;
                    gameData.animation.callBack();
                    GetComponent<AudioController>().ClearId();
                }
            }
        }
    }

    private void Init()
    {
        //create spheres
        spheres = new GameObject[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                spheres[i, j] = Instantiate(spherePrefab,
                                spherePrefab.transform.position + new Vector3(0, 0, -6) + new Vector3(i - (size - 1f) / 2, j - (size - 1f) / 2, 0),
                                spherePrefab.transform.rotation) as GameObject;
                spheres[i, j].transform.SetParent(matrixSphereCenter.transform, true);
            }
        }

        //Create mask
        float halfBallSize = (Screen.width + 0f) / size / 2;
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(halfBallSize * 2, halfBallSize * 2);
        masks = new GameObject[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                masks[i, j] = Instantiate(mask,
                              maskCenter.transform.position + new Vector3(0, 0, -6) + new Vector3(i - (size - 1f) / 2, j - (size - 1f) / 2, 0) * halfBallSize * 2,
                              maskCenter.transform.rotation) as GameObject;
                masks[i, j].transform.SetParent(maskCenter.transform, true);
            }
        }
        mask.SetActive(false);
        maskCenter.SetActive(false);

        //init GameData
        gameData = new GameData(size, spherePrefab, spheres, materials, maskCenter, masks, textures, this, this.GetComponent<AudioController>());
        gameData.InitMatrix();

        //Try to restore Game data
        ReStart(restoreData: true);

        //auto-apply the screen size
        Camera.main.orthographicSize = 4.5f / Screen.width * Screen.height;

        //moving stars position
        dx = 0f;

        //Create the rank panel
        CreateRankPanel();

        //Init the language
        //PlayerPrefs.DeleteKey("Language");
        SetAllLanguage();

        //Start Background Audio
        this.GetComponent<AudioController>().StartBackgroundAudio();
    }

    public void SetAllLanguage()
    {
        int language;//default is 0, English
        if (PlayerPrefs.HasKey("Language"))
        {
            language = PlayerPrefs.GetInt("Language");
        }
        else
        {
            language = 0;
        }
        GUIHelpParent.GetComponent<QuestionController>().SetQuestionData(language);
        this.GetComponent<LanguageController>().SetLanguage(language);
        helpButton.GetComponentInChildren<Text>().text = this.GetComponent<LanguageController>().GetString("help")
            + string.Format("({0})", GUIHelpParent.GetComponent<QuestionController>().helpCount);
    }

    private void CreateRankPanel()
    {
        //PlayerPrefs.DeleteAll();
        rankLines = new GameObject[11];
        rankLines[0] = GUIRankLine;
        GUIRankLine.GetComponent<RankLineManager>().InitPosition(0);
        for (int i = 1; i < 11; i++)
        {
            rankLines[i] = Instantiate(GUIRankLine, GUIRankLine.transform.position, GUIRankLine.transform.rotation) as GameObject;
            rankLines[i].transform.SetParent(GUIRankParent.transform, true);
            rankLines[i].GetComponent<RankLineManager>().InitRankData(i);
            rankLines[i].GetComponent<RankLineManager>().InitPosition(i);
        }
        return ;
    }

    private Vector3 GetIntVector3(Vector3 vec)
    {
        return new Vector3((int)vec.x, (int)vec.y, (int)vec.z);
    }

    void OnMouseDown()
    {
        //Debug.Log("Hello");
    }

    public void HandleGameOverRank()
    {
        int i = 1;
        for (; i < 11; i++)
        {
            //if(int.Parse(rankLines[i].GetComponent<RankLineManager>().Score) <= gameData.score) break;
            int tempScore;
            if (int.TryParse(rankLines[i].GetComponent<RankLineManager>().Score, out tempScore))
            {
                if (tempScore <= gameData.score) break;
            }
            else
            {
                break;
            }
        }
        if(i != 11)
        {
            for(int j=10;j > i; j--)
            {
                rankLines[j].GetComponent<RankLineManager>().Score = rankLines[j - 1].GetComponent<RankLineManager>().Score;
                rankLines[j].GetComponent<RankLineManager>().Label = rankLines[j - 1].GetComponent<RankLineManager>().Label;
                rankLines[j].GetComponent<RankLineManager>().Date = rankLines[j - 1].GetComponent<RankLineManager>().Date;
            }
            rankLines[i].GetComponent<RankLineManager>().SetEdit(gameData.score.ToString());
        }
    }

    //Panel controller, do not call it when animation!    
    public void SwitchPanel(int index)
    {
        if (index == 0)
        {
            if (gameData.gameStatus == GameStatus.PAUSING)
            {
                gameData.gameStatus = GameStatus.IDLE;
            }
            matrixSphereCenter.SetActive(true);
            ActivePanel(index);
        }
        else
        {
            if (gameData.gameStatus == GameStatus.IDLE)
            {
                gameData.gameStatus = GameStatus.PAUSING;
            }
            matrixSphereCenter.SetActive(false);
            ActivePanel(index);
        }
    }
    private void ActivePanel(int index)
    {
        foreach(var panel in panels)
        {
            panel.SetActive(false);
        }
        panels[index].SetActive(true);
        currentPanel = index;
    }

    //Button actions
    public void SettingPanel()
    {
        if (gameData.gameStatus != GameStatus.ANIMATING)
        {
            if (currentPanel == 4)
            {
                SwitchPanel(0);
            }
            else
            {
                SwitchPanel(4);
                panels[4].GetComponent<SettingController>().InitSetting();
            }
        }
    }

    public void HelpPanel()
    {
        //Marry code
        if(marryActiveCount >= 10 && GUIHelpParent.GetComponent<QuestionController>().helpCount == 1)
        {
            Debug.Log("MarryPanel!");
            if (gameData.gameStatus != GameStatus.ANIMATING && gameData.gameStatus != GameStatus.OVER)
            {
                if (currentPanel == 3)
                {
                    SwitchPanel(0);
                }
                else
                {
                    SwitchPanel(3);
                }
            }
            return;
        }

        if (gameData.gameStatus != GameStatus.ANIMATING && gameData.gameStatus != GameStatus.OVER)
        {
            if (currentPanel == 2)
            {
                SwitchPanel(0);
            }
            else
            {
                SwitchPanel(2);
                GUIHelpParent.GetComponent<QuestionController>().InitQuestion();
            }
        }
    }

    public void StartExposion()
    {
        if (gameData.gameStatus == GameStatus.IDLE)
        {
            gameData.StartExposion();
        }
    }

    public void NextStep()
    {
        gameData.NextStep();
        gameData.UpdateSphereByMatrix();
    }

    public void ScoreRank()
    {
        if(gameData.gameStatus != GameStatus.ANIMATING)
        {
            if(currentPanel == 1)
            {
                SwitchPanel(0);
            }
            else
            {
                SwitchPanel(1);
            }
        }
    }

    public void SetHelpButton()
    {
        if(GUIHelpParent.GetComponent<QuestionController>().helpCount != 0 && PlayerPrefs.GetString("DefaultName") == "toto")
        {
            helpButton.SetActive(true);
        }
        else
        {
            helpButton.SetActive(false);
        }
    }

    public void ReStart(bool restoreData = false)
    {
        if (gameData.gameStatus != GameStatus.ANIMATING)
        {
            gameData.gameStatus = GameStatus.IDLE;
            SwitchPanel(0);

            if (restoreData && PlayerPrefs.HasKey("UnsavedGameData"))
            {
                string unsavedGameData = PlayerPrefs.GetString("UnsavedGameData"); 
                gameData.LoadFromUsavedGameDataString(unsavedGameData);
                restoreGUI.CrossFadeAlpha(0.0f, 2.5f, false);
            }
            else
            {
                GUIHelpParent.GetComponent<QuestionController>().helpCount = 2;
                gameData.InitGame();
                gameData.NextStep();
                gameData.NextStep();
                restoreGUI.enabled = false;
            }
            SetHelpButton();
            helpButton.GetComponentInChildren<Text>().text = this.GetComponent<LanguageController>().GetString("help") + string.Format("({0})", 2);
            gameData.SaveRegretMatrix();
            gameData.UpdateSphereByMatrix();
        }
    }

    public void Regret()
    {
        if (gameData.gameStatus == GameStatus.IDLE)
        {
            gameData.RegretMatrix();
            gameData.UpdateSphereByMatrix();
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void IncreaseMarryActiveCount()
    {
        if(currentPanel == 1 && marryActiveCount < 6)
        {
            marryActiveCount++;
        }
        if (currentPanel == 2 && marryActiveCount > 5)
        {
            marryActiveCount++;
            if (marryActiveCount == 10)
            {
                gameData.score++;
            }
        }
    }

    public void PlayVideo()
    {
        marryActiveCount = 0;
        Debug.Log("Playing movie!");
        //Handheld.PlayFullScreenMovie("test.mp4", Color.black, FullScreenMovieControlMode.Full);
        //#if UNITY_IPHONE || UNITY_ANDROID
        //Handheld.PlayFullScreenMovie("final.mp4", Color.black, FullScreenMovieControlMode.Full);
        //#endif
        SwitchPanel(2);
        GUIHelpParent.GetComponent<QuestionController>().InitMarryQuestion();
    }
}

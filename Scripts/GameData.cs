using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script
{
    [Serializable]
    class UsavedGameData
    {
        public int score;
        public int helpCount;

        // two-dimensional-array is not Serializable, hao qi!
        public Ball[] matrix;
    }

    class GameData
    {
        //Controller
        public GameController gameController;
        public AudioController audioController;

        //Input arguments
        public int size;
        public Ball[,] matrix;
        public GameObject spherePrefab;
        public GameObject[,] spheres;
        public GameObject[,] masks;
        public GameObject maskCenter;
        public Material[] materials;
        public Texture[] textures;

        //For game rule control
        public int score;
        public int regretScore;
        public Ball selectedBall;
        public GameStatus gameStatus;
        public Animation animation;
        public Ball[,] regretMatrix;

        //Private usgage
        private System.Random random = new System.Random();

        public GameData(int size, GameObject spherePrefab, GameObject[,] spheres, Material[] materials, GameObject maskCenter, GameObject[,] masks, Texture[] textures, GameController gameController, AudioController audioController)
        {
            this.size = size;
            this.spherePrefab = spherePrefab;
            this.spheres = spheres;
            this.materials = materials;
            this.maskCenter = maskCenter;
            this.masks = masks;
            this.textures = textures;
            this.gameController = gameController;
            this.audioController = audioController;
        }

        public string ToUsavedGameDataString()
        {
            UsavedGameData usavedGameData = new UsavedGameData
            {
                score = score,
                helpCount = gameController.GUIHelpParent.GetComponent<QuestionController>().helpCount,
                matrix = ArrayConver_2Dto1D(matrix),
            };
            //Debug.Log(JsonUtility.ToJson(usavedGameData));
            return JsonUtility.ToJson(usavedGameData);
        }

        public void LoadFromUsavedGameDataString(string usavedGameDataString)
        {
            UsavedGameData usavedGameData = JsonUtility.FromJson<UsavedGameData>(usavedGameDataString);
            score = usavedGameData.score;
            gameController.GUIHelpParent.GetComponent<QuestionController>().helpCount = usavedGameData.helpCount;
            matrix = ArrayConver_1Dto2D(usavedGameData.matrix, size, size);
        }

        private T[] ArrayConver_2Dto1D<T>(T[,] source)
        {
            int width = source.GetLength(0);
            int height = source.GetLength(1);
            List<T> result = new List<T>();
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    result.Add(source[i,j]);
                }
            }
            return result.ToArray();
        }

        private T[,] ArrayConver_1Dto2D<T>(T[] source, int width, int height)
        {
            T[,] result = new T[width,height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i, j] = source[i * width + j];
                }
            }
            return result;
        }

        public void InitGame()
        {
            score = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j].present = PresentType.NONE;
                }
            }
        }

        public void InitMatrix()
        {
            //new matrix
            matrix = new Ball[size, size];
            regretMatrix = new Ball[size, size];

            //initialize
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = new Ball(i, j);
                    matrix[i, j].type = 0;// random.Next(0, materials.Length);
                    regretMatrix[i, j] = new Ball(i, j);
                    regretMatrix[i, j].type = 0;// random.Next(0, materials.Length);
                }
            }            
        }

        //For game play rules
        public void RegretMatrix()
        {
            score = regretScore;
            CopyMatrix(regretMatrix, matrix);
        }
        public void SaveRegretMatrix()
        {
            regretScore = score;
            CopyMatrix(matrix, regretMatrix);
        }
        public void NextStep()
        {
            List<KeyValuePair<int, int>> growList = GetGrowList();

            //Tiny to big
            foreach(var pair in growList)
            {
                matrix[pair.Key, pair.Value].present = PresentType.BIG;
            }

            //check expose
            List<KeyValuePair<int, int>> collapseList = new List<KeyValuePair<int, int>>();
            foreach(var pair in growList)
            {
                collapseList.AddRange(TestBallCollapse(pair.Key, pair.Value));
            }
            DisableCollapseBalls(collapseList);

            //Check Game over
            if (isGameOver())
            {
                gameStatus = GameStatus.OVER;
                gameController.HandleGameOverRank();
                gameController.ScoreRank();
            }

            //non to tiny, here can be bug if the size cannot be divided by 3
            try
            {
                KeyValuePair<int, int> candidate;
                for (int i = 0; i < 3; i++)
                {
                    candidate = RandomPickNonePosistion();
                    matrix[candidate.Key, candidate.Value].present = PresentType.TINY;
                    matrix[candidate.Key, candidate.Value].type = random.Next(materials.Length);
                }
            }
            catch
            {

            }
        }
        public void SelectBall(int x, int y)
        {
            if(x< 0 || y < 0 || x >= size || y >= size)
            {
                if (selectedBall != null)
                {
                    DeSelectBall();
                }
                return;
            }
            if(gameStatus == GameStatus.IDLE)
            {
                if (selectedBall == null)
                {
                    if (matrix[x, y].present == PresentType.BIG)
                    {
                        DoSelectBall(x, y);
                    }
                }
                else
                {
                    if (matrix[x, y].present != PresentType.BIG)
                    {                        
                        //Check reachable
                        List<KeyValuePair<int, int>> path = GetPath(selectedBall.x, selectedBall.y, x, y);
                        if(path.Count == 0)
                        {
                            DeSelectBall();
                        }
                        else
                        {
                            //Set regret matrix
                            SaveRegretMatrix();

                            //set animation queue
                            maskCenter.SetActive(false);
                            gameStatus = GameStatus.ANIMATING;
                            animation = MakeMovingAnimation(path);                            
                        }
                    }
                    else
                    {
                        DeSelectBall();
                    }
                }
            }
            else
            {
                //Code for animation status?
            }
        }

        //For the rendering
        public void UpdateSphereByMatrix(bool savedGameData = true)
        {
            // Save the GameData
            if (savedGameData)
            {
                PlayerPrefs.SetString("UnsavedGameData", this.ToUsavedGameDataString());
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    switch (matrix[i, j].present)
                    {
                        case PresentType.NONE:
                            spheres[i, j].SetActive(false);
                            spheres[i, j].transform.localScale = new Vector3(1, 1, 1) * 0.95f;
                            break;
                        case PresentType.TINY:
                            spheres[i, j].SetActive(true);
                            spheres[i, j].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f) * 0.95f;
                            break;
                        case PresentType.BIG:
                            spheres[i, j].SetActive(true);
                            spheres[i, j].transform.localScale = new Vector3(1, 1, 1) * 0.95f;
                            break;
                        default:
                            break;
                    }                    
                    spheres[i, j].GetComponent<Renderer>().material = materials[matrix[i, j].type];
                }
            }
        }
        public void UpdateMaskByMatrix(Ball[,] GetLastPointMatrix)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (GetLastPointMatrix[i, j] == null)
                    {
                        masks[i, j].GetComponent<RawImage>().texture = textures[1];
                    }
                    else
                    {
                        masks[i, j].GetComponent<RawImage>().texture = textures[0];
                    }
                }
            }
        }

        //For animation
        private Animation MakeMovingAnimation(List<KeyValuePair<int, int>> path)
        {
            Animation result = new Animation();
            result.animationType = AnimationType.MOVING;
            result.startDate = DateTime.Now;
            result.movingPath = path;
            result.callBack = MovingAnimationCallBack;
            return result;
        }

        private void MovingAnimationCallBack()
        {
            //Deselect the moving ball
            selectedBall.present = PresentType.NONE;
            int tempType = selectedBall.type;
            DeSelectBall(withAudio:false);

            //Check if it covers tiny ball
            KeyValuePair<int, int> targetPosition = animation.movingPath.Last();
            int x = targetPosition.Key;
            int y = targetPosition.Value;
            if (matrix[x, y].present == PresentType.TINY)
            {
                KeyValuePair<int, int> candidate = RandomPickNonePosistion();
                matrix[candidate.Key, candidate.Value].present = PresentType.TINY;
                matrix[candidate.Key, candidate.Value].type = matrix[x, y].type;
            }

            //Set the target position ball
            matrix[x, y].present = PresentType.BIG;
            matrix[x, y].type = tempType;
            UpdateSphereByMatrix(savedGameData:false);

            //Test collapse
            var list = TestBallCollapse(x, y);
            if (list.Count == 0)
            {
                gameStatus = GameStatus.ANIMATING;
                animation = MakGrowingAnimation();
                //NextStep();
                //UpdateSphereByMatrix();
            }
            else
            {
                gameStatus = GameStatus.ANIMATING;
                animation = MakCollapseAnimation(list);
                //DisableCollapseBalls(list);
                //UpdateSphereByMatrix();
            }
        }

        private Animation MakGrowingAnimation()
        {
            Animation result = new Animation();
            result.animationType = AnimationType.GROWING;
            result.startDate = DateTime.Now;
            result.growList = GetGrowList();
            result.callBack = GrowingAnimationCallBack;
            return result;
        }

        private void GrowingAnimationCallBack()
        {
            //Tiny to big
            foreach (var pair in animation.growList)
            {
                matrix[pair.Key, pair.Value].present = PresentType.BIG;
            }
            
            //check expose
            List<KeyValuePair<int, int>> collapseList = new List<KeyValuePair<int, int>>();
            foreach(var pair in animation.growList)
            {
                collapseList.AddRange(TestBallCollapse(pair.Key, pair.Value));
            }

            //Notice: This gamedata changes here, but will take effect after the animation because of the UpdateSphereByMatrix() in the callback
            DisableCollapseBalls(collapseList);            

            //non to tiny, here can be bug if the size cannot be divided by 3
            try
            {
                KeyValuePair<int, int> candidate;
                for (int i = 0; i < 3; i++)
                {
                    candidate = RandomPickNonePosistion();
                    matrix[candidate.Key, candidate.Value].present = PresentType.TINY;
                    matrix[candidate.Key, candidate.Value].type = random.Next(materials.Length);
                }
            }
            catch
            {

            }

            //refresh
            //Set animation
            if (collapseList.Count > 0)
            {
                gameStatus = GameStatus.ANIMATING;
                animation = MakCollapseAnimation(collapseList);
            }
            else
            {
                UpdateSphereByMatrix();
            }

            //Check Game over
            if (isGameOver())
            {
                //Audio
                audioController.StartAudioEffect(AudioController.Audio.OVER);

                gameStatus = GameStatus.OVER;
                gameController.HandleGameOverRank();
                gameController.ScoreRank();

                //Clear unsaved data
                PlayerPrefs.DeleteKey("UnsavedGameData");
            }
        }

        private Animation MakCollapseAnimation(List<KeyValuePair<int, int>> collapseList)
        {
            Animation result = new Animation();
            result.animationType = AnimationType.COLLAPSING;
            result.startDate = DateTime.Now;
            result.collapseList = collapseList;
            result.callBack = CollapseAnimationCallBack;
            return result;
        }

        private void CollapseAnimationCallBack()
        {
            foreach (var pair in animation.collapseList)
            {
                matrix[pair.Key, pair.Value].present = PresentType.NONE;
            }
            UpdateSphereByMatrix();

            //Audio scoring
            audioController.PendingScoreAudio();
            return;
        }

        public void StartExposion()
        {
            gameStatus = GameStatus.ANIMATING;
            animation = MakeExposionAnimation();
        }

        private Animation MakeExposionAnimation()
        {
            Animation result = new Animation();
            result.animationType = AnimationType.EXPOSION;
            result.startDate = DateTime.Now;
            result.exposionX = random.Next(size);
            result.exposionY = random.Next(size);
            result.callBack = ExposionAnimationCallBack;
            return result;
        }

        private void ExposionAnimationCallBack()
        {
            Debug.Log("callback!");
            for(int i = 0; i < size; i++)
            {
                matrix[i, animation.exposionY].present = PresentType.NONE;
                matrix[animation.exposionX, i].present = PresentType.NONE;
            }
            UpdateSphereByMatrix();
            return;
        }

        //Private functions
        private void DoSelectBall(int x, int y, bool withAudio=true)
        {
            selectedBall = matrix[x, y];
            spheres[x, y].tag = "Selected";
            (spheres[x, y].GetComponent("Halo") as Behaviour).enabled = true;
            maskCenter.SetActive(true);
            UpdateMaskByMatrix(GetLastPointMatrix(x, y));

            //Audio
            if (withAudio)
            {
                audioController.StartAudioEffect(AudioController.Audio.SELECTED);
            }
        }

        private void DeSelectBall(bool withAudio=true)
        {
            (spheres[selectedBall.x, selectedBall.y].GetComponent("Halo") as Behaviour).enabled = false;
            spheres[selectedBall.x, selectedBall.y].tag = "Untagged";
            selectedBall = null;
            maskCenter.SetActive(false);

            //Audio
            if (withAudio)
            {
                audioController.StartAudioEffect(AudioController.Audio.DESELECT);
            }
        }

        private void CopyMatrix(Ball[,] fromMatrix, Ball[,] toMatrix)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    toMatrix[i, j].present = fromMatrix[i, j].present;
                    toMatrix[i, j].type = fromMatrix[i, j].type;
                }
            }
        }

        private List<KeyValuePair<int, int>> GetGrowList()
        {
            List<KeyValuePair<int, int>> growList = new List<KeyValuePair<int, int>>();            
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (matrix[i, j].present == PresentType.TINY)
                    {
                        growList.Add(new KeyValuePair<int, int>(i, j));
                    }
                }
            }
            return growList;
        }
        private void DisableCollapseBalls(List<KeyValuePair<int, int>> list)
        {
            foreach(var pair in list)
            {
                matrix[pair.Key, pair.Value].present = PresentType.NONE;
            }
        }
        private List<KeyValuePair<int, int>> TestBallCollapse(int x, int y)
        {
            int CollapseCount = 5;
            int[] ScoreMap = { 0, 1, 2, 3, 4, 5, 7, 10, 15, 20, 30, 50, 100 };
            List<KeyValuePair<int, int>> result = new List<KeyValuePair<int, int>>();
            List<KeyValuePair<int, int>> temp = new List<KeyValuePair<int, int>>();
            int tempScore = 0;
            temp.Clear();
            tempScore = GetSameBallLength(x, y, 0, 1, temp) + GetSameBallLength(x, y, 0, -1, temp) + 1;
            if (tempScore >= CollapseCount)
            {
                score += ScoreMap[tempScore];
                result.AddRange(temp);
            }
            temp.Clear();
            tempScore = GetSameBallLength(x, y, 1, 1, temp) + GetSameBallLength(x, y, -1, -1, temp) + 1;
            if (tempScore >= CollapseCount)
            {
                score += ScoreMap[tempScore];
                result.AddRange(temp);
            }
            temp.Clear();
            tempScore = GetSameBallLength(x, y, 1, 0, temp) + GetSameBallLength(x, y, -1, 0, temp) + 1;
            if (tempScore >= CollapseCount)
            {
                score += ScoreMap[tempScore];
                result.AddRange(temp);
            }
            temp.Clear();
            tempScore = GetSameBallLength(x, y, 1, -1, temp) + GetSameBallLength(x, y, -1, 1, temp) + 1;
            if (tempScore >= CollapseCount)
            {
                score += ScoreMap[tempScore];
                result.AddRange(temp);
            }
            if(result.Count > 0)
            {
                result.Add(new KeyValuePair<int, int>(x, y));
            }
            return result;
        }
        private int GetSameBallLength(int x, int y, int mx, int my, List<KeyValuePair<int, int>> temp)
        {
            int d = 1;
            while (TestBallType(x + d * mx, y + d * my) == matrix[x,y].type)
            {
                temp.Add(new KeyValuePair<int, int>(x + d * mx, y + d * my));
                d++;
            }
            return d - 1;
        }
        private int TestBallType(int x, int y)
        {
            if(x < 0 || x >= size || y < 0 || y >= size || matrix[x, y].present != PresentType.BIG)
            {
                return -1;
            }
            else
            {
                return matrix[x, y].type;
            }
        }
        private KeyValuePair<int, int> RandomPickNonePosistion()
        {
            List<KeyValuePair<int, int>> noneList = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (matrix[i, j].present == PresentType.NONE)
                    {
                        noneList.Add(new KeyValuePair<int, int>(i, j));
                    }
                }
            }
            return noneList[random.Next(noneList.Count)];
        }
        private bool isGameOver()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (matrix[i, j].present != PresentType.BIG)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private Ball[,] GetLastPointMatrix(int x1, int y1)
        {
            Ball[,] LastPointMatrix = new Ball[size, size];

            List<KeyValuePair<int, int>> reachList = new List<KeyValuePair<int, int>>();
            List<KeyValuePair<int, int>> NextReachList = new List<KeyValuePair<int, int>>();
            reachList.Add(new KeyValuePair<int, int>(x1, y1));

            while (reachList.Count > 0)
            {
                NextReachList = new List<KeyValuePair<int, int>>();
                foreach (var pair in reachList)
                {
                    int x = pair.Key;
                    int y = pair.Value;
                    TestPointAndAddList(x - 1, y, x, y, LastPointMatrix, NextReachList);
                    TestPointAndAddList(x, y - 1, x, y, LastPointMatrix, NextReachList);
                    TestPointAndAddList(x + 1, y, x, y, LastPointMatrix, NextReachList);
                    TestPointAndAddList(x, y + 1, x, y, LastPointMatrix, NextReachList);
                }
                reachList = NextReachList;
            }
            return LastPointMatrix;
        }
        private List<KeyValuePair<int, int>> GetPath(int x1, int y1, int x2, int y2)
        {
            List<KeyValuePair<int, int>> result = new List<KeyValuePair<int, int>>();
            Ball[,] LastPointMatrix = new Ball[size, size];

            List<KeyValuePair<int, int>> reachList = new List<KeyValuePair<int, int>>();
            List<KeyValuePair<int, int>> NextReachList = new List<KeyValuePair<int, int>>();
            reachList.Add(new KeyValuePair<int, int>(x1, y1));

            while (reachList.Count > 0)
            {
                NextReachList = new List<KeyValuePair<int, int>>();
                foreach (var pair in reachList)
                {
                    int x = pair.Key;
                    int y = pair.Value;
                    TestPointAndAddList(x - 1, y    , x, y, LastPointMatrix, NextReachList);
                    TestPointAndAddList(x    , y - 1, x, y, LastPointMatrix, NextReachList);
                    TestPointAndAddList(x + 1, y    , x, y, LastPointMatrix, NextReachList);
                    TestPointAndAddList(x    , y + 1, x, y, LastPointMatrix, NextReachList);
                }
                reachList = NextReachList;
            }
            if(LastPointMatrix[x2, y2] == null)
            {
                return result;
            }
            else
            {
                Ball p = new Ball(x2, y2);
                while (p != null)
                {
                    result.Add(new KeyValuePair<int, int>(p.x, p.y));
                    p = LastPointMatrix[p.x, p.y];
                }
                result.Reverse();
                return result;
            }
        }
        private void TestPointAndAddList(int x, int y,int lastx, int lasty, Ball[,] LastPointMatrix, List<KeyValuePair<int, int>> NextReachList)
        {
            if(x >=0 && x < size && y >=0 && y < size && LastPointMatrix[x,y] == null && matrix[x,y].present != PresentType.BIG)
            {
                LastPointMatrix[x, y] = new Ball(lastx, lasty);
                NextReachList.Add(new KeyValuePair<int, int>(x, y));
            }
        } 
    }

    //Helper classes and functions
    class Animation
    {
        public AnimationType animationType;
        public DateTime startDate;
        public List<KeyValuePair<int, int>> movingPath;
        public List<KeyValuePair<int, int>> growList;
        public List<KeyValuePair<int, int>> collapseList;
        public Action callBack;
        public int exposionX;
        public int exposionY;
    }

    [Serializable]
    class Ball
    {
        public int x;
        public int y;
        public PresentType present;
        public int type;
        public Ball(int x, int y)
        {
            present = PresentType.NONE;
            type = 0;
            this.x = x;
            this.y = y;
        }
    }

    enum PresentType
    {
        NONE,
        TINY,
        BIG
    }

    enum GameStatus
    {
        IDLE,
        ANIMATING,
        PAUSING,
        OVER
    }

    enum AnimationType
    {
        MOVING,
        COLLAPSING,
        GROWING,
        EXPOSION
    }
}

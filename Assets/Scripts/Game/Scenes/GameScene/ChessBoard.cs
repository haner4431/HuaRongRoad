using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DigitalHuarongRoad.Share;
using LFrame.Core;
using LFrame.Core.Event;
using LFrame.Core.Tools;
using LFrame.Core.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DigitalHuarongRoad
{
    public class ChessBoard : MonoBehaviour
    {
        private static ChessBoard instance;

        public static ChessBoard Instance
        {
            get
            {
                return instance;
            }
        }

        #region achievement

        /// <summary>
        /// 关卡成就
        /// </summary>
        private enum LevelAchievementType
        {
            PassRoad, //通关
            LimitStep, //规定步数内通关
            LimitTime, //规定时间内通关
            Max
        }

        public int LimitStep
        {
            get
            {
                return level + 5;
            }
        }

        private int stepCounter;


        public float LimitTime
        {
            get
            {
                return level * 10;
            }
        }

        private float timeCounter;

        #endregion

        #region observer field

        public event Action<GameObject> OnClicked;

        #endregion


        private Dictionary<int, Texture2D> allNumTex;

        public Dictionary<int, Texture2D> AllNumTex
        {
            get
            {
                return allNumTex;
            }
        }

        private string NumPrefabPath
        {
            get { return "Prefabs/Num"; }
        }

        [SerializeField] private Transform[] numMapTrans;
        private GameObject numPrefab;
        private Dictionary<int, PawnBehaviour> allChess;

        private List<StateNode> tipsStatePath;
        private TipsRenderer continuousRenderer;

        public int[,] initRoad = { { 6, 4, 7 }, { 8, 5, 9 }, { 3, 2, 1 } };

        public int[,] correctRoad = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
        private int n = 3;
        [NonSerialized] public int level = 1;
        [SerializeField] private Animator boxAnimator;
        [SerializeField] private GameObject portalEffect;
        [SerializeField] private GameObject chessPanel;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            for (int i = 0; i < numMapTrans.Length; i++)
            {
                Helper.SetActiveState(numMapTrans[i], false);
            }

            Helper.SetActiveState(portalEffect, false);
            boxAnimator = Helper.GetComponent<Animator>(boxAnimator);

            numPrefab = Resources.Load<GameObject>(NumPrefabPath);
            allChess = new Dictionary<int, PawnBehaviour>();
            allNumTex = new Dictionary<int, Texture2D>();
            tipsStatePath = new List<StateNode>(1000);
            continuousRenderer = GetComponentInChildren<TipsRenderer>();
            initRotation = transform.rotation;
            ReadTex();
            instance = this;
        }

        public void NextLevel()
        {
            if (this.level + 1 <= 31)
            {
                StartGame(this.level + 1);
            }
        }

        public void StartGame(int level)
        {
            this.level = level;
            stepCounter = 0;
            timeCounter = 0;
            Pitch = 0;
            Yaw = 0;
            transform.rotation = initRotation;
            if (CameraCtrl.Instance != null)
            {
                CameraCtrl.Instance.MoveTrans(1);
            }

            EvtManager.Notify(GameEvents.StepChange, stepCounter);
            EvtManager.Notify(GameEvents.TimeChange, timeCounter);
            boxAnimator.CrossFade("BoxIdle", 0.1f);
            Helper.SetActiveState(portalEffect, false);
            Helper.SetActiveState(chessPanel, true);

            initRoad = GetStateByDepth(level).state;
            RenderHuaRoad();
            HighlightCorrectPawn();
            UIManager.Instance.OpenWindow(GameView.UI_GamePanel, this);
            playing = true;
        }

        public void Gameover(bool isPass)
        {
            if (gameoverCR != null)
            {
                Loom.StopCR(gameoverCR);
                gameoverCR = null;
            }

            gameoverCR = Loom.StartCR(Gameover_CR(isPass));
        }

        private Coroutine gameoverCR;

        private IEnumerator Gameover_CR(bool isPass)
        {
            playing = false;
            UIManager.Instance.CloseWindow(GameView.UI_GamePanel);
            if (CameraCtrl.Instance != null)
            {
                CameraCtrl.Instance.MoveTrans(1);
            }

            tipsStatePath.Clear();
            int len = allChess.Count;
            for (int i = 0; i < len; i++)
            {
                allChess.ElementAt(i).Value.UnInitialise();
            }

            Helper.SetActiveState(chessPanel, false);
            if (isPass)
            {
                boxAnimator.CrossFade("BoxOpen", 0.1f);
                yield return new WaitForSeconds(1);
                Helper.SetActiveState(portalEffect, true);
                yield return new WaitForSeconds(1.5f);
                int[] achivements = new int[(int)LevelAchievementType.Max];
                achivements[(int)LevelAchievementType.PassRoad] = 1;
                achivements[(int)LevelAchievementType.LimitStep] = stepCounter <= LimitStep ? 1 : 0;
                achivements[(int)LevelAchievementType.LimitTime] = timeCounter <= LimitTime ? 1 : 0;
                LevelInfoManager.Instance.SaveLevelInfo(level, achivements);
            }

            UIManager.OpenWD(GameView.UI_Gameover, this);

            gameoverCR = null;
        }

        public void ReStart()
        {
            tipsStatePath.Clear();
            int len = allChess.Count;
            for (int i = 0; i < len; i++)
            {
                allChess.ElementAt(i).Value.UnInitialise();
            }

            StartGame(level);
        }

        private bool adjustView;

        private void Update()
        {
            if (playing == false) return;
            timeCounter += Time.deltaTime;
            EvtManager.Notify(GameEvents.TimeChange, timeCounter);
            AdjustViewAngle();
            ClickListener();
        }

        private void LateUpdate()
        {
            adjustView = false;
        }

        private StateNode GetStateByDepth(int level)
        {
            int index = Random.Range(0, Navigation.Instance.QueryByDepth[level].Count);
            return Navigation.Instance.QueryByDepth[level][index];
        }

        /// <summary>
        /// 渲染强调正确排列的数字
        /// </summary>
        private void HighlightCorrectPawn()
        {
            if (highlightCorrectPawnCR != null)
            {
                Loom.StopCR(highlightCorrectPawnCR);
                highlightCorrectPawnCR = null;
            }
            highlightCorrectPawnCR = Loom.StartCR(HighlightCorrectPawn_CR());
        }

        private Coroutine highlightCorrectPawnCR;
        private IEnumerator HighlightCorrectPawn_CR()
        {
            yield return new WaitUntil(() => ContainsMovePawn() == false);
            var road = GetCurRoad();
            List<Vector3> renderNums = new List<Vector3>(9);
            int num = 1;
            for (int i = 1; i < 10; i++)
            {
                if ((GetCorrectIndexByNum(allChess[i].Number).x == allChess[i].CurIndex.x) &&
                    (GetCorrectIndexByNum(allChess[i].Number).y == allChess[i].CurIndex.y))
                {
                    renderNums.Add(allChess[i].transform.position);
                }
            }

            continuousRenderer.Render(renderNums.ToArray());

            highlightCorrectPawnCR = null;
        }

        /// <summary>
        /// 获取游戏中当前华容道数字的排列
        /// </summary>
        private int[,] GetCurRoad()
        {
            if (allChess != null || allChess.Count != 9)
            {
                int[,] result = new int[3, 3];
                for (int i = 0; i < allChess.Count; i++)
                {
                    result[allChess.ElementAt(i).Value.CurIndex.x, allChess.ElementAt(i).Value.CurIndex.y] =
                        allChess.ElementAt(i).Value.Number;
                }

                return result;
            }

            return null;
        }

        #region 提示相关

        /// <summary>
        /// 获取提示路径结点
        /// </summary>
        public void Tips()
        {
            if (tipsStatePath == null) return;
            if (CameraCtrl.Instance != null)
            {
                CameraCtrl.Instance.MoveTrans(1);
            }
            tipsStatePath.Clear();
            int[,] curRoad = new int[n, n];
            for (int i = 0; i < allChess.Count; i++)
            {
                curRoad[allChess.ElementAt(i).Value.CurIndex.x, allChess.ElementAt(i).Value.CurIndex.y] =
                    allChess.ElementAt(i).Value.Number;
            }

            var path = Navigation.FindPath(curRoad, allChess[9].CurIndex);
            if (path != null && path.Count > 1)
            {
                tipsStatePath = path;
                tipsStatePath.RemoveAt(0); //路径包括起点，方便后续处理已移除
                HighLightChess(GetChessByCurIndex(tipsStatePath.First().origin));
            }
            else
            {
                Helper.Log("============未找到路径");
            }
        }

        //处理提示信息
        private void HandleTips(PawnBehaviour curClickedChess)
        {
            if (tipsStatePath != null && tipsStatePath.Count != 0)
            {
                if (curClickedChess.CurIndex.Equals(tipsStatePath.First().origin))
                {
                    tipsStatePath.RemoveAt(0);
                    if (tipsStatePath.Count != 0)
                    {
                        HighLightChess(GetChessByCurIndex(tipsStatePath.First().origin));
                    }
                }
                else
                {
                    SotpHandleTips();
                }
            }
        }

        public void SotpHandleTips()
        {
            tipsStatePath.Clear();
            for (int i = 0; i < allChess.Count; i++)
            {
                allChess.ElementAt(i).Value.ChangeColor(Color.white);
            }
        }

        #endregion


        private void ReadTex()
        {
            var allTex = Resources.LoadAll<Texture2D>("Materials/Num/NumTex");
            for (int i = 0; i < allTex.Length; i++)
            {
                allNumTex.Add(int.Parse(allTex[i].name), allTex[i]);
            }
        }

        #region 棋子操作

        /// <summary>
        /// 判断是否有棋子正在移动
        /// </summary>
        /// <returns></returns>
        private bool ContainsMovePawn()
        {
            for (int i = 1; i < 10; i++)
            {
                if (allChess[i].IsMove) return true;
            }

            return false;
        }

        /// <summary>
        /// 9号位移动
        /// </summary>
        /// <param name="destination">目标位置</param>
        private void MoveNext(PawnBehaviour destination)
        {
            if (Vector2Int.Distance(allChess[9].CurIndex, destination.CurIndex) != 1) return;
            if (allChess[9].IsMove || destination.IsMove) return;
            HandleTips(destination);
            SwapChess(allChess[9], destination);
            allChess[9].MoveTo(destination.transform);
            destination.MoveTo(allChess[9].transform);
            HighlightCorrectPawn();
            stepCounter++;
            EvtManager.Notify(GameEvents.StepChange, stepCounter);
            if (JudgeWin())
            {
                Gameover(true);
                Debug.Log("Win");
            }
        }

        //选取指定坐标对应的数组的值和9（空位）交换，如果能交换的话
        public void SwapChess(PawnBehaviour chess1, PawnBehaviour chess2)
        {
            Vector2Int temp = chess1.CurIndex;
            chess1.CurIndex = chess2.CurIndex;
            chess2.CurIndex = temp;
        }

        public void HighLightChess(PawnBehaviour Chess)
        {
            Chess.ChangeColor(Color.red);
        }

        /// <summary>
        /// 获取数字相邻的其他数字
        /// </summary>
        /// <returns></returns>
        public List<PawnBehaviour> GetSibling(int num)
        {
            PawnBehaviour curChess = GetChessByNum(num);
            if (curChess != null)
            {
                List<PawnBehaviour> result = new List<PawnBehaviour>(4);
                if (curChess.CurIndex.x + 1 >= 0 && curChess.CurIndex.x + 1 < 3)
                {
                    Vector2Int curIndex = new Vector2Int(curChess.CurIndex.x + 1, curChess.CurIndex.y);
                    result.Add(GetChessByCurIndex(curIndex));
                }

                if (curChess.CurIndex.x - 1 >= 0 && curChess.CurIndex.x - 1 < 3)
                {
                    Vector2Int curIndex = new Vector2Int(curChess.CurIndex.x - 1, curChess.CurIndex.y);
                    result.Add(GetChessByCurIndex(curIndex));
                }

                if (curChess.CurIndex.y + 1 >= 0 && curChess.CurIndex.y + 1 < 3)
                {
                    Vector2Int curIndex = new Vector2Int(curChess.CurIndex.x, curChess.CurIndex.y + 1);
                    result.Add(GetChessByCurIndex(curIndex));
                }

                if (curChess.CurIndex.y - 1 >= 0 && curChess.CurIndex.y - 1 < 3)
                {
                    Vector2Int curIndex = new Vector2Int(curChess.CurIndex.x, curChess.CurIndex.y - 1);
                    result.Add(GetChessByCurIndex(curIndex));
                }

                return result;
            }

            return null;
        }

        public Vector2Int GetCorrectIndexByNum(int num)
        {
            // Vector2Int index =Vector2Int.one * -1;
            // if (num < 1 || num > (n * n ))
            // {
            //     return index; 
            // }
            //
            // index.x = num / n;
            // index.y = num % n - 1;
            // return index;

            Vector2Int index = Vector2Int.one * -1;
            if (num < 1 || num > (n * n))
            {
                return index;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (correctRoad[i, j] == num)
                    {
                        index.x = i;
                        index.y = j;
                        break;
                        ;
                    }
                }
            }

            return index;
        }

        public Transform GetCorrectTransByNum(int num)
        {
            if (num > 0 && num <= 9)
            {
                return numMapTrans[num - 1];
            }

            return null;
        }

        public Vector2Int GetCurIndexByNum(int num)
        {
            Vector2Int index = Vector2Int.one * -1;
            if (num < 1 || num > (n * n))
            {
                return index;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (initRoad[i, j] == num)
                    {
                        index.x = i;
                        index.y = j;
                        break;
                        ;
                    }
                }
            }

            return index;
        }

        private void CreateNumChess(int num)
        {
            PawnBehaviour chess;
            if (allChess.ContainsKey(num))
            {
                chess = allChess[num];
                chess.Number = num;
            }
            else
            {
                var go = GameObject.Instantiate(numPrefab, transform);
                go.name = num.ToString();
                go.transform.rotation = GetCorrectTransByNum(num).rotation;
                chess = go.AddComponent<PawnBehaviour>();
                chess.Number = num;
                AddChessToDic(chess);
            }

            chess.Initialise();
        }

        public PawnBehaviour GetChessByCurIndex(Vector2Int index)
        {
            for (int i = 0; i < allChess.Count; i++)
            {
                if (allChess.ElementAt(i).Value.CurIndex == index)
                {
                    return allChess.ElementAt(i).Value;
                }
            }

            return null;
        }

        private PawnBehaviour GetChessByNum(int num)
        {
            if (allChess.ContainsKey(num))
            {
                return allChess[num];
            }

            return null;
        }

        public void AddChessToDic(PawnBehaviour chess)
        {
            allChess.Add(chess.Number, chess);
        }

        public void DelChessFromDic(int num)
        {
            if (allChess.ContainsKey(num))
            {
                allChess.Remove(num);
            }
        }

        #endregion


        /// <summary>
        /// 打乱顺序
        /// </summary>
        /// <param name="num">打乱次数</param>
        public void DisorderNum(int num)
        {
            List<PawnBehaviour> tempChessList;
            for (int i = 0; i < num; i++)
            {
                tempChessList = GetSibling(9);
                int index = Random.Range(0, tempChessList.Count);
                SwapChess(GetChessByNum(9), tempChessList[index]);
            }
        }

        public void RenderHuaRoad()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    CreateNumChess(initRoad[i, j]);
                }
            }
        }

        //判断索引是否有效
        public bool IsValid(Vector2Int index)
        {
            return (index.x >= 0 && index.x < n) && (index.y >= 0 && index.y < n);
        }


        RaycastHit hitInfo;
        Ray cameraRay;
        public LayerMask hitableLayer;

        private bool playing;

        //鼠标点击事件
        public void ClickListener()
        {
            cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(cameraRay, out hitInfo, 1000, hitableLayer))
                {
                    OnClick(hitInfo.transform.gameObject);
                    if (hitInfo.transform.CompareTag("ChessArea"))
                    {
                        var hitedChess = hitInfo.transform.GetComponent<PawnBehaviour>();
                        if (hitedChess == null) return;
                        hitedChess.ChangeColor(Color.white);
                        MoveNext(hitedChess);
                    }
                }
            }
        }


        private float Pitch;
        private float Yaw;
        public float adjustViewSensitivity = 0.001f;
        private Quaternion initRotation;

        private void AdjustViewAngle()
        {
            if (ContainsMovePawn()) return;
            if (CameraCtrl.Instance != null && CameraCtrl.Instance.currentViewport == 2) return;
            if (Input.GetMouseButton(0))
            {
       ;
                adjustView = true;
                Pitch -= Input.GetAxis("Mouse Y");
                Yaw -= Input.GetAxis("Mouse X");
                transform.rotation = initRotation * Quaternion.Euler(new Vector3(Pitch, Yaw, initRotation.z));
            }
        }


        public bool JudgeWin()
        {
            int[,] curRoad = GetCurRoad();
            int num = 1;
            for (int i = 0; i < curRoad.GetLength(0); i++)
            {
                for (int j = 0; j < curRoad.GetLength(1); j++)
                {
                    if (curRoad[i, j] != num++) return false;
                }
            }

            return true;
        }

        #region observer handle

        private void OnClick(GameObject gameObject)
        {
            OnClicked?.Invoke(gameObject);
        }

        #endregion
    }
}

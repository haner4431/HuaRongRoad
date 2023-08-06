using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace LFrame.Core.Tools
{
    public class StateNode
    {
        public float cost; //路径代价
        public int stateNo; // 状态编号
        public int depth; // 当前结点的深度

        public int[,] state; //当前的矩阵状态
        public Vector2Int origin; //行动原点
        public StateNode parent;
        public PriorityQueue<StateNode> sibling; //联通状态集合
    }


    /// <summary>
    /// hash集合中的hashcode唯一性比较器
    /// </summary>
    public class StateNodeEqualityComparer : IEqualityComparer<StateNode>
    {
        private StringBuilder sbStr = new StringBuilder(1000);

        public bool Equals(StateNode x, StateNode y)
        {
            if (x == null && y == null) return true;
            if (x != null && y == null) return false;
            if (x == null && y != null) return false;
            if (x.state.GetLength(0) != y.state.GetLength(0))
            {
                return false;
            }

            if (x.state.GetLength(1) != y.state.GetLength(1))
            {
                return false;
            }

            for (int i = 0; i < x.state.GetLength(0); i++)
            {
                for (int j = 0; i < x.state.GetLength(1); i++)
                {
                    if (x.state[i, j] != y.state[i, j]) return false;
                }
            }

            return true;
        }

        public int GetHashCode(StateNode obj)
        {
            if (sbStr.Length > 0)
            {
                sbStr.Remove(0, sbStr.Length);
            }

            for (int i = 0; i < obj.state.GetLength(0); i++)
            {
                for (int j = 0; j < obj.state.GetLength(1); j++)
                {
                    sbStr.AppendFormat("{0}{1}{2}", i, j, obj.state[i, j]);
                }
            }

            return sbStr.ToString().GetHashCode();
        }
    }

    public class Navigation : MonoBehaviour
    {
        private enum CalculateCostMethod
        {
            Euler,
            Straight,
            incorrectNumCount
        }


        #region logic

        private static Navigation instance;

        public static Navigation Instance
        {
            get
            {
                return instance;
            }
        }

        private int[,] correctRoad = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
        private Dictionary<int, Vector2Int> targetStateDic;
        private static int[,] targetState;
        private static HashSet<StateNode> queryTable; //用于预处理，记录所有状态
        private Dictionary<int, List<StateNode>> queryByDepth; //按深度存储的查询表

        public Dictionary<int, List<StateNode>> QueryByDepth
        {
            get
            {
                return queryByDepth;
            }
        }
        
        public int QueryTableSize
        {
            get
            {
                return queryTable.Count;
            }
        }
        private static HashSet<StateNode> close;
        private static Queue<StateNode> open;
        private CancellationTokenSource preprocessCTS;

        private StateNodeEqualityComparer equalityComparer;
        private int maxSearchDepth = 31;
        private bool isInit;

        public bool IsInit
        {
            get
            {
                return isInit;
            }
        }

        private void Start()
        {
            instance = this;
         
            StartCoroutine(Intialise_CR());
        }

        private void OnDestroy()
        {
            preprocessCTS.CancelAfter(1);
            open.Clear();
            close.Clear();
            queryTable.Clear();
            open = null;
            close = null;
            queryTable = null;
        }  

        public static void Intialise(Transform root)
        {
            root.AddComponent<Navigation>();
        }

        private IEnumerator Intialise_CR()
        {
            targetStateDic = new Dictionary<int, Vector2Int>();
            equalityComparer = new StateNodeEqualityComparer();
            targetState = correctRoad;
            open = new Queue<StateNode>();
            close = new HashSet<StateNode>(equalityComparer);
            queryTable = new HashSet<StateNode>(equalityComparer);
            queryByDepth = new Dictionary<int, List<StateNode>>();
            for (int i = 0; i < 32; i++)
            {
                queryByDepth.Add(i,new List<StateNode>(50000));
            }
            for (int i = 0; i < correctRoad.GetLength(0); i++)
            {
                for (int j = 0; j < correctRoad.GetLength(1); j++)
                {
                    targetStateDic.Add(correctRoad[i, j], new Vector2Int(i, j));
                }
            }
            preprocessCTS = new CancellationTokenSource();
            Task.Run(PreProcess, preprocessCTS.Token);
       
             yield break;
        }

        /// <summary>
        /// 预处理数据
        /// </summary>
        private void PreProcess()
        {
            int number = 0;
            var node = new StateNode();
            node.cost = 0;
            node.origin = new Vector2Int(targetState.GetLength(0) - 1, targetState.GetLength(1) - 1); 
            node.state = new int[targetState.GetLength(0), targetState.GetLength(1)];
            node.depth = 0;
            node.stateNo = number++;
            node.parent = null;
            node.sibling = null;
            Array.Copy(targetState, node.state, targetState.Length);
            open.Enqueue(node);
            while (open != null || open.Count != 0)
            {
                if (open.Count == 0) break;
                var root = open.Dequeue();
                queryTable.Add(root);
                queryByDepth[root.depth].Add(root);
                if (root.depth > maxSearchDepth)
                {
                    break;
                }
                PriorityQueue<StateNode> sibling = new PriorityQueue<StateNode>(PriorityQueueComparer.Comparer);
                int m = 1;
                for (int i = 0; i < 4; i++)
                {
                    if (i < 2)
                    {
                        if ((root.origin.x - m) >= 0 && (root.origin.x - m) < root.state.GetLength(0))
                        {
                            var newNode = new StateNode();
                            newNode.origin = new Vector2Int(root.origin.x - m, root.origin.y);
                            newNode.state = new int[root.state.GetLength(0), root.state.GetLength(1)];
                            newNode.depth = root.depth + 1;
                            newNode.parent = root;
                            Array.Copy(root.state, newNode.state, root.state.Length);
                            (newNode.state[root.origin.x, root.origin.y], newNode.state[newNode.origin.x, newNode.origin.y]) =
                                (newNode.state[newNode.origin.x, newNode.origin.y], newNode.state[root.origin.x, root.origin.y]);

                            if (close.Contains(newNode) == false)
                            {
                                newNode.stateNo = number++;
                                newNode.cost = GetCost(newNode, CalculateCostMethod.incorrectNumCount);
                                sibling.Enqueue(newNode);
                                open.Enqueue(newNode);
                            }
                            else
                            {
                                queryTable.TryGetValue(newNode, out StateNode actualNode);
                                sibling.Enqueue(actualNode);
                            }
                        }
                    }
                    else
                    {
                        if (root.origin.y - m >= 0 && root.origin.y - m < root.state.GetLength(1))
                        {
                            var newNode = new StateNode();
                            newNode.origin = new Vector2Int(root.origin.x, root.origin.y - m);
                            newNode.state = new int[root.state.GetLength(0), root.state.GetLength(1)];
                            newNode.depth = root.depth + 1;
                            newNode.parent = root;
                            Array.Copy(root.state, newNode.state, root.state.Length);
                            (newNode.state[root.origin.x, root.origin.y], newNode.state[newNode.origin.x, newNode.origin.y]) =
                                (newNode.state[newNode.origin.x, newNode.origin.y], newNode.state[root.origin.x, root.origin.y]);

                            //有效状态
                            if (close.Contains(newNode) == false)
                            {
                                newNode.stateNo = number++;
                                newNode.cost = GetCost(newNode, CalculateCostMethod.incorrectNumCount);
                                sibling.Enqueue(newNode);
                                open.Enqueue(newNode);
                            }
                            else
                            {
                                queryTable.TryGetValue(newNode, out StateNode actualNode);
                                sibling.Enqueue(actualNode);
                            }
                        }
                    }

                    m = m * -1;
                }

                root.sibling = sibling;
                close.Add(root);
            }

            queryByDepth.Remove(0);
            Loom.RunMainTrd(() =>
            {
                Helper.Log("总状态数: {0} 初始化状态", queryTable.Count, isInit);
            });
            isInit = true;
            open.Clear();
            close.Clear();
        }

        private static bool EqualsArrayValue(int[,] state, int[,] state1)
        {
            bool isEqual = true;
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    if (state[i, j] != state1[i, j])
                    {
                        isEqual = false;
                        break;
                    }
                }
            }

            return isEqual;
        }

        private Vector2Int IndexOfOriginState(int value)
        {
            if (targetStateDic.ContainsKey(value))
            {
                return targetStateDic[value];
            }

            return Vector2Int.one * -1;
        }


        private void AddToChildren(StateNode node, PriorityQueue<StateNode> children)
        {
            if (close.Contains(node) == false)
            {
                children.Enqueue(node);
            }
        }

        private static void AddToOpenTable(StateNode node)
        {
            if (close.Contains(node) == false)
            {
                open.Enqueue(node);
            }
        }

        public List<StateNode> FindWay(int[,] curState, Vector2Int origin)
        {
            if (isInit == false) return null;
            close.Clear();
            open.Clear();
            var path = new List<StateNode>(100);
            int depth = 0;
            var node = new StateNode();
            node.cost = 0;
            node.origin = origin;
            node.state = new int[targetState.GetLength(0), targetState.GetLength(1)];
            node.depth = 0;
            node.parent = null;
            node.sibling = null;
            Array.Copy(curState, node.state, targetState.Length);
            if (queryTable.TryGetValue(node, out StateNode backupNode))
            {
                open.Enqueue(backupNode);
            }
            else
            {
                Helper.LogError("当前状态库中无目前此状态");
                return null;
            }

            if (MoveToTargetStateByBFS(ref path) == false)
            {
                path.Clear();
                Helper.Log("未查找到路径");
            }

            return path;
        }

        public static List<StateNode> FindPath(int[,] curState, Vector2Int origin)
        {
            if (curState != null && curState[origin.x, origin.y] == 9)
            {
                return Instance.FindWay(curState, origin);
            }
            else
            {
                Helper.LogError("param not correct");
            }

            return null;
        }

        private static bool MoveToTargetStateByBFS(ref List<StateNode> path)
        {
            int originNo = open.Peek().stateNo;
            while (open != null || open.Count > 0)
            {
                if (open.Count == 0) return false;
                var root = open.Dequeue();
                path.Add(root);
                if (Navigation.EqualsArrayValue(root.state, targetState))
                {
                    Helper.Log("路径搜索成功,本次共搜索了{0}次", close.Count);
                    return true;
                }

                close.Add(root);
                if (root.sibling == null || root.sibling.Count == 0)
                {
                    Helper.Log("未搜索到结果");
                    return false;
                }
                else
                {
                    if (close.Contains(root.sibling.Peek()) == false)
                    {
                        AddToOpenTable(root.sibling.Peek());
                    }
                }
            }

            return false;
        }


        private bool MoveToTargetStateByDFS(StateNode root, ref List<StateNode> path)
        {
            if (root == null || root.depth > maxSearchDepth)
            {
                return false;
            }

            close.Add(root);
            path.Add(root);
            PriorityQueue<StateNode> childen = new PriorityQueue<StateNode>(PriorityQueueComparer.Comparer);
            if (Navigation.EqualsArrayValue(root.state, targetState))
            {
                Helper.Log("=================result ");
                Helper.Log("{0} {1} {2} \n ", root.state[0, 0], root.state[0, 1], root.state[0, 2]);
                Helper.Log("{0} {1} {2} \n ", root.state[1, 0], root.state[1, 1], root.state[1, 2]);
                Helper.Log("{0} {1} {2} \n ", root.state[2, 0], root.state[2, 1], root.state[2, 2]);
                return true;
            }
            else
            {
                int m = 1;
                for (int i = 0; i < 4; i++)
                {
                    if (i < 2)
                    {
                        if ((root.origin.x - m) >= 0 && (root.origin.x - m) < root.state.GetLength(0))
                        {
                            var newNode = new StateNode();
                            newNode.origin = new Vector2Int(root.origin.x - m, root.origin.y);
                            newNode.state = new int[root.state.GetLength(0), root.state.GetLength(1)];
                            newNode.depth = root.depth + 1;
                            Array.Copy(root.state, newNode.state, root.state.Length);
                            (newNode.state[root.origin.x, root.origin.y], newNode.state[newNode.origin.x, newNode.origin.y]) =
                                (newNode.state[newNode.origin.x, newNode.origin.y], newNode.state[root.origin.x, root.origin.y]);
                            newNode.cost = GetCost(newNode, CalculateCostMethod.incorrectNumCount);
                            AddToChildren(newNode, childen);
                        }
                    }
                    else
                    {
                        if (root.origin.y - m >= 0 && root.origin.y - m < root.state.GetLength(1))
                        {
                            var newNode = new StateNode();
                            newNode.origin = new Vector2Int(root.origin.x, root.origin.y - m);
                            newNode.state = new int[root.state.GetLength(0), root.state.GetLength(1)];
                            newNode.depth = root.depth + 1;
                            Array.Copy(root.state, newNode.state, root.state.Length);
                            (newNode.state[root.origin.x, root.origin.y], newNode.state[newNode.origin.x, newNode.origin.y]) =
                                (newNode.state[newNode.origin.x, newNode.origin.y], newNode.state[root.origin.x, root.origin.y]);
                            newNode.cost = GetCost(newNode, CalculateCostMethod.incorrectNumCount);
                            AddToChildren(newNode, childen);
                        }
                    }

                    m = m * -1;
                }
            }

            while (childen.Count != 0)
            {
                if (MoveToTargetStateByDFS(childen.Dequeue(), ref path))
                {
                    return true;
                }
            }

            path.Remove(root);
            return false;
        }

        private float GetCost(StateNode node, CalculateCostMethod calculateCostMethod = CalculateCostMethod.Straight)
        {
            float costSum = 0;
            switch (calculateCostMethod)
            {
                case CalculateCostMethod.Straight:
                {
                    Vector2Int curIndex;
                    for (int i = 0; i < node.state.GetLength(0); i++)
                    {
                        for (int j = 0; j < node.state.GetLength(1); j++)
                        {
                            curIndex = new Vector2Int(i, j);
                            costSum += (IndexOfOriginState(node.state[i, j]) - curIndex).magnitude;
                        }
                    }

                    break;
                }
                case CalculateCostMethod.Euler:
                {
                    Vector2Int curIndex;
                    for (int i = 0; i < node.state.GetLength(0); i++)
                    {
                        for (int j = 0; j < node.state.GetLength(1); j++)
                        {
                            curIndex = new Vector2Int(i, j);
                            costSum += Mathf.Abs((IndexOfOriginState(node.state[i, j]).x - curIndex.x) + (IndexOfOriginState(node.state[i, j]).y - curIndex.y));
                        }
                    }

                    break;
                }
                case CalculateCostMethod.incorrectNumCount:
                {
                    // for (int i = 0; i < node.state.GetLength(0); i++)
                    // {
                    //     for (int j = 0; j < node.state.GetLength(1); j++)
                    //     {
                    //         if (node.state[i, j] != targetState[i,j])
                    //         {
                    //             costSum++;
                    //         }
                    //     }
                    // }
                    break;
                }
            }

            return costSum + node.stateNo;
        }

        #endregion
    }
}

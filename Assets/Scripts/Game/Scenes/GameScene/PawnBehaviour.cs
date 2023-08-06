using System;
using DG.Tweening;
using LFrame.Core.Tools;
using UnityEngine;

namespace DigitalHuarongRoad
{
    
    /// <summary>
    /// 棋类行为类，控制每一个生成的数字棋子
    /// </summary>
    public class PawnBehaviour : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        private ChessBoard chessBoard;
        [SerializeField] private Vector2Int curIndex; //该棋子现在所在的索引位置
        private Transform targetTrans;
        private Vector3 moveTraget;
        private Tweener moveTweener = null;

        public bool IsMove;

        public Vector2Int CurIndex
        {
            get
            {
                return curIndex;
            }

            set
            {
                curIndex = value;
            }
        }
        
        /// <summary>
        /// 该棋子代表的数字
        /// </summary>
        [SerializeField]private int number;

        public int Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
            }
        }
        
        private void Awake()
        {
            chessBoard = ChessBoard.Instance;
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = Resources.Load<Material>("Materials/Num/NumColor");
            gameObject.AddComponent<NewJelly1>();
        }
        
        private void Update()
        {
            //TransformHandle();
        }
        

        private void OnDestroy()
        {
            chessBoard.DelChessFromDic(number);
        }

        public void Initialise()
        {
            curIndex = chessBoard.GetCurIndexByNum(number);
            targetTrans = chessBoard.GetCorrectTransByNum(chessBoard.correctRoad[curIndex.x,curIndex.y]);
            moveTraget = targetTrans.position;
            meshRenderer.material.mainTexture = chessBoard.AllNumTex[number];
            if (!gameObject.activeSelf)
            {
                Helper.SetActiveState(gameObject,true);
            }
            MoveTo(targetTrans);
        }
        
        public void UnInitialise()
        {
            if (gameObject.activeSelf)
            {
                Helper.SetActiveState(gameObject,false);
            }
               
        }
        

        private void TransformHandle()
        {
            if (targetTrans==null || Vector3.Distance(transform.position, moveTraget) < 0.01f)
            {
                if (targetTrans != null) targetTrans = null;
                return;
            }
            transform.position =  Vector3.Lerp(transform.position, moveTraget,0.1f);
        }

        public void MoveTo(Transform target)
        {
            IsMove = true;
            targetTrans = target;
            this.moveTraget = target.position;
            moveTweener = transform.DOMove(moveTraget, 0.5f);
            moveTweener.SetEase(Ease.OutExpo);
            moveTweener.OnComplete(() =>
            {
                moveTweener = null;
                IsMove = false;
            });
        }

        public void ChangeColor(Color color)
        {
            meshRenderer.material.SetColor("_Color",color);
        }

    }
}

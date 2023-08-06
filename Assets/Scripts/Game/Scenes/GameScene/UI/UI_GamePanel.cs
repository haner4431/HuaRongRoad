using System;
using LFrame.Core.Event;
using LFrame.Core.Scene;
using LFrame.Core.Tools;
using LFrame.Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DigitalHuarongRoad
{
    public class UI_GamePanel : View
    {
        private ChessBoard chessBoard;
        public Button B_Tips;
        public Button B_Options;
        public Text T_Time;
        public Text T_Step;

        private void Awake()
        {
            InitialComponent();
            EvtManager.RegEvt(GameEvents.StepChange, evt =>
            {
                T_Step.text = Helper.StringFormat("已经消耗步数：{0}步", EvtManager.DecodeEvt<int>(evt));
            } );
         
            EvtManager.RegEvt(GameEvents.TimeChange, evt =>
            {
                T_Time.text = Helper.StringFormat("计时：{0}s",   Math.Round(EvtManager.DecodeEvt<float>(evt),0));
            } );

        }

        public override void OnOpen(params object[] args)
        {
            base.OnOpen(args);
            chessBoard = args[0] as ChessBoard;
        }


        private void InitialComponent()
        {
            B_Tips = Helper.GetComponent<Button>(B_Tips);
            B_Options = Helper.GetComponent<Button>(B_Options);
            T_Step = Helper.GetComponent<Text>(T_Step);
            T_Time = Helper.GetComponent<Text>(T_Time);
            B_Tips.onClick.AddListener(() =>
            {
                if (chessBoard != null)
                    chessBoard.Tips();
                if (CameraCtrl.Instance != null)
                {
                    CameraCtrl.Instance.MoveTrans(1);
                }
            });

         
            
            B_Options.onClick.AddListener(() =>
            {
                OpenWD(GameView.UI_GameOptions,chessBoard);
            });
            
            
        }


    }
}

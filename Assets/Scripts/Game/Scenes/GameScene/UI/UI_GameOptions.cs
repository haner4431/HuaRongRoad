using System;
using LFrame.Core.Scene;
using LFrame.Core.Tools;
using LFrame.Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DigitalHuarongRoad
{
    public class UI_GameOptions: ZoomedView
    {
        private ChessBoard chessBoard;
        public Button B_Return;
        public Button B_ReStart;
        public Button B_SelectLevel;
        public Button B_ReturnInit;
        public Button B_Setting;

        private void Awake()
        {
            B_Return = Helper.GetComponent<Button>(B_Return);
            B_ReStart = Helper.GetComponent<Button>(B_ReStart);
            B_ReturnInit = Helper.GetComponent<Button>(B_ReturnInit);
            B_SelectLevel = Helper.GetComponent<Button>(B_SelectLevel);
            B_Setting = Helper.GetComponent<Button>(B_Setting);
            
            B_Return.onClick.AddListener(() =>
            {
                CloseWD();
            });
            
            B_ReStart.onClick.AddListener(() =>
            {
                CloseWD(()=>chessBoard.ReStart());
                
            }); 
            
            B_ReturnInit.onClick.AddListener(() =>
            {
                CloseWD(()=> SceneManager.GotoScene("Init"));
              
            });  
            
            B_SelectLevel.onClick.AddListener(() =>
            {
                CloseWD(()=>
                {
                    SceneManager.GotoScene("Init", true);
                });
              
            });

            B_Setting.onClick.AddListener(() =>
            {
                OpenWD(ShareView.UI_Setting);
          
            });
        }

        public override void OnOpen(params object[] args)
        {
            base.OnOpen(args);
            if (chessBoard == null &&args.Length > 0  )
            {
                chessBoard = args[0] as ChessBoard;
                
            }
           

        }

    
    }
}

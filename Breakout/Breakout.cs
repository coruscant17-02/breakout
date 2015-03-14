using DxLibDLL;
using System;

namespace Test
{
    class TestApplication
    {
        ///<summary>
        ///アプリケーションのエントリーポイント（ここからプログラムが始まる）
        ///</summary>
        [STAThread]
        static void Main()
        {
            // 画面モードの設定
            DX.ChangeWindowMode(DX.TRUE);
            DX.SetGraphMode(640, 480, 16);

            if (DX.DxLib_Init() == -1)
            {
                return;
            }

            DX.LoadGraphScreen(0, 0, "ball.png", DX.TRUE);
            DX.LoadGraphScreen(50, 0, "wall.png", DX.TRUE);

            DX.SetDrawScreen(DX.DX_SCREEN_BACK);

            int BallGHandle = DX.LoadGraph("ball.png");
            int WallGHandle = DX.LoadGraph("wall.png");

            //getGraphicSize
            int BallGSizeX;
            int BallGSizeY;
            DX.GetGraphSize(BallGHandle, out BallGSizeX, out BallGSizeY);

            //getGraphicSize
            int WallGSizeX;
            int WallGSizeY;
            DX.GetGraphSize(WallGHandle, out WallGSizeX, out WallGSizeY);

            //BallPosition
            int BallX = 100;
            int BallY = 50;
            //BallSpeed
            int BallVX = 10;
            int BallVY = 5;

            //ボールの中心
            double BallCenterX;
            double BallCenterY;

            int WallX = 300;
            int WallY = 200;

            //ブロックの中心
            double WallCenterX;
            double WallCenterY;

            //MainLoop
            while (DX.ProcessMessage() == 0 && DX.CheckHitKeyAll() == 0)
            {
                DX.ClearDrawScreen();

                //ボールと枠の衝突判定(速度にマイナス1をかけることで向きを逆にする)
                //この場合ボールはX座標100～500,Y座標50～400の範囲を移動することになります。
                if (BallX + BallGSizeX >= 500) BallVX *= -1;
                if (BallX <= 90) BallVX *= -1;
                if (BallY + BallGSizeY >= 400) BallVY *= -1;
                if (BallY <= 40) BallVY *= -1;

                //calcCenterPositionOfBall&Wall
                BallCenterX = BallX + BallGSizeX / 2;
                BallCenterY = BallY + BallGSizeY / 2;

                WallCenterX = WallX + WallGSizeX / 2;
                WallCenterY = WallY + WallGSizeY / 2;

                if (Math.Abs(BallCenterX - WallCenterX) <= (BallGSizeX + WallGSizeX) / 2)
                {
                    //fromUnder
                    if ((BallY - BallVY >= WallY + WallGSizeY) && (BallY <= WallY + WallGSizeY))
                    {
                        BallVY *= -1;
                        BallY = (WallY + WallGSizeY) * 2 - BallY;
                    }
                    //fromUP
                    if ((BallY - BallVY + BallGSizeY <= WallY) && (BallY + BallGSizeY >= WallY))
                    {
                        BallVY *= -1;
                        BallY = (WallY - BallGSizeY) * 2 - BallY;
                    }
                    //piledUpToSide
                    if (Math.Abs(BallCenterY - WallCenterY) <= (BallGSizeY + WallGSizeY) / 2)
                    {
                        //fromLeft
                        if ((BallX - BallVX >= WallX + WallGSizeX) && (BallX <= WallX + WallGSizeX))
                        {
                            BallVX *= -1;
                            BallX = (WallX + WallGSizeX) * 2 - BallX;
                        }
                        //fromRight
                        if ((BallX - BallVX + BallGSizeX <= WallX) && (BallX + BallGSizeX >= WallX))
                        {
                            BallVX *= -1;
                            BallX = (WallX - BallGSizeX) * 2 - BallX;
                        }

                    }
                }

                // ボールを移動させる(＝表示させる座標を変更する)
                DX.DrawGraph(BallX, BallY, BallGHandle, DX.TRUE);
                DX.DrawGraph(WallX, WallY, WallGHandle, DX.TRUE);

                BallX += BallVX;
                BallY += BallVY;

                DX.ScreenFlip();

                DX.WaitTimer(20);

            }

            DX.WaitKey();

            DX.DxLib_End();

            return;
        }
    }
}
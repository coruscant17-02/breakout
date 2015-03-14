using DxLibDLL;
using System;

namespace Test
{
    class TestApplication
    {
        public const int WALL_NUM = 5;

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
            //int WallGHandle = DX.LoadGraph("wall.png");

            //getGraphicSize
            int BallGSizeX;
            int BallGSizeY;
            DX.GetGraphSize(BallGHandle, out BallGSizeX, out BallGSizeY);

            //import PNGfile to memory
            int[] WallGHandle = new int[WALL_NUM];

            //getGraphicSize
            int[] WallGSizeX = new int[WALL_NUM];
            int[] WallGSizeY = new int[WALL_NUM];
            for (int i = 0; i < WALL_NUM; i++)
            {
                WallGHandle[i] = DX.LoadGraph("wall.png");
                DX.GetGraphSize(WallGHandle[i], out WallGSizeX[i], out WallGSizeY[i]);   
            }
            /*-----------------------------------------------------------------------------*/
            //BallPosition
            int BallX = 300;
            int BallY = 250;

            //BallSpeed
            int BallVX = 10;
            int BallVY = 5;

            //CenterOfBall
            double BallCenterX;
            double BallCenterY;

            /*-----------------------------------------------------------------------------*/
            //BlockPosition
            int[] WallX = new int[] { 150, 210, 270, 330, 390 };
            int[] WallY = new int[] { 100, 160, 80, 110, 100 };
            int[] WallHP = new int[] { 3, 3, 3, 3, 3 };
            int[] WallHitFlag = new int[] { 0, 0, 0, 0, 0 };

            //CenterOfBlock
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

                //ブロックの数だけループする
                for (int i = 0; i < WALL_NUM; i++)
                {
                    //HPが１以上なら
                    if (WallHP[i] > 0)
                    {
                        WallCenterX = WallX[i] + WallGSizeX[i] / 2;
                        WallCenterY = WallY[i] + WallGSizeY[i] / 2;

                        if (Math.Abs(BallCenterX - WallCenterX) <= (BallGSizeX + WallGSizeX[i]) / 2)
                        {
                            //fromUnder
                            if ((BallY - BallVY >= WallY[i] + WallGSizeY[i]) && (BallY <= WallY[i] + WallGSizeY[i]))
                            {
                                BallVY *= -1;
                                BallY = (WallY[i] + WallGSizeY[i]) * 2 - BallY;
                                WallHitFlag[i] = DX.TRUE;
                            }
                            //fromUP
                            if ((BallY - BallVY + BallGSizeY <= WallY[i]) && (BallY + BallGSizeY >= WallY[i]))
                            {
                                BallVY *= -1;
                                BallY = (WallY[i] - BallGSizeY) * 2 - BallY;
                                WallHitFlag[i] = DX.TRUE;
                            }
                            //piledUpToSide
                            if (Math.Abs(BallCenterY - WallCenterY) <= (BallGSizeY + WallGSizeY[i]) / 2)
                            {
                                //fromLeft
                                if ((BallX - BallVX >= WallX[i] + WallGSizeX[i]) && (BallX <= WallX[i] + WallGSizeX[i]))
                                {
                                    BallVX *= -1;
                                    BallX = (WallX[i] + WallGSizeX[i]) * 2 - BallX;
                                    WallHitFlag[i] = DX.TRUE;
                                }
                                //fromRight
                                if ((BallX - BallVX + BallGSizeX <= WallX[i]) && (BallX + BallGSizeX >= WallX[i]))
                                {
                                    BallVX *= -1;
                                    BallX = (WallX[i] - BallGSizeX) * 2 - BallX;
                                    WallHitFlag[i] = DX.TRUE;
                                }
                            }

                            //判定で当たったと判断
                            if (WallHitFlag[i] == DX.TRUE)
                            {
                                WallHP[i]--;
                                WallHitFlag[i] = DX.FALSE;
                            }
                        }   
                    }
                }

                
                DX.DrawGraph(BallX, BallY, BallGHandle, DX.TRUE);
                DX.clsDx();
                for (int i = 0; i < WALL_NUM; i++)
                {
                    if (WallHP[i]>0)
                    {
                        DX.DrawGraph(WallX[i], WallY[i], WallGHandle[i], DX.TRUE);      
                    }
                }

                // ボールを移動させる(＝表示させる座標を変更する)
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
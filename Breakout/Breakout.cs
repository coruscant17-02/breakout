using DxLibDLL;
using System;

namespace Test
{
    class TestApplication
    {
        public const int WALL_NUM = 5;
        public const int STAGE_SIZEX1 = 100;
        public const int STAGE_SIZEY1 = 50;
        public const int STAGE_SIZEX2 = 500;
        public const int STAGE_SIZEY2 = 500;

        ///<summary>
        ///アプリケーションのエントリーポイント（ここからプログラムが始まる）
        ///</summary>
        [STAThread]
        static void Main()
        {
            // 画面モードの設定
            DX.ChangeWindowMode(DX.TRUE);
            DX.SetGraphMode(800, 600, 32);

            /*-----------------------------------------------------------------------------*/

            //BallSetting
            int BallGHandle;
            int BallGSizeX;
            int BallGSizeY;
            //BallPosition
            int BallX = 100;
            int BallY = 50;
            //BallSpeed
            int BallVX = 3;
            int BallVY = 3;
            //CenterOfBall
            double BallCenterX;
            double BallCenterY;

            int BallStopFlag = 1;


            /*-----------------------------------------------------------------------------*/
            //WallSetting
            int[] WallGHandle = new int[WALL_NUM];
            int[] WallGSizeX = new int[WALL_NUM];
            int[] WallGSizeY = new int[WALL_NUM];
            //BlockPosition
            int[] WallX = new int[] { 150, 210, 270, 330, 390 };
            int[] WallY = new int[] { 100, 160, 80, 110, 100 };
            int[] WallHP = new int[] { 3, 3, 3, 3, 3 };
            int[] WallHitFlag = new int[] { 0, 0, 0, 0, 0 };
            //CenterOfBlock
            double WallCenterX;
            double WallCenterY;

            int WallHPSum;
            /*-----------------------------------------------------------------------------*/
            //BarSetting
            int BarGHandle;
            int BarGSizeX;
            int BarGSizeY;
            int BarX = 150;
            int BarY = 400;
            int BarVX = 2;
            int BarVY = 2;
            int BarHitFlag;

            //CenterOfBar
            double BarCenterX;
            double BarCenterY;

            /*-----------------------------------------------------------------------------*/
            //Score
            int MissCounter = 0;
            int GameOverFlag = 0;
            int GameClearFlag = 0;

            if (DX.DxLib_Init() == -1)
            {
                return;
            }

            DX.SetDrawScreen(DX.DX_SCREEN_BACK);

            BallGHandle = DX.LoadGraph("ball.png");
            BarGHandle = DX.LoadGraph("bar.png");

            DX.GetGraphSize(BallGHandle, out BallGSizeX, out BallGSizeY);
            DX.GetGraphSize(BarGHandle, out BarGSizeX, out BarGSizeY);

            for (int i = 0; i < WALL_NUM; i++)
            {
                WallGHandle[i] = DX.LoadGraph("wall.png");
                DX.GetGraphSize(WallGHandle[i], out WallGSizeX[i], out WallGSizeY[i]);
            }
            
/*-----------------------------------------------------------------------------------------*/
            //MainLoop
            while (DX.ProcessMessage() == 0)
            {
                DX.ClearDrawScreen();

                //ボールの移動
                if (BallStopFlag !=1)
                {
                    BallX += BallVX;
                    BallY += BallVY;
                }else{
                    BallX = (BarX * 2 + BarGSizeX - BallGSizeX) / 2;
                    BallY = BarY - BallGSizeY;
                }

                //ボールと外枠の衝突判定
                //右の枠
                if (((BallX - BallVX + BallGSizeX <= STAGE_SIZEX2) && (BallX + BallGSizeX >= STAGE_SIZEX2)) || (BallX + BallGSizeX >= STAGE_SIZEX2))
                {
                    BallVX *= -1;
                    BallX = (STAGE_SIZEX2 - BallGSizeX) * 2 - BallX;
                }
                //左の枠
                if (((BallX <= STAGE_SIZEX1) && (BallX - BallVX >= STAGE_SIZEX1)) || (BallX <= STAGE_SIZEX1))
                {
                    BallVX *= -1;
                    BallX = STAGE_SIZEX1 * 2 - BallX;
                }
                //下の枠に当たるとミスがカウントされる
                if (((BallY - BallVY + BallGSizeY <= STAGE_SIZEY2) && (BallY + BallGSizeY >= STAGE_SIZEY2)) || (BallY + BallGSizeY >= STAGE_SIZEY2))
                {
                    MissCounter++;	//ミス回数を１増やす
                    BallStopFlag = DX.TRUE;	//ボールを一度止める
                    BallX = (BarX * 2 + BarGSizeX - BallGSizeX) / 2;	//バーの中心に乗っける
                    BallY = BarY - BallGSizeY;

                }
                //上の枠
                if (((BallY <= STAGE_SIZEY1) && (BallY - BallVY >= STAGE_SIZEY1)) || (BallY <= STAGE_SIZEY1))
                {
                    BallVY *= -1;
                    BallY = STAGE_SIZEY1 * 2 - BallY;
                }

                //ボールと壁の衝突判定
                BallCenterX = BallX + BallGSizeX / 2;
                BallCenterY = BallY + BallGSizeY / 2;

                /*-----------------------*/
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

                    //バーの移動(キー入力)
                    if (DX.CheckHitKey(DX.KEY_INPUT_LEFT) == 1)
                    {
                        if (BarX - BarVX <= STAGE_SIZEX1)
                        {
                            BarX = STAGE_SIZEX1;
                        }else{
                            BarX -= BarVX;
                        }
                    }
                    if (DX.CheckHitKey(DX.KEY_INPUT_RIGHT) == 1)
                    {
                        if (BarX + BarGSizeX + BarVX >= STAGE_SIZEX2)
                        {
                            BarX = STAGE_SIZEX2 - BarGSizeX;
                        }else{
                            BarX += BarVX;
                        }
                    }
//koko
                    if ((BallStopFlag==1)&&(DX.CheckHitKey(DX.KEY_INPUT_SPACE)==1))
                    {
                        BallStopFlag = DX.FALSE;
                    }

                    //ボールとバーの衝突判定
                    BarCenterX = BarX + BarGSizeX / 2;
                    BarCenterY = BarY + BarGSizeY / 2;

                    if (Math.Abs(BallCenterX - BarCenterX) <= (BallGSizeX + BarGSizeY) / 2)
                    {
                        if ((BallY - BallVY >= BarY + BarGSizeY)&&(BallY <= BarY + BarGSizeY))
                        {
                            BallVY *= -1;
                            BallY = BarY + BarGSizeY;
                            BarHitFlag = DX.TRUE;
                        }
                        if ((BallY - BallVY + BallGSizeY <= BarY)&&(BallY + BallGSizeY >= BarY))
                        {
                            BallVY *= -1;
                            BallY = BarY - BallGSizeY;
                            BarHitFlag = DX.TRUE;
                        }
                    }

                    if (Math.Abs(BallCenterY - BarCenterY) <= (BallGSizeY + BarGSizeY) / 2)
                    {
                        if ((BallX - BallVX >= BarX + BarGSizeX)&&(BallX <= BarX + BarGSizeX))
                        {
                            BallVX *= -1;
                            BallX = BarX + BarGSizeX;
                            BarHitFlag = DX.TRUE;
                        }
                        if ((BallX - BallVX + BallGSizeX <= BarX )&&(BallX + BallGSizeX >= BarX))
                        {
                            BallVX *= -1 ;
                            BallX = BarX - BallGSizeX ;
                            BarHitFlag = DX.TRUE;
                        }
                    }
                }
                /*-----------------------*/
                //ゲームオーバー判定(１０回ミスしたらゲームオーバー)
                if (MissCounter >= 10)
                {
                    GameOverFlag = DX.TRUE;
                    BallVX = 0;
                    BallVY = 0;
                }
                //ゲームクリア判定
                WallHPSum = 0;
                for (int i = 0; i < WALL_NUM; i++)
                {
                    WallHPSum += WallHP[i];
                }
                if (WallHPSum == 0)
                {
                    GameClearFlag = DX.TRUE;
                    BallVX = 0;
                    BallVY = 0;
                }

                //外枠の表示
                DX.DrawBox(STAGE_SIZEX1, STAGE_SIZEY1, STAGE_SIZEX2, STAGE_SIZEY2, DX.GetColor(255, 255, 255), DX.FALSE);
                //ボールの表示
                DX.DrawGraph(BallX, BallY, BallGHandle, DX.TRUE);
                //バーの表示
                DX.DrawGraph(BarX, BarY, BarGHandle, DX.TRUE);

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

                // ＥＳＣキーが押されたらループから抜ける
                if (DX.CheckHitKey(DX.KEY_INPUT_ESCAPE) == 1) break;
            }
/*-----------------------------------------------------------------------------------------*/
            DX.DxLib_End();

            return;
        }
    }
}
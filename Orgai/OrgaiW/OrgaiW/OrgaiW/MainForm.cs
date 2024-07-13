/*
    The MIT License

    Copyright(c) 2024 OrgaiCom

    以下に定める条件に従い、本ソフトウェアおよび関連文書のファイル（以下「ソフトウェア」）
    の複製を取得するすべての人に対し、ソフトウェアを無制限に扱うことを無償で許可します。
    これには、ソフトウェアの複製を使用、複写、変更、結合、掲載、頒布、サブライセンス、
    および/または販売する権利、およびソフトウェアを提供する相手に同じことを許可する権利も
    無制限に含まれます。

    上記の著作権表示および本許諾表示を、ソフトウェアのすべての複製または重要な部分に
    記載するものとします。

    ソフトウェアは「現状のまま」で、明示であるか暗黙であるかを問わず、何らの保証もなく
    提供されます。ここでいう保証とは、商品性、特定の目的への適合性、および権利非侵害に
    ついての保証も含みますが、それに限定されるものではありません。 作者または著作権者は、
    契約行為、不法行為、またはそれ以外であろうと、ソフトウェアに起因または関連し、
    あるいはソフトウェアの使用またはその他の扱いによって生じる一切の請求、損害、
    その他の義務について何らの責任も負わないものとします。
*/

using CommonClass;
using orgai;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

// ※Debug版で測定。Release版だと２～３倍ぐらい速い
// train実行時間(Titanic)
// CPU 14 core 使用時：119.697 (420 NN   38gene)
// GPU 4608 core 使用時：42.495 (420 NN   38gene)

// train実行時間(HousePrices)  ※たまに誤差が大きくなることがある
// CPU 14 core 使用時：79.002 (420 NN   95gene)
// GPU 4608 core 使用時：35.685 (420NN  95gene)

namespace OrgaiW
{
    public partial class MainForm : Form
    {
        string newLine = System.Environment.NewLine;

        static Orgai   orgai   = new Orgai();   // Orgai

        static Queue<string> commandQueue = new Queue<string>();

        int mode;

        bool stopFlag = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            bool success;

            Config.InitOrgai(orgai, textBoxLog, commandQueue, labelBest, labelBestAccuracyRate);

            success = orgai.LoadData();

            if(success == false)
            {
                MessageBox.Show("train.csv, test.csv が無いと起動できません。これらは kaggleサイトより入手できます。");

                Application.Exit();
            }
        }

        void StartProc(int mode)
        {
            buttonTrain.Enabled = false;
            buttonTest.Enabled = false;
            buttonPredict.Enabled = false;

            buttonStop.Enabled = true;

            orgai.LogClear();

            this.mode = mode;
        }

        private void buttonTrain_Click(object sender, EventArgs e)
        {
            StartProc(1);

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    backgroundWorkerMain.RunWorkerAsync();
                }
                catch (Exception)
                {
                    Thread.Sleep(500);

                    continue;
                }

                break;
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            StartProc(2);

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    backgroundWorkerMain.RunWorkerAsync();
                }
                catch (Exception)
                {
                    Thread.Sleep(500);

                    continue;
                }

                break;
            }
        }

        private void buttonPredict_Click(object sender, EventArgs e)
        {
            StartProc(3);

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    backgroundWorkerMain.RunWorkerAsync();
                }
                catch (Exception)
                {
                    Thread.Sleep(500);

                    continue;
                }

                break;
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonStop.Enabled = false;

            orgai.StopProc();

            buttonTrain.Enabled = true;
            buttonTest.Enabled = true;
            buttonPredict.Enabled = true;
        }

        private void backgroundWorkerMain_DoWork(object sender, DoWorkEventArgs e)
        {
            long ms;

            LibDiag.StopwatchStart();

            if (mode == 1)
            {
                orgai.TrainMode();
            }
            else if (mode == 2)
            {
                orgai.TestMode();
            }
            else if (mode == 3)
            {
                orgai.PredictMode();
            }

            ms = LibDiag.StopwatchStop();

            MessageBox.Show("処理時間：" + ms + "ms", "メッセージ",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            stopFlag = true;
        }

        private void timerLog_Tick(object sender, EventArgs e)
        {
            string logStr = "";

            for (int i=0; i<100000; i++)
            {
                if(commandQueue.Count > 0)
                {
                    string command;
                    string cmd;
                    string log;

                    command = commandQueue.Dequeue();

                    if (command != null)
                    {
                        cmd = command.Substring(0, 4);

                        if (cmd == "LOG:")
                        {
                            log = command.Substring(4);

                            logStr += log + newLine;
                        }
                        else if (cmd == "STP:")
                        {
                            if (logStr != "")
                            {
                                orgai.LogWrite(logStr);

                                logStr = "";
                            }

                            buttonTrain.Enabled = true;
                            buttonTest.Enabled = true;
                            buttonPredict.Enabled = true;

                            buttonStop.Enabled = false;
                        }
                        else if (cmd == "ACC:")
                        {
                            labelBestAccuracyRate.Text = command.Substring(4) + " " + orgai.bestUnit;
                        }
                        else if (cmd == "GEN:")
                        {
                            labelGene.Text = command.Substring(4);
                        }
                    }
                    else
                    {
                        // たまに来てしまう
                    }
                }
                else
                {
                    if(logStr != "")
                    {
                        orgai.LogWrite(logStr);

                        logStr = "";
                    }

                    break;
                }
            }

            if(stopFlag)
            {
                stopFlag = false;

                if (logStr != "")
                {
                    orgai.LogWrite(logStr);

                    logStr = "";
                }

                buttonTrain.Enabled = true;
                buttonTest.Enabled = true;
                buttonPredict.Enabled = true;

                buttonStop.Enabled = false;
            }
        }
    }
}

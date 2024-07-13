namespace OrgaiW
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonTrain = new System.Windows.Forms.Button();
            this.buttonTest = new System.Windows.Forms.Button();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.buttonPredict = new System.Windows.Forms.Button();
            this.backgroundWorkerMain = new System.ComponentModel.BackgroundWorker();
            this.timerLog = new System.Windows.Forms.Timer(this.components);
            this.buttonStop = new System.Windows.Forms.Button();
            this.labelBest = new System.Windows.Forms.Label();
            this.labelBestAccuracyRate = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelGene = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonTrain
            // 
            this.buttonTrain.Location = new System.Drawing.Point(11, 7);
            this.buttonTrain.Margin = new System.Windows.Forms.Padding(2);
            this.buttonTrain.Name = "buttonTrain";
            this.buttonTrain.Size = new System.Drawing.Size(64, 22);
            this.buttonTrain.TabIndex = 0;
            this.buttonTrain.Text = "train";
            this.buttonTrain.UseVisualStyleBackColor = true;
            this.buttonTrain.Click += new System.EventHandler(this.buttonTrain_Click);
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(224, 7);
            this.buttonTest.Margin = new System.Windows.Forms.Padding(2);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(64, 22);
            this.buttonTest.TabIndex = 1;
            this.buttonTest.Text = "test";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // textBoxLog
            // 
            this.textBoxLog.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxLog.Location = new System.Drawing.Point(11, 33);
            this.textBoxLog.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(530, 481);
            this.textBoxLog.TabIndex = 2;
            // 
            // buttonPredict
            // 
            this.buttonPredict.Location = new System.Drawing.Point(156, 7);
            this.buttonPredict.Margin = new System.Windows.Forms.Padding(2);
            this.buttonPredict.Name = "buttonPredict";
            this.buttonPredict.Size = new System.Drawing.Size(64, 22);
            this.buttonPredict.TabIndex = 3;
            this.buttonPredict.Text = "predict";
            this.buttonPredict.UseVisualStyleBackColor = true;
            this.buttonPredict.Click += new System.EventHandler(this.buttonPredict_Click);
            // 
            // backgroundWorkerMain
            // 
            this.backgroundWorkerMain.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerMain_DoWork);
            // 
            // timerLog
            // 
            this.timerLog.Enabled = true;
            this.timerLog.Tick += new System.EventHandler(this.timerLog_Tick);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(79, 7);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(64, 22);
            this.buttonStop.TabIndex = 4;
            this.buttonStop.Text = "stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // labelBest
            // 
            this.labelBest.AutoSize = true;
            this.labelBest.Location = new System.Drawing.Point(313, 12);
            this.labelBest.Name = "labelBest";
            this.labelBest.Size = new System.Drawing.Size(67, 12);
            this.labelBest.TabIndex = 5;
            this.labelBest.Text = "最高正解率:";
            // 
            // labelBestAccuracyRate
            // 
            this.labelBestAccuracyRate.AutoSize = true;
            this.labelBestAccuracyRate.Location = new System.Drawing.Point(386, 12);
            this.labelBestAccuracyRate.Name = "labelBestAccuracyRate";
            this.labelBestAccuracyRate.Size = new System.Drawing.Size(21, 12);
            this.labelBestAccuracyRate.TabIndex = 6;
            this.labelBestAccuracyRate.Text = "0 %";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(472, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "世代:";
            // 
            // labelGene
            // 
            this.labelGene.AutoSize = true;
            this.labelGene.Location = new System.Drawing.Point(509, 12);
            this.labelGene.Name = "labelGene";
            this.labelGene.Size = new System.Drawing.Size(11, 12);
            this.labelGene.TabIndex = 8;
            this.labelGene.Text = "1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 525);
            this.Controls.Add(this.labelGene);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelBestAccuracyRate);
            this.Controls.Add(this.labelBest);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonPredict);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.buttonTrain);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "OrgaiW";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonTrain;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Button buttonPredict;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMain;
        private System.Windows.Forms.Timer timerLog;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Label labelBest;
        private System.Windows.Forms.Label labelBestAccuracyRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelGene;
    }
}


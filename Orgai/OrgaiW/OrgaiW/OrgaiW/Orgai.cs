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
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace orgai
{
    public class Orgai
    {
        public enum EnAiType
        {
            TITANIC,
            HOUSE_PRICES,
        }

        public static LibCsv libCsv = new LibCsv();  // csvライブラリ
        public static LibLog libLog = new LibLog();  // logライブラリ
        public static LibData libData = new LibData();  // Dataライブラリ
        public static LibMath libMath = new LibMath();  // Mathライブラリ
        public static LibSp libSp = new LibSp();  // SimpleParallelライブラリ

        Queue<string> commandQueue;

        public string bestUnit = "";

        public bool useNvidiaGpu = false;

        public EnAiType aiType = EnAiType.TITANIC;  // Titanic, HousePrices

        public string trainDataPath = "";
        public string modelFolderPath = "";

        public string testDataPath = "";
        public string testResultFolderPath = "";
        
        public string predictDataPath = "";

        public static float wMin = -10f;
        public static float wMax = 10f;

        public int inputNeuronNum = -1;  // -1:Auto
        public List<int> middleNeuronNums = new List<int>();  // 中間層のニューロン数のリスト
        public int outputNeuronNum = 0;

        public int neuralNetworkNum = 0;
        public int betterNeuralNetworkNum = 0;
        public float derivationRate = 0.97f;  // この割合だけ 派生元のパラメータを継承する

        public int geneMax = 40;

        public string idColName = "";
        public string correctColName = "";
        public float posiValue = 1f;

        public string judgmentResultStrPosi = "";
        public string judgmentResultPosi = "";
        public string judgmentResultStrNega = "";
        public string judgmentResultNega = "";

        DataTable dtTrain;  // 学習用データ
        DataTable dtTest;  // テスト用データ
        DataTable dtPredict;    // 予測用データ
        DataTable dtPlusTrainTest;  // 学習用データ ＋ テスト用データ
        DataTable dtPlusTestTrain;  // テスト用データ + 学習用データ
        DataTable dtPlusPredictTest;  // 予測用データ ＋ テスト用データ
        int trainDataNum;   // 学習用データの個数
        int testDataNum;     // テスト用データの個数
        int predictDataNum;     // 予測用データの個数

        bool stopFlag = false;

        // LibSp ↓↓↓↓↓↓↓↓↓↓

        // TITANIC か HOUSE_PRICES を選択
        const int skipNum = 5;  // TITANIC
        //const int skipNum = 3;  // HOUSE PRICES

        const int neuralNetNum = 420;
        const int trainDataNumMax = 1500;
        const int inputNeuronNumMax = 20;

        const int middleLayerCountMax = 10;
        const int middleLayerNeuronNumMax = 40;

        // FloatShareNum
        const int FS1 = 11;
        const int FS2 = 1; // dummy
        const int FS3 = 1; // dummy
        const int FS4 = (inputNeuronNumMax);
        const int FS5 = 1; // trainDataNumMax
        const int FS6 = 1; // dummy
        const int FS7 = middleLayerCountMax;

        const int FS8 = inputNeuronNumMax;
        const int FS9 = ((1 * inputNeuronNumMax) * skipNum);
        const int FS10 = 1; // dummy
        const int FS11 = (inputNeuronNumMax * skipNum);

        const int FS12 = (middleLayerNeuronNumMax * middleLayerCountMax);
        const int FS13 = (((middleLayerNeuronNumMax * middleLayerNeuronNumMax) * middleLayerCountMax) * skipNum);
        const int FS14 = 1; // dummy
        const int FS15 = ((middleLayerNeuronNumMax * middleLayerCountMax) * skipNum);

        const int FS16 = 1;
        const int FS17 = ((middleLayerNeuronNumMax * 1) * skipNum);
        const int FS18 = 1; // dummy
        const int FS19 = (1 * skipNum);

        // FloatInput0Num
        const int FIZ1 = skipNum; // nnNo

        const int FIZ2 = ((1 * inputNeuronNumMax) * skipNum);
        const int FIZ3 = (((middleLayerNeuronNumMax * middleLayerNeuronNumMax) * middleLayerCountMax) * skipNum);
        const int FIZ4 = ((middleLayerNeuronNumMax * 1) * skipNum);

        // FloatInputNum
        //const int FI1 = 1;

        // FloatOutputNum
        const int FO1 = neuralNetNum; // PredictValueList

        // Offset(FloatShare)
        const int FSOS1 = FS1;
        const int FSOS2 = (FSOS1 + FS2);
        const int FSOS3 = (FSOS2 + FS3);
        const int FSOS4 = (FSOS3 + FS4);
        const int FSOS5 = (FSOS4 + FS5);
        const int FSOS6 = (FSOS5 + FS6);
        const int FSOS7 = (FSOS6 + FS7);
        const int FSOS8 = (FSOS7 + FS8);
        const int FSOS9 = (FSOS8 + FS9);
        const int FSOS10 = (FSOS9 + FS10);
        const int FSOS11 = (FSOS10 + FS11);
        const int FSOS12 = (FSOS11 + FS12);
        const int FSOS13 = (FSOS12 + FS13);
        const int FSOS14 = (FSOS13 + FS14);
        const int FSOS15 = (FSOS14 + FS15);
        const int FSOS16 = (FSOS15 + FS16);
        const int FSOS17 = (FSOS16 + FS17);
        const int FSOS18 = (FSOS17 + FS18);
        const int FSOS19 = (FSOS18 + FS19);

        // Offset(FloatInput0)
        const int FIZOS1 = FIZ1;
        const int FIZOS2 = (FIZOS1 + FIZ2);
        const int FIZOS3 = (FIZOS2 + FIZ3);
        const int FIZOS4 = (FIZOS3 + FIZ4);

        // Offset(FloatInput)
        //const int FIOS1 = FI1;

        // Offset(FloatOutput)
        const int FOOS1 = FO1;

        // 各種変数のOffset(FloatShare)
        int OSfsCorrectNum       = 0;
        int OSfsAccuracyRate     = 1;
        int OSfsCalcError        = 2;
        int OSfsTrainDataNum     = 3;
        int OSfsAiType           = 4;
        int OSfsThresholdVal     = 5;
        int OSfsPosiValue        = 6;
        int OSfsInputNeuronNum   = 7;
        int OSfsMiddleLayerCount = 8;
        int OSfsOutputNeuronNum  = 9;    // 1
        int OSfsOutputValue      = 10;

        // 各種配列のOffset(FloatShare)
        int OSfsInputValueList(int aryIdx) { return (FSOS3 + aryIdx); }
        int OSfsCorrectValueList(int aryIdx) { return (FSOS4 + aryIdx); }
        int OSfsMiddleNeuronNums(int aryIdx) { return (FSOS6 + aryIdx); }

        int OSfsInputLayerDendriteNum(int aryIdx) { return (FSOS7 + aryIdx); }
        int OSfsInputLayerXVal(int skipIdx, int aryIdx) { return (FSOS8 + ((1 * inputNeuronNumMax) * skipIdx) + aryIdx); }
        int OSfsInputLayerYVal(int skipIdx, int aryIdx) { return (FSOS10 + (inputNeuronNumMax * skipIdx) + aryIdx); }

        int OSfsMiddleLayerDendriteNum(int layIdx, int aryIdx)       { return (FSOS11 + layIdx * middleLayerNeuronNumMax + aryIdx); }
        int OSfsMiddleLayerXVal(int skipIdx, int layIdx, int aryIdx, int dIdx)    { return (FSOS12 + (((middleLayerNeuronNumMax * middleLayerNeuronNumMax) * middleLayerCountMax) * skipIdx) + layIdx * middleLayerNeuronNumMax * middleLayerNeuronNumMax + aryIdx * middleLayerNeuronNumMax + dIdx); }
        int OSfsMiddleLayerYVal(int skipIdx, int layIdx, int aryIdx)              { return (FSOS14 + ((middleLayerNeuronNumMax * middleLayerCountMax) * skipIdx) + layIdx * middleLayerNeuronNumMax + aryIdx); }

        int OSfsOutputLayerDendriteNum(int aryIdx)       { return (FSOS15 + aryIdx); }
        int OSfsOutputLayerXVal(int skipIdx, int aryIdx, int dIdx)    { return (FSOS16 + ((middleLayerNeuronNumMax * 1) * skipIdx) + aryIdx * middleLayerNeuronNumMax + dIdx); }
        int OSfsOutputLayerYVal(int skipIdx, int aryIdx)              { return (FSOS18 + (1 * skipIdx) + aryIdx); } // 1個

        // 各種変数へのインタフェース(Get/Set)(FloatShare)
        float fsCorrectNum       { get { return libSp.GetFS(OSfsCorrectNum); }         set { libSp.SetFS(OSfsCorrectNum, value); } }
        float fsAccuracyRate     { get { return libSp.GetFS(OSfsAccuracyRate); }       set { libSp.SetFS(OSfsAccuracyRate, value); } }
        float fsCalcError        { get { return libSp.GetFS(OSfsCalcError); }          set { libSp.SetFS(OSfsCalcError, value); } }
        float fsTrainDataNum     { get { return libSp.GetFS(OSfsTrainDataNum); }       set { libSp.SetFS(OSfsTrainDataNum, value); } }
        float fsAiType           { get { return libSp.GetFS(OSfsAiType); }             set { libSp.SetFS(OSfsAiType, value); } }
        float fsThresholdVal     { get { return libSp.GetFS(OSfsThresholdVal); }       set { libSp.SetFS(OSfsThresholdVal, value); } }
        float fsPosiValue        { get { return libSp.GetFS(OSfsPosiValue); }          set { libSp.SetFS(OSfsPosiValue, value); } }
        float fsInputNeuronNum   { get { return libSp.GetFS(OSfsInputNeuronNum); }     set { libSp.SetFS(OSfsInputNeuronNum, value); } }
        float fsMiddleLayerCount { get { return libSp.GetFS(OSfsMiddleLayerCount); }   set { libSp.SetFS(OSfsMiddleLayerCount, value); } }
        float fsOutputNeuronNum  { get { return libSp.GetFS(OSfsOutputNeuronNum); }    set { libSp.SetFS(OSfsOutputNeuronNum, value); } }
        float fsOutputValue      { get { return libSp.GetFS(OSfsOutputValue); }        set { libSp.SetFS(OSfsOutputValue, value); } }

        // 各種配列へのインタフェース(Get)(FloatShare)
        float fsInputValueList(int aryIdx) { return libSp.GetFS(OSfsInputValueList(aryIdx)); }
        float fsCorrectValueList(int aryIdx) { return libSp.GetFS(OSfsCorrectValueList(aryIdx)); }
        float fsMiddleNeuronNums(int aryIdx) { return libSp.GetFS(OSfsMiddleNeuronNums(aryIdx)); }

        float fsInputLayerDendriteNum(int aryIdx) { return libSp.GetFS(OSfsInputLayerDendriteNum(aryIdx)); }
        float fsInputLayerXVal(int skipIdx, int aryIdx) { return libSp.GetFS(OSfsInputLayerXVal(skipIdx, aryIdx)); }
        float fsInputLayerYVal(int skipIdx, int aryIdx) { return libSp.GetFS(OSfsInputLayerYVal(skipIdx, aryIdx)); }

        float fsMiddleLayerDendriteNum(int layIdx, int aryIdx) { return libSp.GetFS(OSfsMiddleLayerDendriteNum(layIdx, aryIdx)); }
        float fsMiddleLayerXVal(int skipIdx, int layIdx, int aryIdx, int dIdx) { return libSp.GetFS(OSfsMiddleLayerXVal(skipIdx, layIdx, aryIdx, dIdx)); }
        float fsMiddleLayerYVal(int skipIdx, int layIdx, int aryIdx) { return libSp.GetFS(OSfsMiddleLayerYVal(skipIdx, layIdx, aryIdx)); }

        float fsOutputLayerDendriteNum(int aryIdx) { return libSp.GetFS(OSfsOutputLayerDendriteNum(aryIdx)); }
        float fsOutputLayerXVal(int skipIdx, int aryIdx, int dIdx) { return libSp.GetFS(OSfsOutputLayerXVal(skipIdx, aryIdx, dIdx)); }
        float fsOutputLayerYVal(int skipIdx, int aryIdx) { return libSp.GetFS(OSfsOutputLayerYVal(skipIdx, aryIdx)); }

        // 各種配列へのインタフェース(Set)(FloatShare)
        void SfsInputValueList(int aryIdx, float value) { libSp.SetFS(OSfsInputValueList(aryIdx), value); }
        void SfsCorrectValueList(int aryIdx, float value) { libSp.SetFS(OSfsCorrectValueList(aryIdx), value); }
        void SfsMiddleNeuronNums(int aryIdx, float value) { libSp.SetFS(OSfsMiddleNeuronNums(aryIdx), value); }

        void SfsInputLayerDendriteNum(int aryIdx, float value) { libSp.SetFS(OSfsInputLayerDendriteNum(aryIdx), value); }
        void SfsInputLayerXVal(int skipIdx, int aryIdx, float value) { libSp.SetFS(OSfsInputLayerXVal(skipIdx, aryIdx), value); }
        void SfsInputLayerYVal(int skipIdx, int aryIdx, float value) { libSp.SetFS(OSfsInputLayerYVal(skipIdx, aryIdx), value); }

        void SfsMiddleLayerDendriteNum(int layIdx, int aryIdx, float value) { libSp.SetFS(OSfsMiddleLayerDendriteNum(layIdx, aryIdx), value); }
        void SfsMiddleLayerXVal(int skipIdx, int layIdx, int aryIdx, int dIdx, float value) { libSp.SetFS(OSfsMiddleLayerXVal(skipIdx, layIdx, aryIdx, dIdx), value); }
        void SfsMiddleLayerYVal(int skipIdx, int layIdx, int aryIdx, float value) { libSp.SetFS(OSfsMiddleLayerYVal(skipIdx, layIdx, aryIdx), value); }

        void SfsOutputLayerDendriteNum(int aryIdx, float value) { libSp.SetFS(OSfsOutputLayerDendriteNum(aryIdx), value); }
        void SfsOutputLayerXVal(int skipIdx, int aryIdx, int dIdx, float value) { libSp.SetFS(OSfsOutputLayerXVal(skipIdx, aryIdx, dIdx), value); }
        void SfsOutputLayerYVal(int skipIdx, int aryIdx, float value) { libSp.SetFS(OSfsOutputLayerYVal(skipIdx, aryIdx), value); }

        // 各種変数のOffset(FloatInput0)
        // なし

        // 各種配列のOffset(FloatInput0)
        int OSfizNnNo(int skipIdx) { return (/*FIZOS1 +*/ skipIdx); }
        int OSfizInputLayerWVal(int skipIdx, int aryIdx) { return (FIZOS1 + ((1 * inputNeuronNumMax) * skipIdx) + aryIdx); }
        int OSfizMiddleLayerWVal(int skipIdx, int layIdx, int aryIdx, int dIdx) { return (FIZOS2 + (((middleLayerNeuronNumMax * middleLayerNeuronNumMax) * middleLayerCountMax) * skipIdx) + layIdx * middleLayerNeuronNumMax * middleLayerNeuronNumMax + aryIdx * middleLayerNeuronNumMax + dIdx); }
        int OSfizOutputLayerWVal(int skipIdx, int aryIdx, int dIdx) { return (FIZOS3 + ((middleLayerNeuronNumMax * 1) * skipIdx) + aryIdx * middleLayerNeuronNumMax + dIdx); }

        // 各種変数へのインタフェース(Get/Set)(FloatInput0)
        // なし

        // 各種配列へのインタフェース(Get)(FloatInput0)
        float fizNnNo(int skipIdx) { return libSp.GetFI0(OSfizNnNo(skipIdx)); }

        float fizInputLayerWVal(int skipIdx, int aryIdx) { return libSp.GetFI0(OSfizInputLayerWVal(skipIdx, aryIdx)); }
        float fizMiddleLayerWVal(int skipIdx, int layIdx, int aryIdx, int dIdx) { return libSp.GetFI0(OSfizMiddleLayerWVal(skipIdx, layIdx, aryIdx, dIdx)); }
        float fizOutputLayerWVal(int skipIdx, int aryIdx, int dIdx) { return libSp.GetFI0(OSfizOutputLayerWVal(skipIdx, aryIdx, dIdx)); }

        // 各種配列へのインタフェース(Set)(FloatInput0)
        void SfizNnNo(int skipIdx, float value) { libSp.SetFI0(OSfizNnNo(skipIdx), value); }

        void SfizInputLayerWVal(int skipIdx, int aryIdx, float value) { libSp.SetFI0(OSfizInputLayerWVal(skipIdx, aryIdx), value); }
        void SfizMiddleLayerWVal(int skipIdx, int layIdx, int aryIdx, int dIdx, float value) { libSp.SetFI0(OSfizMiddleLayerWVal(skipIdx, layIdx, aryIdx, dIdx), value); }
        void SfizOutputLayerWVal(int skipIdx, int aryIdx, int dIdx, float value) { libSp.SetFI0(OSfizOutputLayerWVal(skipIdx, aryIdx, dIdx), value); }

        // 各種変数のOffset(FloatInput)
        // なし

        // 各種配列のOffset(FloatInput)
        // なし

        // 各種変数へのインタフェース(Get/Set)(FloatInput)
        // なし

        // 各種配列へのインタフェース(Get)(FloatInput)
        // なし

        // 各種配列へのインタフェース(Set)(FloatInput)
        // なし

        // 各種配列のOffset(FloatOutput)
        int OSfoPredictValueList(int aryIdx) { return (0 + aryIdx); }

        // 各種配列へのインタフェース(Get)(FloatOutput)
        float foPredictValueListBIdx(int bIdx, int aryIdx) { return libSp.GetFO(bIdx, OSfoPredictValueList(aryIdx)); }

        // LibSp ↑↑↑↑↑↑↑↑↑↑

        public void Init(TextBox textBoxLog, Queue<string> commandQueue)
        {
            libLog.Init(textBoxLog);

            this.commandQueue = commandQueue;
        }

        /// <summary>
        /// 学習用データ、テスト用データ、予測用データを読み込みます。
        /// </summary>
        /// <returns>bool | true:成功   false:失敗</returns>
        public bool LoadData()
        {
            // 学習用データ読み込み
            dtTrain = libCsv.Read(trainDataPath, true);

            if(dtTrain == null)
            {
                return false;
            }

            // テスト用データ読み込み
            dtTest = libCsv.Read(testDataPath, true);

            if (dtTest == null)
            {
                return false;
            }

            // 予測用データ読み込み
            dtPredict = libCsv.Read(predictDataPath, true);

            if (dtPredict == null)
            {
                return false;
            }

            Config.DataProcessing(dtTrain, dtTest, dtPredict);

            trainDataNum = dtTrain.Rows.Count;  // 学習用データの個数
            testDataNum = dtTest.Rows.Count;  // テスト用データの個数
            predictDataNum = dtPredict.Rows.Count;  // 予測用データの個数

            dtPlusTrainTest = GetTrainAndTest(dtTrain, dtTest);
            dtPlusTestTrain = GetTrainAndTest(dtTest, dtTrain);
            dtPlusPredictTest = GetTrainAndTest(dtPredict, dtTest);

            return true;
        }

        /// <summary>
        /// LogにDataTableの内容を出力する。
        /// </summary>
        /// <param name="dt">内容を出力するDataTable</param>
        void WriteDataTable(DataTable dt)
        {
            string log;

            log = libData.DataTableToString(dt);

            libLog.WriteLine(log);
        }

        void LogAddDataTable(DataTable dt)
        {
            string log;

            log = libData.DataTableToString(dt);

            commandQueue.Enqueue("LOG:" + log);
        }

        public void LogClear()
        {
            libLog.Clear();
        }

        public void LogWrite(string log)
        {
            libLog.Write(log);
        }

        void LogAdd(string log)
        {
            try
            {
                commandQueue.Enqueue("LOG:" + log);
            }
            catch(Exception)
            {

            }
        }

        void SetBestAccuracyRate(float accuracy)
        {
            commandQueue.Enqueue("ACC:" + accuracy);
        }

        void SetGene(int gene)
        {
            commandQueue.Enqueue("GEN:" +  gene);
        }

        public void StopProc()
        {
            stopFlag = true;
        }

        DataTable GetTrainAndTest(DataTable dtMain, DataTable dtSub)
        {
            DataTable dt1;
            DataTable dt2;
            DataTable dtPlus;

            dt1 = dtMain.Copy();
            dt2 = dtSub.Copy();

            if (dt1.Columns.Contains(correctColName))
            {
                dt1.Columns.Remove(correctColName);
            }

            if (dt2.Columns.Contains(correctColName))
            {
                dt2.Columns.Remove(correctColName);
            }

            dtPlus = libData.DataTablePlus(dt1, dt2);

            return dtPlus;
        }

        /// <summary>
        /// 学習（訓練）モード
        /// </summary>
        /// <param name="messageBox">true:処理完了時にメッセージボックスを出す   false:出さない</param>
        /// <returns>bool | true:成功   false:失敗</returns>
        public bool TrainMode(bool messageBox = false)
        {
            float thresholdVal = 0;  // 出力値（○○らしさ）がこの値以上ならば、○○と判定する。
            int gene = 0;

            float bestAccuracyRate = -1.0f; // 最も正解率の高いニューラルネットワークの正解率（判定精度）
            float bestCalcError = 1.0E+15f; // 最も誤差の小さいニューラルネットワークの誤差（判定精度）

            List<NeuralNet> betterNeuralNet = new List<NeuralNet>();  // 次の世代へ生き残るニューラルネットワークのリスト
            List<float> betterNeuralNetAccuracyRate = new List<float>();

            List<float> betterNeuralNetCalcError = new List<float>();

            List<int> correctNumList;
            List<float> correctValueList;  // 正解の値のリスト

            List<List<float>> inputValueList;  // 入力層に渡すパラメータのリストのリスト（学習用データの数だけある）

            int parallelismDegree = Environment.ProcessorCount; // 使用可能なプロセッサの数

            stopFlag = false;

            LogAdd("学習（訓練）モードを実行します。");

            LogAdd("");
            LogAdd("Train Data:");

            LogAddDataTable(dtTrain); // 学習用データの内容表示

            {
                inputValueList = new List<List<float>>();  // 入力層に渡すパラメータのリストのリスト（学習用データの数だけある）
                correctValueList = new List<float>();

                for (int r = 0; r < dtTrain.Rows.Count; r++)
                {
                    List<float> inputValue;  // 入力層に渡すパラメータのリスト

                    inputValue = Config.MakeInputValue(dtPlusTrainTest, r);

                    inputValueList.Add(inputValue);

                    correctValueList.Add(float.Parse("" + dtTrain.Rows[r][correctColName]));
                }
            }

            LogAdd("");
            LogAdd("Train Data Num: " + trainDataNum);

            {
                List<float> inputValue;  // 入力層に渡すパラメータのリスト
                int inputValueNum;

                inputValue = inputValueList[0];
                inputValueNum = inputValue.Count;

                if (inputNeuronNum == -1)
                {
                    inputNeuronNum = inputValueNum;
                }

                LogAdd("");
                LogAdd("Input Value Num:  " + inputValueNum);
                LogAdd("Input Neuron Num: " + inputNeuronNum);

                if (inputValueNum == inputNeuronNum)
                {
                    LogAdd("OK");
                }
                else
                {
                    LogAdd("NG");

                    return false;
                }
            }

            LogAdd("");

            if (useNvidiaGpu)
            {
                libSp.GetParams();

                {
                    int result;

                    result = libSp.DeviceSet();

                    if (result != 0)
                    {
                        return false;
                    }
                }

                // Share(初期値)セット ↓↓↓↓↓↓↓↓↓↓↓↓
                for (libSp.bIdx = 0; libSp.bIdx < libSp.threadNum; libSp.bIdx++)  // trainDataNum
                {
                    fsTrainDataNum = trainDataNum;
                    fsAiType = (int)aiType;
                    fsThresholdVal = thresholdVal;
                    fsPosiValue = posiValue;
                    fsInputNeuronNum = inputNeuronNum;
                    fsMiddleLayerCount = middleNeuronNums.Count;
                    fsOutputNeuronNum = outputNeuronNum;

                    for (int i = 0; i < middleNeuronNums.Count; i++)
                    {
                        SfsMiddleNeuronNums(i, middleNeuronNums[i]);
                    }

                    for (int i = 0; i < inputNeuronNum; i++)
                    {
                        SfsInputLayerDendriteNum(i, 1);
                    }

                    for (int l = 0; l < middleNeuronNums.Count; l++)
                    {
                        for (int i = 0; i < middleNeuronNums[l]; i++)
                        {
                            if(l == 0)
                            {
                                SfsMiddleLayerDendriteNum(l, i, inputNeuronNum);
                            }
                            else
                            {
                                SfsMiddleLayerDendriteNum(l, i, middleNeuronNums[l - 1]);
                            }
                        }
                    }

                    for (int i = 0; i < outputNeuronNum; i++)
                    {
                        SfsOutputLayerDendriteNum(i, middleNeuronNums[middleNeuronNums.Count - 1]);
                    }

                    {
                        for (int i = 0; i < inputNeuronNum; i++)
                        {
                            SfsInputValueList(i, inputValueList[libSp.bIdx][i]);
                        }
                    }
                } // for (libSp.bIdx = 0; libSp.bIdx < libSp.threadNum; libSp.bIdx++)  // trainDataNum

                // Share(初期値)セット ↑↑↑↑↑↑↑↑↑↑↑↑

                {
                    int result;

                    result = libSp.SetShare();

                    if (result != 0)
                    {
                        return false;
                    }
                }
            } // if (useNvidiaGpu)

            while (true)
            {
                List<NeuralNet> neuralNetMaru = new List<NeuralNet>();  // ○○らしさを出力するニューラルネットワークのリスト
                int geneBestCorrectNum = -1;        // 最も正解率の高いニューラルネットワークの正解数
                float geneBestAccuracyRate = -1.0f; // 最も正解率の高いニューラルネットワークの正解率（判定精度）
                float geneBestCalcError = 1.0E+15f;  // 最も誤差の小さいニューラルネットワークの誤差
                int geneBestNeuralNet = -1;         // 最も正解率の高いニューラルネットワークのインデックス

                gene++;

                LogAdd("");
                LogAdd("gene = " + gene);

                SetGene(gene);

                // (1) 各ニューロンの結合強度がランダムなニューラルネットワークを420個作る。
                // （出力層のニューロンは1個で固定。）

                if (gene == 1)
                {
                    // 420個のニューラルネットワークを作成
                    for (int i = 0; i < neuralNetworkNum; i++)
                    {
                        NeuralNet nn;

                        nn = new NeuralNet(inputNeuronNum, middleNeuronNums);

                        neuralNetMaru.Add(nn);

                        betterNeuralNet.Add(nn);

                        if (aiType == EnAiType.TITANIC)
                        {
                            betterNeuralNetAccuracyRate.Add(0f);
                        }
                        else if (aiType == EnAiType.HOUSE_PRICES)
                        {
                            betterNeuralNetCalcError.Add(1.0E+15f);
                        }
                        else
                        {
                            // ここには来ないはず
                        }
                    }
                }
                else
                {
                    betterNeuralNetAccuracyRate = new List<float>();
                    betterNeuralNetCalcError = new List<float>();

                    for (int i = 0; i < neuralNetworkNum; i++)
                    {
                        if (i < betterNeuralNetworkNum)
                        {
                            neuralNetMaru.Add(betterNeuralNet[i]);
                        }
                        else
                        {
                            NeuralNet nn;

                            nn = new NeuralNet(inputNeuronNum, middleNeuronNums,
                                betterNeuralNet[LibRandom.Next(betterNeuralNetworkNum)],
                                derivationRate);

                            neuralNetMaru.Add(nn);

                            betterNeuralNet.Add(nn);
                        }

                        if (aiType == EnAiType.TITANIC)
                        {
                            betterNeuralNetAccuracyRate.Add(0f);
                        }
                        else if (aiType == EnAiType.HOUSE_PRICES)
                        {
                            betterNeuralNetCalcError.Add(1.0E+15f);
                        }
                        else
                        {
                            // ここには来ないはず
                        }
                    }
                }

                {
                    correctNumList = new List<int>();

                    for (int i = 0; i < neuralNetworkNum; i++)
                    {
                        correctNumList.Add(0);
                    }
                }

                //LogAdd("");
                //LogAdd("使用可能なプロセッサの数: " + parallelismDegree);

                //LogAdd("");
                //LogAdd("処理時間計測開始");
                //LibDiag.StopwatchStart();

                // (2) 作成した420個のニューラルネットワークに学習用データをインプットする。
                // (3) 全てのニューラルネットワークで判定処理を行い、正解率を出す。
                LogAdd("");
                LogAdd("各ニューラルネットワークの正解率（判定精度）:");

                if (useNvidiaGpu == false)
                {
                    //for (int i = 0; i < neuralNetworkNum; i++)

                    // CPU 全core使用
                    Parallel.For(0, neuralNetworkNum, i =>
                    {
                        if (stopFlag == false)
                        {
                            int correctNum = 0;     // 正解数
                            float accuracyRate;     // 正解率（判定精度）
                            float calcError;        // 誤差

                            List<float> predictValueList;  // 予測値のリスト

                            predictValueList = new List<float>();

                            for (int r = 0; r < dtTrain.Rows.Count; r++)
                            {
                                List<float> inputValue;  // 入力層に渡すパラメータのリスト
                                float outputValue = 0;  // 出力値（○○らしさ）

                                inputValue = inputValueList[r];

                                // 入力層に4つのパラメータを渡す
                                neuralNetMaru[i].SetInputValue(inputValue);

                                // 出力値（○○らしさ）を計算する
                                neuralNetMaru[i].ComputeOutputValue();

                                // 出力値（○○らしさ）を取得する
                                outputValue = neuralNetMaru[i].GetOutputValue();

                                if (aiType == EnAiType.TITANIC)
                                {
                                    // 正解か不正解か判定
                                    if (outputValue >= thresholdVal)
                                    {
                                        // AIは、○○と判定

                                        if (correctValueList[r] == posiValue)
                                        {
                                            // 正解
                                            correctNum++;
                                        }
                                    }
                                    else
                                    {
                                        // AIは、○○ではないと判定

                                        if (correctValueList[r] != posiValue)
                                        {
                                            // 正解
                                            correctNum++;
                                        }
                                    }
                                }
                                else if (aiType == EnAiType.HOUSE_PRICES)
                                {
                                    predictValueList.Add(outputValue);
                                }
                                else
                                {
                                    // ここには来ないはず
                                }
                            }

                            if (aiType == EnAiType.TITANIC)
                            {
                                // 正解率（判定精度）計算
                                accuracyRate = (float)correctNum * 100 / trainDataNum;

                                // 正解率（判定精度）、正解数、予測用データの個数 を表示
                                //LogAdd("ニューラルネットワーク" + (i + 1) + " : 正解率（判定精度）: "
                                //    + accuracyRate + " % (" + correctNum + "/" + trainDataNum + ")");

                                betterNeuralNetAccuracyRate[i] = accuracyRate;
                                correctNumList[i] = correctNum;
                            }
                            else if (aiType == EnAiType.HOUSE_PRICES)
                            {
                                calcError = libMath.GetRmse(predictValueList, correctValueList);

                                // 誤差（判定精度）、正解数、予測用データの個数 を表示
                                //LogAdd("ニューラルネットワーク" + (i + 1) + " : 誤差（判定精度）: "
                                //    + calcError);

                                betterNeuralNetCalcError[i] = calcError;
                            }
                            else
                            {
                                // ここには来ないはず
                            }
                        }
                    });  // Parallel.For(0, neuralNetworkNum, i =>
                }
                else
                {
                    // Nvidia GPU 4608core

                    for (int n = 0; n < neuralNetworkNum; n += skipNum)
                    {
                        int skipIdx;
                        int nnNo;

                        libSp.bIdx = 0;

                        // INPUTセット ↓↓↓↓↓↓↓↓↓↓↓↓
                        for (skipIdx = 0; skipIdx < skipNum; skipIdx++)
                        {
                            nnNo = n + skipIdx;
                            SfizNnNo(skipIdx, nnNo);

                            for (int i = 0; i < inputNeuronNum; i++)
                            {
                                SfizInputLayerWVal(skipIdx, i, neuralNetMaru[nnNo].inputLayer.neurons[i].wVal[0]);
                            }

                            for (int l = 0; l < neuralNetMaru[nnNo].middleLayer.Count; l++)
                            {
                                for (int i = 0; i < neuralNetMaru[nnNo].middleLayer[l].neuronNum; i++)
                                {
                                    for (int j = 0; j < neuralNetMaru[nnNo].middleLayer[l].neurons[i].wVal.Count; j++)
                                    {
                                        SfizMiddleLayerWVal(skipIdx, l, i, j, neuralNetMaru[nnNo].middleLayer[l].neurons[i].wVal[j]);
                                    }
                                }
                            }

                            for (int i = 0; i < outputNeuronNum; i++)
                            {
                                for (int j = 0; j < neuralNetMaru[nnNo].outputLayer.wVal.Count; j++)
                                {
                                    SfizOutputLayerWVal(skipIdx, i, j, neuralNetMaru[nnNo].outputLayer.wVal[j]);
                                }
                            }
                        } // for (skipIdx = 0; skipIdx < skipNum; skipIdx++)

                        // INPUTセット ↑↑↑↑↑↑↑↑↑↑↑↑

                        {
                            int result;

                            // trainDataNum * skipNum loop
                            result = libSp.Main();

                            if (result != 0)
                            {
                                return false;
                            }
                        }
                    } // for (int n = 0; n < neuralNetworkNum; n++)

                    {
                        int result;

                        // Copy PredictValueList(trainDataNum * neuralNetworkNum)
                        result = libSp.GetOutput();

                        if (result != 0)
                        {
                            return false;
                        }
                    }

                    for (int n = 0; n < neuralNetworkNum; n++)
                        //Parallel.For(0, neuralNetworkNum, n =>  // なぜか逆に遅い
                    {
                        int correctNum = 0;
                        float accuracyRate = 0;
                        float calcError = 0;

                        if (aiType == EnAiType.TITANIC)
                        {
                            for (int i = 0; i < trainDataNum; i++)
                            {
                                // 正解か不正解か判定
                                if (foPredictValueListBIdx(i, n) >= thresholdVal)
                                {
                                    // AIは、Posiと判定

                                    if (correctValueList[i] == posiValue)
                                    {
                                        // 正解
                                        correctNum = correctNum + 1;
                                    }
                                }
                                else
                                {
                                    // AIは、Posiではないと判定

                                    if (correctValueList[i] != posiValue)
                                    {
                                        // 正解
                                        correctNum = correctNum + 1;
                                    }
                                }
                            }

                            // 正解率（判定精度）計算
                            accuracyRate = (float)correctNum * 100 / trainDataNum;

                            // 正解率（判定精度）、正解数、予測用データの個数 を表示
                            //LogAdd("ニューラルネットワーク" + (i + 1) + " : 正解率（判定精度）: "
                            //    + accuracyRate + " % (" + correctNum + "/" + trainDataNum + ")");
                        }
                        else if (aiType == EnAiType.HOUSE_PRICES)
                        {
                            // 誤差（RMSE）計算
                            {
                                float sum = 0;

                                for (int i = 0; i < trainDataNum; i++)
                                {
                                    sum += (float)Math.Pow(foPredictValueListBIdx(i, n) - correctValueList[i], 2);
                                }

                                calcError = (float)Math.Sqrt(sum / trainDataNum);
                            }

                            // 誤差（判定精度）、正解数、予測用データの個数 を表示
                            //LogAdd("ニューラルネットワーク" + (i + 1) + " : 誤差（判定精度）: "
                            //    + calcError);
                        }
                        else
                        {
                            // ここには来ないはず
                        }

                        if (aiType == EnAiType.TITANIC)
                        {
                            betterNeuralNetAccuracyRate[n] = accuracyRate;
                            correctNumList[n] = (int)correctNum;
                        }
                        else if (aiType == EnAiType.HOUSE_PRICES)
                        {
                            betterNeuralNetCalcError[n] = calcError;
                        }
                        else
                        {
                            // ここには来ないはず
                        }
                    } //); // for (int n = 0; n < neuralNetworkNum; n++)
                } // Nvidia GPU 4608core

                if (aiType == EnAiType.TITANIC)
                {
                    for (int i = 0; i < neuralNetworkNum; i++)
                    {
                        float accuracyRate;
                        int correctNum;

                        accuracyRate = betterNeuralNetAccuracyRate[i];
                        correctNum = correctNumList[i];

                        // 正解率（判定精度）、正解数、予測用データの個数 を表示
                        LogAdd("ニューラルネットワーク" + (i + 1) + " : 正解率（判定精度）: "
                            + accuracyRate + " % (" + correctNum + "/" + trainDataNum + ")");
                    }
                }
                else if (aiType == EnAiType.HOUSE_PRICES)
                {
                    for (int i = 0; i < neuralNetworkNum; i++)
                    {
                        float calcError;

                        calcError = betterNeuralNetCalcError[i];

                        // 誤差（判定精度）、正解数、予測用データの個数 を表示
                        LogAdd("ニューラルネットワーク" + (i + 1) + " : 誤差（判定精度）: "
                            + calcError);
                    }
                }
                else
                {
                    // ここには来ないはず
                }

                if (aiType == EnAiType.TITANIC)
                {
                    for (int i = 0; i < neuralNetworkNum; i++)
                    {
                        if (correctNumList[i] > geneBestCorrectNum)
                        {
                            // 最も正解率の高いニューラルネットワークを更新する
                            geneBestCorrectNum = correctNumList[i];
                            geneBestAccuracyRate = betterNeuralNetAccuracyRate[i];
                            geneBestNeuralNet = i;
                        }
                    }
                }
                else if (aiType == EnAiType.HOUSE_PRICES)
                {
                    for (int i = 0; i < neuralNetworkNum; i++)
                    {
                        if (betterNeuralNetCalcError[i] < geneBestCalcError)
                        {
                            // 最も誤差の小さいニューラルネットワークを更新する
                            geneBestCalcError = betterNeuralNetCalcError[i];
                            geneBestNeuralNet = i;
                        }
                    }
                }
                else
                {
                    // ここには来ないはず
                }

                //LogAdd("");
                //LogAdd("処理時間: " + LibDiag.StopwatchStop() + " ms");

                // (4) 最も正解率の高いニューラルネットワークをピックアップする。
                LogAdd("");

                if (aiType == EnAiType.TITANIC)
                {
                    LogAdd("最も正解率の高いニューラルネットワーク:");
                    LogAdd("ニューラルネットワーク" + (geneBestNeuralNet + 1) + " : 正解率（判定精度）: "
                        + geneBestAccuracyRate + " % (" + geneBestCorrectNum + "/" + trainDataNum + ")");

                    if (bestAccuracyRate < geneBestAccuracyRate)
                    {
                        bestAccuracyRate = geneBestAccuracyRate;

                        SetBestAccuracyRate(bestAccuracyRate);

                        // (9) 最終的に最も正解率の高いニューラルネットワークをファイルに保存する。
                        neuralNetMaru[geneBestNeuralNet].Save(modelFolderPath);
                    }

                    //betterNeuralNet から正解率の低いものを削除
                    {
                        for (int i = 0; i < neuralNetworkNum - betterNeuralNetworkNum; i++)
                        {
                            float worstAccuracyRate = 1000f;
                            int worstIdx = -1;

                            for (int idx = 0; idx < betterNeuralNetAccuracyRate.Count; idx++)
                            {
                                if (betterNeuralNetAccuracyRate[idx] < worstAccuracyRate)
                                {
                                    worstAccuracyRate = betterNeuralNetAccuracyRate[idx];
                                    worstIdx = idx;
                                }
                            }

                            betterNeuralNet.RemoveAt(worstIdx);
                            betterNeuralNetAccuracyRate.RemoveAt(worstIdx);
                        }
                    }
                }
                else if (aiType == EnAiType.HOUSE_PRICES)
                {
                    LogAdd("最も誤差の小さいニューラルネットワーク:");
                    LogAdd("ニューラルネットワーク" + (geneBestNeuralNet + 1) + " : 誤差（判定精度）: "
                        + geneBestCalcError);

                    if (bestCalcError > geneBestCalcError)
                    {
                        bestCalcError = geneBestCalcError;

                        SetBestAccuracyRate(bestCalcError);

                        // (9) 最終的に最も誤差の小さいニューラルネットワークをファイルに保存する。
                        neuralNetMaru[geneBestNeuralNet].Save(modelFolderPath);
                    }

                    //betterNeuralNet から誤差の大きいものを削除
                    {
                        for (int i = 0; i < neuralNetworkNum - betterNeuralNetworkNum; i++)
                        {
                            float worstCalcError = -1.0f;
                            int worstIdx = -1;

                            for (int idx = 0; idx < betterNeuralNetCalcError.Count; idx++)
                            {
                                if (betterNeuralNetCalcError[idx] > worstCalcError)
                                {
                                    worstCalcError = betterNeuralNetCalcError[idx];
                                    worstIdx = idx;
                                }
                            }

                            betterNeuralNet.RemoveAt(worstIdx);
                            betterNeuralNetCalcError.RemoveAt(worstIdx);
                        }
                    }
                }
                else
                {
                    // ここには来ないはず
                }

                ////(5) ピックアップしたニューラルネットワークの各ニューロンの結合強度をDNAとする。

                ////(6) そのDNAを派生させて、420個のDNAを作る。

                ////(7) 420個のDNAから420個のニューラルネットワークを作る。

                ////(8)(2)へ行ってxx回（世代）繰り返す。

                if (stopFlag)
                {
                    break;
                }

                if (gene >= geneMax)
                {
                    break;
                }
            } // while(true)

            if(useNvidiaGpu)
            {
                int result;

                result = libSp.DeviceReset();

                if (result != 0)
                {
                    return false;
                }
            }

            LogAdd("学習処理が完了しました。");

            if (messageBox)
            {
                MessageBox.Show("学習処理が完了しました。", "メッセージ",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return true;
        }

        /// <summary>
        /// テストモード
        /// </summary>
        public void TestMode()
        {
            NeuralNet neuralNetMaru;  // ○○らしさを出力するニューラルネットワーク
            List<int> middleNeuronNums = new List<int>();  // 中間層のニューロン数のリスト
            float thresholdVal = 0; // 出力値（○○らしさ）がこの値以上ならば、○○と判定する。
            string judgmentResult;  // 判定結果
            string judgmentResultVal;  // 判定結果(実際にデータに書き込む値)
            DataTable dtTestResult = new DataTable();

            stopFlag = false;

            LogAdd("テストモードを実行します。");

            dtTestResult.Columns.Add(idColName);
            dtTestResult.Columns.Add(correctColName);

            LogAdd("");
            LogAdd("Test Data(" + testDataNum + "個):");

            LogAddDataTable(dtTest); // テスト用データの内容表示

            // 学習済みのニューラルネットワークをファイルから読み込んで、再現する。
            neuralNetMaru = new NeuralNet(modelFolderPath);

            // テスト用データの各行のデータの○○らしさを出力する。
            LogAdd("");

            if (aiType == EnAiType.TITANIC)
            {
                LogAdd("Result(" + judgmentResultStrPosi + "):");
            }
            else if(aiType == EnAiType.HOUSE_PRICES)
            {
                LogAdd("Result:");
            }
            else
            {
                // ここには来ないはず
            }

            for (int r = 0; r < dtTest.Rows.Count; r++)
            {
                List<float> inputValue;  // 入力層に渡すパラメータのリスト
                float outputValue = 0;  // 出力値（○○らしさ）

                if (stopFlag)
                {
                    break;
                }

                inputValue = Config.MakeInputValue(dtPlusTestTrain, r);

                // 入力層に4つのパラメータを渡す
                neuralNetMaru.SetInputValue(inputValue);

                // 出力値（○○らしさ）を計算する
                neuralNetMaru.ComputeOutputValue();

                // 出力値（○○らしさ）を取得する
                outputValue = neuralNetMaru.GetOutputValue();

                if (aiType == EnAiType.TITANIC)
                {
                    // 正解か不正解か判定
                    if (outputValue >= thresholdVal)
                    {
                        // AIは、○○と判定
                        judgmentResult = judgmentResultStrPosi;
                        judgmentResultVal = judgmentResultPosi;
                    }
                    else
                    {
                        // AIは、○○ではないと判定
                        judgmentResult = judgmentResultStrNega;
                        judgmentResultVal = judgmentResultNega;
                    }

                    dtTestResult.Rows.Add("" + dtTest.Rows[r][idColName], judgmentResultVal);

                    // テスト用データの各行のId、○○らしさ、
                    // 判定結果、正解か不正解かを表示する
                    LogAdd("" + dtTest.Rows[r][idColName] + " | "
                        + outputValue + " | 判定結果 : " + judgmentResult);
                }
                else if(aiType == EnAiType.HOUSE_PRICES)
                {
                    judgmentResultVal = "" + outputValue;

                    dtTestResult.Rows.Add("" + dtTest.Rows[r][idColName], judgmentResultVal);

                    // テスト用データの各行のId、テスト結果を表示する
                    LogAdd("" + dtTest.Rows[r][idColName] + " | "
                        + outputValue);
                }
                else
                {
                    // ここには来ないはず
                }
            }

            // 正解率（判定精度）計算
            //accuracyRate = (float)correctNum * 100 / predictDataNum;

            // 正解率（判定精度）、正解数、テスト用データの個数 を表示
            LogAdd("");
            //libLog.WriteLine("正解率（判定精度）: "
            //    + accuracyRate + " % (" + correctNum + "/" + predictDataNum + ")");

            Config.ResultDataProcessing(dtTestResult);

            libCsv.Write(dtTestResult, testResultFolderPath + @"/test_result.csv", true);
        }

        /// <summary>
        /// 予測モード
        /// </summary>
        public void PredictMode()
        {
            NeuralNet neuralNetMaru;  // ○○らしさを出力するニューラルネットワーク
            List<int> middleNeuronNums = new List<int>();  // 中間層のニューロン数のリスト
            float thresholdVal = 0; // 出力値（○○らしさ）がこの値以上ならば、○○と判定する。
            string judgmentResult;  // 判定結果
            string correctOrIncorrect;  // 正解か不正解か
            int correctNum = 0;     // 正解数
            float accuracyRate;     // 正解率（判定精度）
            float calcError;        // 誤差（判定精度）
            List<float> correctValueList;  // 正解の値のリスト
            List<float> predictValueList;  // 予測値のリスト

            stopFlag = false;

            LogAdd("予測モードを実行します。");

            LogAdd("");
            LogAdd("Predict Data(" + predictDataNum + "個):");

            LogAddDataTable(dtPredict); // 予測用データの内容表示

            {
                correctValueList = new List<float>();

                for (int r = 0; r < dtPredict.Rows.Count; r++)
                {
                    correctValueList.Add(float.Parse("" + dtPredict.Rows[r][correctColName]));
                }
            }

            // 学習済みのニューラルネットワークをファイルから読み込んで、再現する。
            neuralNetMaru = new NeuralNet(modelFolderPath);

            // 予測用データの各行のデータの○○らしさを出力する。
            LogAdd("");

            if (aiType == EnAiType.TITANIC)
            {
                LogAdd("Result(" + judgmentResultStrPosi + "):");
            }
            else if (aiType == EnAiType.HOUSE_PRICES)
            {
                LogAdd("Result:");
            }
            else
            {
                // ここには来ないはず
            }

            predictValueList = new List<float>();

            for (int r = 0; r < dtPredict.Rows.Count; r++)
            {
                List<float> inputValue;  // 入力層に渡すパラメータのリスト
                float outputValue = 0;  // 出力値（○○らしさ）

                if (stopFlag)
                {
                    break;
                }

                inputValue = Config.MakeInputValue(dtPlusPredictTest, r);

                // 入力層に4つのパラメータを渡す
                neuralNetMaru.SetInputValue(inputValue);

                // 出力値（○○らしさ）を計算する
                neuralNetMaru.ComputeOutputValue();

                // 出力値（○○らしさ）を取得する
                outputValue = neuralNetMaru.GetOutputValue();

                if (aiType == EnAiType.TITANIC)
                {
                    // 正解か不正解か判定
                    if (outputValue >= thresholdVal)
                    {
                        // AIは、○○と判定
                        judgmentResult = judgmentResultStrPosi;

                        if (correctValueList[r] == posiValue)
                        {
                            // 正解
                            correctNum++;
                            correctOrIncorrect = "o";
                        }
                        else
                        {
                            // 不正解
                            correctOrIncorrect = "x";
                        }
                    }
                    else
                    {
                        // AIは、○○ではないと判定
                        judgmentResult = judgmentResultStrNega;

                        if (correctValueList[r] != posiValue)
                        {
                            // 正解
                            correctNum++;
                            correctOrIncorrect = "o";
                        }
                        else
                        {
                            // 不正解
                            correctOrIncorrect = "x";
                        }
                    }

                    // 予測用データの各行のId、正解の内容、○○らしさ、
                    // 判定結果、正解か不正解かを表示する
                    LogAdd("" + dtPredict.Rows[r][idColName] + "(" + dtPredict.Rows[r][correctColName] + ") | "
                        + outputValue + " | 判定結果 : " + judgmentResult + " => " + correctOrIncorrect);
                }
                else if (aiType == EnAiType.HOUSE_PRICES)
                {
                    predictValueList.Add(outputValue);

                    // 予測用データの各行のId、正解の内容、○○らしさ、
                    // 判定結果、正解か不正解かを表示する
                    LogAdd("" + dtPredict.Rows[r][idColName] + "(" + dtPredict.Rows[r][correctColName] + ") | "
                        + outputValue + " | 誤差 : " + Math.Abs(outputValue - float.Parse("" + dtPredict.Rows[r][correctColName])));
                }
                else
                {
                    // ここには来ないはず
                }
            }

            if (aiType == EnAiType.TITANIC)
            {
                // 正解率（判定精度）計算
                accuracyRate = (float)correctNum * 100 / predictDataNum;

                // 正解率（判定精度）、正解数、予測用データの個数 を表示
                LogAdd("");
                LogAdd("正解率（判定精度）: "
                    + accuracyRate + " % (" + correctNum + "/" + predictDataNum + ")");
            }
            else if(aiType == EnAiType.HOUSE_PRICES)
            {
                calcError = libMath.GetRmse(predictValueList, correctValueList);

                // 誤差（判定精度）、正解数、予測用データの個数 を表示
                LogAdd("誤差（判定精度）: "
                    + calcError);
            }
            else
            {
                // ここには来ないはず
            }
        }

        public static float GetFloat(DataTable dtTrain, int row, string colName, float naNum = 0f)
        {
            float val;

            if ("" + dtTrain.Rows[row][colName] == "NA")
            {
                val = naNum;
            }
            else
            {
                val = float.Parse("" + dtTrain.Rows[row][colName]);
            }

            return val;
        }

        public static void InputValueAddNum(List<float> inputValue, DataTable dtTrain, int row, string colName, float divNum, float naNum = 0f)
        {
            if ("" + dtTrain.Rows[row][colName] == "NA")
            {
                inputValue.Add(naNum / divNum);
            }
            else
            {
                inputValue.Add(float.Parse("" + dtTrain.Rows[row][colName]) / divNum);
            }
        }

        public static float InputValueAddSumNums(List<float> inputValue, DataTable dtTrain, int row, List<string> colNameList, float divNum, float naNum = 0f)
        {
            float sum = 0f;
            string colName;
            float val;

            for (int i = 0; i < colNameList.Count; i++)
            {
                colName = colNameList[i];

                if ("" + dtTrain.Rows[row][colName] == "NA")
                {
                    sum += naNum;
                }
                else
                {
                    sum += float.Parse("" + dtTrain.Rows[row][colName]);
                }
            }

            val = sum / divNum;

            inputValue.Add(val);

            return val;
        }

        public static void InputValueAddGreaterZero(List<float> inputValue, DataTable dtTrain, int row, string colName, float naNum = 0f)
        {
            float val;

            if ("" + dtTrain.Rows[row][colName] == "NA")
            {
                val = naNum;
            }
            else
            {
                val = float.Parse("" + dtTrain.Rows[row][colName]);
            }

            if(val > 0)
            {
                inputValue.Add(1.0f);
            }
            else
            {
                inputValue.Add(0);
            }
        }

        public static void InputValueAddString(List<float> inputValue, DataTable dtTrain, int row, string colName, List<string> strList)
        {
            string str;
            float val;
            int count;
            float unit;
            int idx;

            str = "" + dtTrain.Rows[row][colName];

            count = strList.Count;

            unit = 1.0f / (count - 1);

            idx = strList.IndexOf(str);

            val = unit * idx;

            inputValue.Add(val);
        }

        public static void InputValueAddCategoryManual(List<float> inputValue, DataTable dtTrain, int row, string colName, List<string> values)
        {
            string val;

            val = "" + dtTrain.Rows[row][colName];

            for(int i=0; i<values.Count; i++)
            {
                if(values.Contains(val) == false)
                {
                    MessageBox.Show(colName + " invalid. (values.Contains(\"" + val + "\") == false)");

                    inputValue.Add(0f);
                }

                if (val == values[i])
                {
                    inputValue.Add(1.0f);
                }
                else
                {
                    inputValue.Add(0.0f);
                }
            }
        }

        public static void InputValueAddCategory(List<float> inputValue, DataTable dtTrain, int row, string colName)
        {
            List<string> values = new List<string>();
            string val;

            for(int r=0; r<dtTrain.Rows.Count; r++)
            {
                val = "" + dtTrain.Rows[r][colName];

                if(values.Contains(val) == false)
                {
                    values.Add(val);
                }
            }

            InputValueAddCategoryManual(inputValue, dtTrain, row, colName, values);
        }
    }
}

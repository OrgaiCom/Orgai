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

using System;
using System.Collections.Generic;
using System.Data;
using CommonClass;

namespace orgai
{
    public class NeuralNet
    {
        public int inputNeuronNum;           // 入力層のニューロンの数
        public List<int> middleNeuronNums;   // 中間層のニューロンの数のリスト
        public int outputNeuronNum;          // 出力層のニューロンの数

        public InputLayer inputLayer;               // 入力層
        public List<MiddleLayerOne> middleLayer;    // 中間層
        public Neuron outputLayer;                  // 出力層

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="inputNeuronNum">入力層のニューロンの数</param>
        /// <param name="middleNeuronNums">中間層のニューロンの数のリスト</param>
        public NeuralNet(int inputNeuronNum, List<int> middleNeuronNums)
        {
            // サブルーチン呼び出し
            NeuralNetSub(inputNeuronNum, middleNeuronNums);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="inputNeuronNum">入力層のニューロンの数</param>
        /// <param name="middleNeuronNums">中間層のニューロンの数のリスト</param>
        /// <param name="baseNN">ベースとなるニューラルネットワーク</param>
        /// <param name="derivationRate">派生率（0 ～ 1.0）(大きいほどベースのニューラルネットワークに似ている)</param>
        public NeuralNet(int inputNeuronNum, List<int> middleNeuronNums, NeuralNet baseNN, float derivationRate)
        {
            // サブルーチン呼び出し
            NeuralNetSub(inputNeuronNum, middleNeuronNums, baseNN, derivationRate);
        }

        /// <summary>
        /// コンストラクタ
        /// ファイルからニューラルネットワークのデータを読み込んで再現します。
        /// </summary>
        /// <param name="folderPath">ニューラルネットワークのデータが保存されているフォルダ</param>
        public NeuralNet(string folderPath)
        {
            LibCsv libCsv = new LibCsv();  // csvライブラリ
            int inputNeuronNum;  // 入力層のニューロンの数
            List<int> middleNeuronNums = new List<int>();  // 中間層のニューロン数のリスト
            DataTable dtStructure;  // NeuralNetStructure.csv の内容を格納するDataTable
            DataTable dtWeights;    // NeuralNetWeights.csv の内容を格納するDataTable

            // NeuralNetStructure.csv を読み込み
            dtStructure = libCsv.Read(folderPath + @"/NeuralNetStructure.csv", true);

            // 入力層のニューロンの数を設定
            inputNeuronNum = int.Parse("" + dtStructure.Rows[0][0]);

            // 中間層のニューロンの数を設定
            for(int r = 2; r < dtStructure.Rows.Count; r++)
            {
                string neuronNum;

                neuronNum = "" + dtStructure.Rows[r][0];

                if(neuronNum == "--")
                {
                    break;
                }

                middleNeuronNums.Add(int.Parse(neuronNum));
            }

            // コンストラクタのサブルーチン呼び出し
            // （出力層のニューロンは1個で固定。）
            NeuralNetSub(inputNeuronNum, middleNeuronNums);

            {
                int idx = 0;  // dtWeightsを参照する行のインデックス

                // NeuralNetWeights.csv を読み込み
                dtWeights = libCsv.Read(folderPath + @"/NeuralNetWeights.csv", true);

                // 入力層の重み設定
                for (int i = 0; i < inputNeuronNum; i++)
                {
                    inputLayer.neurons[i].wVal[0] = float.Parse("" + dtWeights.Rows[idx][0]);

                    idx += 2;
                }

                // 中間層の重み設定
                for(int i = 0; i < middleNeuronNums.Count; i++)
                {
                    for(int j = 0; j < middleNeuronNums[i]; j++)
                    {
                        for(int k = 0; k < middleLayer[i].neurons[j].wVal.Count; k++)
                        {
                            middleLayer[i].neurons[j].wVal[k] = float.Parse("" + dtWeights.Rows[idx][0]);

                            idx++;
                        }

                        idx++;
                    }
                }

                // 出力層の重み設定
                for(int i = 0; i < outputLayer.wVal.Count; i++)
                {
                    outputLayer.wVal[i] = float.Parse("" + dtWeights.Rows[idx][0]);

                    idx++;
                }
            }
        }

        /// <summary>
        /// コンストラクタから呼び出すサブルーチン
        /// </summary>
        /// <param name="inputNeuronNum">入力層のニューロンの数</param>
        /// <param name="middleNeuronNums">中間層のニューロンの数のリスト</param>
        void NeuralNetSub(int inputNeuronNum, List<int> middleNeuronNums)
        {
            int previousNeuronNum;  // 前列のニューロンの数

            this.inputNeuronNum = inputNeuronNum;
            this.middleNeuronNums = middleNeuronNums;
            this.outputNeuronNum = 1;   // 出力層はとりあえず、1つで試してみる

            // 入力層作成
            inputLayer = new InputLayer(this.inputNeuronNum);

            previousNeuronNum = this.inputNeuronNum;

            // 中間層作成
            middleLayer = new List<MiddleLayerOne>();

            for (int i = 0; i < middleNeuronNums.Count; i++)
            {
                // 中間層の1列を作成
                MiddleLayerOne middleLayerOne = new MiddleLayerOne(previousNeuronNum, middleNeuronNums[i]);

                previousNeuronNum = middleNeuronNums[i];

                // 中間層の1列を中間層に追加
                middleLayer.Add(middleLayerOne);
            }

            // 出力層作成
            outputLayer = new Neuron(previousNeuronNum);
        }

        /// <summary>
        /// コンストラクタから呼び出すサブルーチン
        /// </summary>
        /// <param name="inputNeuronNum">入力層のニューロンの数</param>
        /// <param name="middleNeuronNums">中間層のニューロンの数のリスト</param>
        /// <param name="baseNN">ベースとなるニューラルネットワーク</param>
        /// <param name="derivationRate">派生率（0 ～ 1.0）(大きいほどベースのニューラルネットワークに似ている)</param>
        void NeuralNetSub(int inputNeuronNum, List<int> middleNeuronNums, NeuralNet baseNN, float derivationRate)
        {
            int previousNeuronNum;  // 前列のニューロンの数

            this.inputNeuronNum = inputNeuronNum;
            this.middleNeuronNums = middleNeuronNums;
            this.outputNeuronNum = 1;   // 出力層はとりあえず、1つで試してみる

            // 入力層作成
            inputLayer = new InputLayer(this.inputNeuronNum, baseNN.inputLayer, derivationRate);

            previousNeuronNum = this.inputNeuronNum;

            // 中間層作成
            middleLayer = new List<MiddleLayerOne>();

            for (int i = 0; i < middleNeuronNums.Count; i++)
            {
                // 中間層の1列を作成
                MiddleLayerOne middleLayerOne = new MiddleLayerOne(previousNeuronNum, middleNeuronNums[i],
                    baseNN.middleLayer[i], derivationRate);

                previousNeuronNum = middleNeuronNums[i];

                // 中間層の1列を中間層に追加
                middleLayer.Add(middleLayerOne);
            }

            // 出力層作成
            outputLayer = new Neuron(previousNeuronNum, baseNN.outputLayer, derivationRate);
        }

        /// <summary>
        /// 入力層にパラメータをセットする
        /// </summary>
        /// <param name="inputValue">セットするパラメータのリスト</param>
        public void SetInputValue(List<float> inputValue)
        {
            for (int i = 0; i < inputValue.Count; i++)
            {
                // 入力層の各ニューロンの x にパラメータをセットする
                inputLayer.neurons[i].xVal[0] = inputValue[i];
            }
        }

        /// <summary>
        /// 出力値を取得する
        /// </summary>
        /// <returns>ニューラルネットワークの出力値</returns>
        public float GetOutputValue()
        {
            // 出力層のニューロンの y の値を返す
            return outputLayer.yVal;
        }

        /// <summary>
        /// 出力値を計算する
        /// </summary>
        public void ComputeOutputValue()
        {
            // 入力層の各ニューロンの y の値を計算する。
            for (int r = 0; r < inputLayer.neuronNum; r++)
            {
                inputLayer.neurons[r].ComputeYVal();
            }

            // 中間層の各ニューロンに前列から y の値を伝達させつつ、
            // 各ニューロンの y の値を計算する。
            for (int c = 0; c < middleLayer.Count; c++)
            {
                if (c == 0)
                {
                    for (int r = 0; r < middleLayer[c].neuronNum; r++)
                    {
                        for (int x = 0; x < middleLayer[c].neurons[r].dendriteNum; x++)
                        {
                            middleLayer[c].neurons[r].xVal[x] = inputLayer.neurons[x].yVal;
                        }

                        middleLayer[c].neurons[r].ComputeYVal();
                    }
                }
                else
                {
                    for (int r = 0; r < middleLayer[c].neuronNum; r++)
                    {
                        for (int x = 0; x < middleLayer[c].neurons[r].dendriteNum; x++)
                        {
                            middleLayer[c].neurons[r].xVal[x] = middleLayer[c - 1].neurons[x].yVal;
                        }

                        middleLayer[c].neurons[r].ComputeYVal();
                    }
                }
            }

            // 中間層の最後の列から y の値を出力層のニューロンに伝達させる。
            for (int x = 0; x < outputLayer.dendriteNum; x++)
            {
                outputLayer.xVal[x] = middleLayer[middleLayer.Count - 1].neurons[x].yVal;
            }

            // 出力層のニューロンの y の値を計算する。
            outputLayer.ComputeYVal();
        }

        /// <summary>
        /// ニューラルネットワークをファイルに保存する。
        /// </summary>
        /// <param name="folderPath">保存先のフォルダ</param>
        public void Save(string folderPath)
        {
            LibCsv libCsv = new LibCsv();  // csvライブラリ

            // NeuralNetStructure.csv を出力する。
            {
                DataTable dtNNStructure = new DataTable();

                dtNNStructure.Columns.Add("各層のニューロン数");

                // 入力層のニューロン数を追加
                dtNNStructure.Rows.Add("" + inputNeuronNum);

                dtNNStructure.Rows.Add("--");

                // 中間層のニューロン数を追加
                for (int i = 0; i < middleNeuronNums.Count; i++)
                {
                    dtNNStructure.Rows.Add("" + middleNeuronNums[i]);
                }

                dtNNStructure.Rows.Add("--");

                // 出力層のニューロン数を追加
                dtNNStructure.Rows.Add("" + outputNeuronNum);

                // ファイルに出力
                libCsv.Write(dtNNStructure, folderPath + @"/NeuralNetStructure.csv", true);
            }

            // NeuralNetWeights.csv を出力する。
            {
                DataTable dtNNWeights = new DataTable();

                dtNNWeights.Columns.Add("各ニューロンの結合強度");

                // 入力層の各ニューロンの結合強度を追加
                for (int i = 0; i < inputLayer.neurons.Count; i++)
                {
                    dtNNWeights.Rows.Add("" + inputLayer.neurons[i].wVal[0]);

                    if (i == inputLayer.neurons.Count - 1)
                    {
                        dtNNWeights.Rows.Add("--");
                    }
                    else
                    {
                        dtNNWeights.Rows.Add("-");
                    }
                }

                // 中間層の各ニューロンの結合強度を追加
                for (int i = 0; i < middleLayer.Count; i++)
                {
                    for(int j = 0; j < middleLayer[i].neurons.Count; j++)
                    {
                        for (int k = 0; k < middleLayer[i].neurons[j].wVal.Count; k++)
                        {
                            dtNNWeights.Rows.Add("" + middleLayer[i].neurons[j].wVal[k]);
                        }

                        if (j == middleLayer[i].neurons.Count - 1)
                        {
                            dtNNWeights.Rows.Add("--");
                        }
                        else
                        {
                            dtNNWeights.Rows.Add("-");
                        }
                    }
                }

                // 出力層のニューロンの結合強度を追加
                for (int i = 0; i < outputLayer.wVal.Count; i++)
                {
                    dtNNWeights.Rows.Add("" + outputLayer.wVal[i]);
                }

                // ファイルに出力
                libCsv.Write(dtNNWeights, folderPath + @"/NeuralNetWeights.csv", true);
            }
        }
    }
}

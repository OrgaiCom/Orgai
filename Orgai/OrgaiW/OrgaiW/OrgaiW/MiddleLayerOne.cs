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

namespace orgai
{
    public class MiddleLayerOne
    {
        public int previousNeuronNum;  // 前列のニューロンの数
        public int neuronNum;  // ニューロンの数
        public List<Neuron> neurons = new List<Neuron>();  // ニューロンのリスト

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="previousNeuronNum">前列のニューロンの数</param>
        /// <param name="neuronNum">入力層のニューロンの数</param>
        public MiddleLayerOne(int previousNeuronNum, int neuronNum)
        {
            Neuron neuron;

            this.previousNeuronNum = previousNeuronNum;
            this.neuronNum = neuronNum;

            // ニューロンのリスト作成
            for (int i = 0; i < neuronNum; i++)
            {
                neuron = new Neuron(previousNeuronNum);  // 樹状突起の数は前列のニューロンの数と同じにする。

                neurons.Add(neuron);
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="previousNeuronNum">前列のニューロンの数</param>
        /// <param name="neuronNum">入力層のニューロンの数</param>
        /// <param name="baseMiddleLayerOne">ベースとなるMiddleLayerOne</param>
        /// <param name="derivationRate">派生率（0 ～ 1.0）(大きいほどベースのMiddleLayerOneに似ている)</param>
        public MiddleLayerOne(int previousNeuronNum, int neuronNum, MiddleLayerOne baseMiddleLayerOne, float derivationRate)
        {
            Neuron neuron;

            this.previousNeuronNum = previousNeuronNum;
            this.neuronNum = neuronNum;

            // ニューロンのリスト作成
            for (int i = 0; i < neuronNum; i++)
            {
                neuron = new Neuron(previousNeuronNum, 
                    baseMiddleLayerOne.neurons[i], derivationRate);  // 樹状突起の数は前列のニューロンの数と同じにする。

                neurons.Add(neuron);
            }
        }
    }
}

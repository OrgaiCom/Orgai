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
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;

namespace orgai
{
    public class Neuron
    {
        public float wMin;  // w* の値の最小値
        public float wMax; // w* の値の最大値

        public int dendriteNum;  // 樹状突起の数
        public List<float> xVal = new List<float>(); // x* の値のリスト
        public List<float> wVal = new List<float>(); // w* の値のリスト
        public float yVal;  // y の値

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dendriteNum">樹状突起の数</param>
        public Neuron(int dendriteNum)
        {
            //Random rnd = new Random();

            wMin = Orgai.wMin; 
            wMax = Orgai.wMax;
            
            // 樹状突起の数を設定
            this.dendriteNum = dendriteNum;

            // x*, w* の値を設定。
            for(int i=0; i<dendriteNum; i++)
            {
                // x* の値は0に初期化しておく
                xVal.Add(0);

                // w* の値は、wMin 〜 wMax の乱数としておく
                wVal.Add((float)(wMin + LibRandom.NextDouble() * (wMax - wMin)));
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dendriteNum">樹状突起の数</param>
        /// <param name="baseNeuron">ベースとなるニューロン</param>
        /// <param name="derivationRate">派生率（0 ～ 1.0）(大きいほどベースのニューロンに似ている)</param>
        public Neuron(int dendriteNum, Neuron baseNeuron, float derivationRate)
        {
            wMin = Orgai.wMin;
            wMax = Orgai.wMax;

            // 樹状突起の数を設定
            this.dendriteNum = dendriteNum;

            // x*, w* の値を設定。
            for (int i = 0; i < dendriteNum; i++)
            {
                // x* の値は0に初期化しておく
                xVal.Add(0);

                if (LibRandom.NextDouble() < derivationRate)
                {
                    wVal.Add(baseNeuron.wVal[i]);
                }
                else
                {
                    // w* の値は、wMin 〜 wMax の乱数としておく
                    wVal.Add((float)(wMin + LibRandom.NextDouble() * (wMax - wMin)));
                }
            }
        }

        /// <summary>
        /// y の値を計算する。
        /// </summary>
        public void ComputeYVal()
        {
            yVal = 0;

            // ｙ ＝ ｘ0ｗ0 ＋ ｘ1ｗ1 ＋ ｘ2ｗ2 ＋ ｘ3ｗ3
            for (int i=0; i < dendriteNum; i++)
            {
                yVal += xVal[i] * wVal[i];
            }
        }
    }
}

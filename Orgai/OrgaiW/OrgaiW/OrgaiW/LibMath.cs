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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClass
{
    public class LibMath
    {
        /// <summary>
        /// 誤差RMSE(Root Mean Square Error)を求める
        /// </summary>
        /// <param name="predictValues">予測した値のリスト</param>
        /// <param name="correctValues">正解の値のリスト</param>
        /// <returns>float | 誤差</returns>
        public float GetRmse(List<float> predictValues, List<float> correctValues)
        {
            if (predictValues.Count != correctValues.Count)
            {
                throw new ArgumentException("predictValues and correctValues must have the same length.");
            }

            float sum = 0;
            
            for (int i = 0; i < predictValues.Count; i++)
            {
                sum += (float)Math.Pow(predictValues[i] - correctValues[i], 2);
            }
            
            return (float)Math.Sqrt(sum / predictValues.Count);
        }

        /// <summary>
        /// 誤差RMSLE(Root Mean Square Log Error)を求める
        /// ※本当は predictValues は >= 0 でないと駄目
        /// </summary>
        /// <param name="predictValues">予測した値のリスト</param>
        /// <param name="correctValues">正解の値のリスト</param>
        /// <returns>float | 誤差</returns>
        public float GetRmsle(List<float> predictValues, List<float> correctValues)
        {
            if (predictValues.Count != correctValues.Count)
            {
                throw new ArgumentException("predictValues and correctValues must have the same length.");
            }

            float sum = 0;

            for (int i = 0; i < predictValues.Count; i++)
            {
                float pVal;
                float cVal;

                pVal = predictValues[i];
                cVal = correctValues[i];

                if(pVal < 0)
                {
                    // 本当は pVal は >= 0 でないと駄目
                    cVal = cVal - pVal;
                    pVal = 0;
                }

                sum += (float)Math.Pow(Math.Log(pVal + 1f) - Math.Log(cVal + 1f), 2);
            }

            return (float)Math.Sqrt(sum / predictValues.Count);
        }
    }
}

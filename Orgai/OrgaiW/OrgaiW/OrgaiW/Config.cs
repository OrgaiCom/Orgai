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

using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace orgai
{
    public class Config
    {
        static Orgai orgai;

        public static void InitOrgai(Orgai orgAi, TextBox textBoxLog, Queue<string> commandQueue, Label labelBest, Label labelBestValue)
        {
            orgai = orgAi;

            orgai.Init(textBoxLog, commandQueue);

            orgai.useNvidiaGpu = true;  // true:NvidiaGPUを使用する   false:CPUで処理する

            // TITANIC または HOUSE_PRICES を選択
            orgai.aiType = Orgai.EnAiType.TITANIC;
            //orgai.aiType = Orgai.EnAiType.HOUSE_PRICES;

            if (orgai.aiType == Orgai.EnAiType.TITANIC)
            {
                labelBest.Text = "最高正解率" + ":";

                orgai.bestUnit = "%";

                labelBestValue.Text = "0 " + orgai.bestUnit;

                // train.csv, test.csv は kaggleサイトより入手できます。
                orgai.trainDataPath = @"../../../../../Titanic/train.csv";
                orgai.modelFolderPath = @"../../../../../Titanic/model";

                orgai.testDataPath = @"../../../../../Titanic/test.csv";
                orgai.testResultFolderPath = @"../../../../../Titanic";

                orgai.predictDataPath = @"../../../../../Titanic/train.csv";

                // 入力層のニューロンは12個(Auto)
                // 中間層のニューロンは34 ～ 10個×8列とする。
                // （出力層のニューロンは1個で固定。）
                orgai.inputNeuronNum = -1;  // Auto

                orgai.middleNeuronNums.Add(34);
                orgai.middleNeuronNums.Add(28);
                orgai.middleNeuronNums.Add(28);
                orgai.middleNeuronNums.Add(22);
                orgai.middleNeuronNums.Add(22);
                orgai.middleNeuronNums.Add(16);
                orgai.middleNeuronNums.Add(16);
                orgai.middleNeuronNums.Add(10);

                orgai.outputNeuronNum = 1;

                orgai.neuralNetworkNum = 420;
                orgai.betterNeuralNetworkNum = 105;

                orgai.geneMax = 38;

                Orgai.wMin = -5f;
                Orgai.wMax = 5f;

                orgai.derivationRate = 0.97f;  // この割合だけ 派生元のパラメータを継承する

                orgai.idColName = "PassengerId";

                orgai.correctColName = "Survived";

                {
                    orgai.posiValue = 1f;

                    orgai.judgmentResultStrPosi = "survived.";
                    orgai.judgmentResultPosi = "1";

                    orgai.judgmentResultStrNega = "not survived.";
                    orgai.judgmentResultNega = "0";
                }
            }
            else if (orgai.aiType == Orgai.EnAiType.HOUSE_PRICES)
            {
                labelBest.Text = "Best誤差" + ":";

                orgai.bestUnit = "";

                labelBestValue.Text = "0 " + orgai.bestUnit;

                // train.csv, test.csv は kaggleサイトより入手できます。
                orgai.trainDataPath = @"../../../../../HousePrices/train.csv";
                orgai.modelFolderPath = @"../../../../../HousePrices/model";

                orgai.testDataPath = @"../../../../../HousePrices/test.csv";
                orgai.testResultFolderPath = @"../../../../../HousePrices";

                orgai.predictDataPath = @"../../../../../HousePrices/train.csv";

                // 入力層のニューロン数はAutoで決定する。
                // 中間層のニューロンは20 ～ 10個×3列とする。
                // （出力層のニューロンは1個で固定。）
                orgai.inputNeuronNum = -1;  // Auto

                orgai.middleNeuronNums.Add(20);
                orgai.middleNeuronNums.Add(15);
                orgai.middleNeuronNums.Add(10);

                orgai.outputNeuronNum = 1;

                orgai.neuralNetworkNum = 420;
                orgai.betterNeuralNetworkNum = 105;

                orgai.geneMax = 95;

                Orgai.wMin = -5f;
                Orgai.wMax = 5f;

                orgai.derivationRate = 0.97f;  // この割合だけ 派生元のパラメータを継承する

                orgai.idColName = "Id";

                orgai.correctColName = "SalePrice";
            }
            else
            {
                // ここには来ないはず
            }
        }

        public static void DataProcessing(DataTable dtTrain, DataTable dtTest, DataTable dtPredict)
        {
            if (orgai.aiType == Orgai.EnAiType.TITANIC)
            {
            }
            else if (orgai.aiType == Orgai.EnAiType.HOUSE_PRICES)
            {
                for (int r = dtTrain.Rows.Count - 1; r >= 0; r--)
                {
                    float val;

                    val = float.Parse("" + dtTrain.Rows[r]["LotArea"]);

                    if (val >= 25000)
                    {
                        dtTrain.Rows.RemoveAt(r);
                        dtPredict.Rows.RemoveAt(r);
                    }

                    val = float.Parse("" + dtTrain.Rows[r]["SalePrice"]);

                    if (val >= 450000)
                    {
                        dtTrain.Rows.RemoveAt(r);
                        dtPredict.Rows.RemoveAt(r);
                    }

                    val = float.Parse("" + dtTrain.Rows[r]["YearBuilt"]);

                    if (val <= 1900)
                    {
                        dtTrain.Rows.RemoveAt(r);
                        dtPredict.Rows.RemoveAt(r);
                    }
                }
            }
            else
            {
                // ここには来ないはず
            }
        }

        public static void ResultDataProcessing(DataTable dtResult)
        {
            if (orgai.aiType == Orgai.EnAiType.TITANIC)
            {
            }
            else if (orgai.aiType == Orgai.EnAiType.HOUSE_PRICES)
            {
#if false
            for (int r = 0; r < dtResult.Rows.Count; r++)
            {
                float val;

                val = float.Parse("" + dtResult.Rows[r][orgai.correctColName]);

                val /= 100000f;

                val = (float)(Math.Exp(val));

                dtResult.Rows[r][orgai.correctColName] = "" + val;
            }
#endif
            }
            else
            {
                // ここには来ないはず
            }
        }

        public static List<float> MakeInputValue(DataTable dtPlus, int row)
        {
            List<float> inputValue = new List<float>();
            float val;

            if (orgai.aiType == Orgai.EnAiType.TITANIC)
            {
                inputValue.Add(float.Parse("" + dtPlus.Rows[row]["Pclass"]));

#if false
            {
                string name;

                name = "" + dtPlus.Rows[row]["Name"];

                if (name.Contains("Mr."))
                {
                    inputValue.Add(0.0f);
                }
                else if (name.Contains("Capt.")
                    || name.Contains("Col.")
                    || name.Contains("Major.")
                    || name.Contains("Dr.")
                    || name.Contains("Rev.")
                    )
                {
                    inputValue.Add(1.0f);
                }
                else if (name.Contains("Master.")
                    || name.Contains("Jonkheer.")
                    )
                {
                    inputValue.Add(2.0f);
                }
                else if (name.Contains("Mlle.")
                    || name.Contains("Miss.")
                    )
                {
                    inputValue.Add(3.0f);
                }
                else if (name.Contains("Don.")
                    || name.Contains("Sir.")
                    || name.Contains("the Countess")
                    || name.Contains("Dona.")
                    || name.Contains("Lady.")
                    )
                {
                    inputValue.Add(4.0f);
                }
                else if (name.Contains("Mme.")
                    || name.Contains("Ms.")
                    || name.Contains("Mrs.")
                    )
                {
                    inputValue.Add(5.0f);
                }
                else
                {
                    MessageBox.Show("Name invalid: " + name);

                    inputValue.Add(0f);
                }
            }
#endif

                {
                    string sex;

                    sex = "" + dtPlus.Rows[row]["Sex"];

                    if (sex == "male")
                    {
                        inputValue.Add(1.0f);
                    }
                    else if (sex == "female")
                    {
                        inputValue.Add(0.0f);
                    }
                    else
                    {
                        MessageBox.Show("Sex invalid");

                        inputValue.Add(0f);
                    }
                }

                {
                    string sex;

                    sex = "" + dtPlus.Rows[row]["Sex"];

                    if (sex == "male")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (sex == "female")
                    {
                        inputValue.Add(1.0f);
                    }
                    else
                    {
                        MessageBox.Show("Sex invalid");

                        inputValue.Add(0f);
                    }
                }

                {
                    string age;

                    age = "" + dtPlus.Rows[row]["Age"];

                    if (age == "")
                    {
                        age = "29.881138";  // 中央値
                    }

                    inputValue.Add(float.Parse(age) / 10.0f);
                }

#if false  // 過学習を起こすので使用しない
            inputValue.Add(float.Parse("" + dtPlus.Rows[row]["SibSp"]));

            inputValue.Add(float.Parse("" + dtPlus.Rows[row]["Parch"]));
#endif

                {
                    int sibSp;
                    int parch;
                    int familySize;

                    sibSp = int.Parse("" + dtPlus.Rows[row]["SibSp"]);
                    parch = int.Parse("" + dtPlus.Rows[row]["Parch"]);

                    familySize = sibSp + parch + 1;

                    if (2 <= familySize
                        && familySize <= 4)
                    {
                        inputValue.Add(2.0f);
                    }
                    else if (familySize == 1
                        ||
                        (5 <= familySize
                        && familySize <= 7)
                        )
                    {
                        inputValue.Add(1.0f);
                    }
                    else if (8 <= familySize)
                    {
                        inputValue.Add(0.0f);
                    }
                    else
                    {
                        MessageBox.Show("FamilySize invalid");

                        inputValue.Add(0.0f);
                    }
                }

                {
                    string fare;

                    fare = "" + dtPlus.Rows[row]["Fare"];

                    if (fare == "")
                    {
                        fare = "33.295479";  // 中央値
                    }

                    inputValue.Add(float.Parse(fare) / 20.0f);
                }

                {
                    string cabin;
                    string cabin1st = "";

                    cabin = "" + dtPlus.Rows[row]["Cabin"];

                    if (cabin != "")
                    {
                        cabin1st = cabin.Substring(0, 1);
                    }

                    if (cabin1st == "")
                    {
                        inputValue.Add(1.0f);
                    }
                    else if (cabin1st == "A")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "B")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "C")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "D")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "E")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "F")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "G")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "T")
                    {
                        inputValue.Add(0.0f);
                    }
                    else
                    {
                        MessageBox.Show("Cabin invalid: " + cabin);

                        inputValue.Add(0f);
                    }
                }

                {
                    string cabin;
                    string cabin1st = "";

                    cabin = "" + dtPlus.Rows[row]["Cabin"];

                    if (cabin != "")
                    {
                        cabin1st = cabin.Substring(0, 1);
                    }

                    if (cabin1st == "")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "A")
                    {
                        inputValue.Add(1.0f);
                    }
                    else if (cabin1st == "B")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "C")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "D")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "E")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "F")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "G")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "T")
                    {
                        inputValue.Add(0.0f);
                    }
                    else
                    {
                        MessageBox.Show("Cabin invalid: " + cabin);

                        inputValue.Add(0f);
                    }
                }

                {
                    string cabin;
                    string cabin1st = "";

                    cabin = "" + dtPlus.Rows[row]["Cabin"];

                    if (cabin != "")
                    {
                        cabin1st = cabin.Substring(0, 1);
                    }

                    if (cabin1st == "")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "A")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "B")
                    {
                        inputValue.Add(1.0f);
                    }
                    else if (cabin1st == "C")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "D")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "E")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "F")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "G")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "T")
                    {
                        inputValue.Add(0.0f);
                    }
                    else
                    {
                        MessageBox.Show("Cabin invalid: " + cabin);

                        inputValue.Add(0f);
                    }
                }

                {
                    string cabin;
                    string cabin1st = "";

                    cabin = "" + dtPlus.Rows[row]["Cabin"];

                    if (cabin != "")
                    {
                        cabin1st = cabin.Substring(0, 1);
                    }

                    if (cabin1st == "")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "A")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "B")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "C")
                    {
                        inputValue.Add(1.0f);
                    }
                    else if (cabin1st == "D")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "E")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "F")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "G")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "T")
                    {
                        inputValue.Add(0.0f);
                    }
                    else
                    {
                        MessageBox.Show("Cabin invalid: " + cabin);

                        inputValue.Add(0f);
                    }
                }

                {
                    string cabin;
                    string cabin1st = "";

                    cabin = "" + dtPlus.Rows[row]["Cabin"];

                    if (cabin != "")
                    {
                        cabin1st = cabin.Substring(0, 1);
                    }

                    if (cabin1st == "")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "A")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "B")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "C")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "D")
                    {
                        inputValue.Add(1.0f);
                    }
                    else if (cabin1st == "E")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "F")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "G")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "T")
                    {
                        inputValue.Add(0.0f);
                    }
                    else
                    {
                        MessageBox.Show("Cabin invalid: " + cabin);

                        inputValue.Add(0f);
                    }
                }

                {
                    string cabin;
                    string cabin1st = "";

                    cabin = "" + dtPlus.Rows[row]["Cabin"];

                    if (cabin != "")
                    {
                        cabin1st = cabin.Substring(0, 1);
                    }

                    if (cabin1st == "")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "A")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "B")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "C")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "D")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "E")
                    {
                        inputValue.Add(1.0f);
                    }
                    else if (cabin1st == "F")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "G")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "T")
                    {
                        inputValue.Add(0.0f);
                    }
                    else
                    {
                        MessageBox.Show("Cabin invalid: " + cabin);

                        inputValue.Add(0f);
                    }
                }

                {
                    string cabin;
                    string cabin1st = "";

                    cabin = "" + dtPlus.Rows[row]["Cabin"];

                    if (cabin != "")
                    {
                        cabin1st = cabin.Substring(0, 1);
                    }

                    if (cabin1st == "")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "A")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "B")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "C")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "D")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "E")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "F")
                    {
                        inputValue.Add(1.0f);
                    }
                    else if (cabin1st == "G")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "T")
                    {
                        inputValue.Add(0.0f);
                    }
                    else
                    {
                        MessageBox.Show("Cabin invalid: " + cabin);

                        inputValue.Add(0f);
                    }
                }

                {
                    string cabin;
                    string cabin1st = "";

                    cabin = "" + dtPlus.Rows[row]["Cabin"];

                    if (cabin != "")
                    {
                        cabin1st = cabin.Substring(0, 1);
                    }

                    if (cabin1st == "")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "A")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "B")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "C")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "D")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "E")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "F")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "G")
                    {
                        inputValue.Add(1.0f);
                    }
                    else if (cabin1st == "T")
                    {
                        inputValue.Add(0.0f);
                    }
                    else
                    {
                        MessageBox.Show("Cabin invalid: " + cabin);

                        inputValue.Add(0f);
                    }
                }

                {
                    string cabin;
                    string cabin1st = "";

                    cabin = "" + dtPlus.Rows[row]["Cabin"];

                    if (cabin != "")
                    {
                        cabin1st = cabin.Substring(0, 1);
                    }

                    if (cabin1st == "")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "A")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "B")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "C")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "D")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "E")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "F")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "G")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (cabin1st == "T")
                    {
                        inputValue.Add(1.0f);
                    }
                    else
                    {
                        MessageBox.Show("Cabin invalid: " + cabin);

                        inputValue.Add(0f);
                    }
                }

                {
                    string embarked;

                    embarked = "" + dtPlus.Rows[row]["Embarked"];

                    if (embarked == "")
                    {
                        //embarked = "S";  // S が一番多い
                        embarked = "C";  // C が一番近い
                    }

                    if (embarked == "S")
                    {
                        inputValue.Add(1.0f);
                    }
                    else if (embarked == "C")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (embarked == "Q")
                    {
                        inputValue.Add(0.0f);
                    }
                    else
                    {
                        MessageBox.Show("Embarked invalid");

                        inputValue.Add(0f);
                    }
                }

                {
                    string embarked;

                    embarked = "" + dtPlus.Rows[row]["Embarked"];

                    if (embarked == "")
                    {
                        //embarked = "S";  // S が一番多い
                        embarked = "C";  // C が一番近い
                    }

                    if (embarked == "S")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (embarked == "C")
                    {
                        inputValue.Add(1.0f);
                    }
                    else if (embarked == "Q")
                    {
                        inputValue.Add(0.0f);
                    }
                    else
                    {
                        MessageBox.Show("Embarked invalid");

                        inputValue.Add(0f);
                    }
                }

                {
                    string embarked;

                    embarked = "" + dtPlus.Rows[row]["Embarked"];

                    if (embarked == "")
                    {
                        //embarked = "S";  // S が一番多い
                        embarked = "C";  // C が一番近い
                    }

                    if (embarked == "S")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (embarked == "C")
                    {
                        inputValue.Add(0.0f);
                    }
                    else if (embarked == "Q")
                    {
                        inputValue.Add(1.0f);
                    }
                    else
                    {
                        MessageBox.Show("Embarked invalid");

                        inputValue.Add(0f);
                    }
                }
            }
            else if (orgai.aiType == Orgai.EnAiType.HOUSE_PRICES)
            {
                Orgai.InputValueAddNum(inputValue, dtPlus, row, "LotArea", 40000);

                Orgai.InputValueAddNum(inputValue, dtPlus, row, "OverallQual", 10);

                Orgai.InputValueAddString(inputValue, dtPlus, row, "KitchenQual", new List<string>() { "Po", "Fa", "TA", "Gd", "Ex" });

                val = Orgai.InputValueAddSumNums(inputValue, dtPlus, row, new List<string>() { "1stFlrSF", "2ndFlrSF", "TotalBsmtSF" }, 3000);

                Orgai.InputValueAddSumNums(inputValue, dtPlus, row, new List<string>() { "YearBuilt", "YearRemodAdd" }, 4000);
                Orgai.InputValueAddSumNums(inputValue, dtPlus, row, new List<string>() { "OpenPorchSF", "3SsnPorch", "EnclosedPorch", "ScreenPorch", "WoodDeckSF" }, 2000);

                Orgai.InputValueAddGreaterZero(inputValue, dtPlus, row, "PoolArea");
                Orgai.InputValueAddGreaterZero(inputValue, dtPlus, row, "2ndFlrSF");
                Orgai.InputValueAddGreaterZero(inputValue, dtPlus, row, "GarageArea");
                Orgai.InputValueAddGreaterZero(inputValue, dtPlus, row, "TotalBsmtSF");
                Orgai.InputValueAddGreaterZero(inputValue, dtPlus, row, "Fireplaces");

                inputValue.Add((Orgai.GetFloat(dtPlus, row, "FullBath") + Orgai.GetFloat(dtPlus, row, "HalfBath") * 0.5f + Orgai.GetFloat(dtPlus, row, "BsmtFullBath") + Orgai.GetFloat(dtPlus, row, "BsmtHalfBath") * 0.5f) / 3.0f);
            }
            else
            {
                // ここには来ないはず
            }

            return inputValue;
        }
    }
}

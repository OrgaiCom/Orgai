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

//クロック
//for iTrainDataNum Clock : 2907097002
//  入力層にパラメータを渡す Clock :    19753
//  出力値を計算する Clock :            2042214
//      入力層の各ニューロンの y の値を計算する。 Clock :           19815
//      中間層の各ニューロンに前列から y の値を伝達させつつ Clock : 1995285
//      y の値を出力層のニューロンに伝達させる Clock :              16522
//      出力層のニューロンの y の値を計算する Clock :               14932
//  後処理 Clock :                      2786

#include "Pre.h"

// include ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
#include <math.h>
// include ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

// Config ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

// TITANIC か HOUSE_PRICES を選択
const int skipNum = 5;  // TITANIC
const unsigned int threadNum = 891;  // train data num // TITANIC

//const int skipNum = 3; // HOUSE PRICES
//const unsigned int threadNum = 1392; // train data num // HOUSE PRICES

const int neuralNetNum = 420;
const int inputNeuronNumMax = 20;

const int middleLayerCountMax = 10;
const int middleLayerNeuronNumMax = 40;

// FloatShareNum
#define FS1     11
#define FS2     1 // dummy
#define FS3     1 // dummy
#define FS4     (inputNeuronNumMax)
#define FS5     1 // trainDataNumMax
#define FS6     1 // dummy
#define FS7     middleLayerCountMax

#define FS8     inputNeuronNumMax
#define FS9     ((1 * inputNeuronNumMax) * skipNum)
#define FS10    1 // dummy
#define FS11    (inputNeuronNumMax * skipNum)

#define FS12    (middleLayerNeuronNumMax * middleLayerCountMax)
#define FS13    (((middleLayerNeuronNumMax * middleLayerNeuronNumMax) * middleLayerCountMax) * skipNum)
#define FS14    1 // dummy
#define FS15    ((middleLayerNeuronNumMax * middleLayerCountMax) * skipNum)

#define FS16    1
#define FS17    ((middleLayerNeuronNumMax * 1) * skipNum)
#define FS18    1 // dummy
#define FS19    (1 * skipNum)

// FloatInput0Num
#define FIZ1    skipNum  // nnNo

#define FIZ2    ((1 * inputNeuronNumMax) * skipNum)  // input layer W
#define FIZ3    (((middleLayerNeuronNumMax * middleLayerNeuronNumMax) * middleLayerCountMax) * skipNum)  // middle layer W
#define FIZ4    ((middleLayerNeuronNumMax * 1) * skipNum)  // output layer W

// FloatInputNum
//#define FI1    1

// FloatOutputNum
#define FO1    neuralNetNum // PredictValueList

// Offset(FloatShare)
#define FSOS1     FS1
#define FSOS2     (FSOS1 + FS2)
#define FSOS3     (FSOS2 + FS3)
#define FSOS4     (FSOS3 + FS4)
#define FSOS5     (FSOS4 + FS5)
#define FSOS6     (FSOS5 + FS6)
#define FSOS7     (FSOS6 + FS7)
#define FSOS8     (FSOS7 + FS8)
#define FSOS9     (FSOS8 + FS9)
#define FSOS10    (FSOS9 + FS10)
#define FSOS11    (FSOS10 + FS11)
#define FSOS12    (FSOS11 + FS12)
#define FSOS13    (FSOS12 + FS13)
#define FSOS14    (FSOS13 + FS14)
#define FSOS15    (FSOS14 + FS15)
#define FSOS16    (FSOS15 + FS16)
#define FSOS17    (FSOS16 + FS17)
#define FSOS18    (FSOS17 + FS18)
#define FSOS19    (FSOS18 + FS19)

// Offset(FloatInput0)
#define FIZOS1     FIZ1
#define FIZOS2     (FIZOS1 + FIZ2)
#define FIZOS3     (FIZOS2 + FIZ3)
#define FIZOS4     (FIZOS3 + FIZ4)

// Offset(FloatInput)
//#define FIOS1     FI1

// Offset(FloatOutput)
#define FOOS1     FO1

const unsigned int fShareNum = FSOS19;
const unsigned int fInput0Num = FIZOS4;
const unsigned int fInputNum = 0;
//const unsigned int fWorkNum = 1;
const unsigned int fOutputNum = FOOS1;

#define fsCorrectNum             FS(0)
#define fsAccuracyRate           FS(1)
#define fsCalcError              FS(2)
#define fsTrainDataNum           FS(3)
#define fsAiType                 FS(4)
#define fsThresholdVal           FS(5)
#define fsPosiValue              FS(6)
#define fsInputNeuronNum         FS(7)
#define fsMiddleLayerCount       FS(8)
#define fsOutputNeuronNum        FS(9)    // 1
#define fsOutputValue            FS(10)

#define fsInputValueList(aryIdx)             FS(FSOS3 + (aryIdx))
#define fsCorrectValueList(aryIdx)           FS(FSOS4 + (aryIdx))
#define fsMiddleNeuronNums(aryIdx)           FS(FSOS6 + (aryIdx))

#define fsInputLayerDendriteNum(aryIdx)      FS(FSOS7 + (aryIdx))
#define fsInputLayerXVal(skipIdx, aryIdx)    FS(FSOS8 + ((1 * inputNeuronNumMax) * skipIdx + aryIdx))
#define fsInputLayerYVal(skipIdx, aryIdx)    FS(FSOS10 + (inputNeuronNumMax * skipIdx + aryIdx))

#define fsMiddleLayerDendriteNum(layIdx, aryIdx)            FS(FSOS11 + (layIdx) * middleLayerNeuronNumMax + (aryIdx))
#define fsMiddleLayerXVal(skipIdx, layIdx, aryIdx, dIdx)    FS(FSOS12 + (((middleLayerNeuronNumMax * middleLayerNeuronNumMax) * middleLayerCountMax) * skipIdx) + (layIdx) * middleLayerNeuronNumMax * middleLayerNeuronNumMax + (aryIdx) * middleLayerNeuronNumMax + (dIdx))
#define fsMiddleLayerYVal(skipIdx, layIdx, aryIdx)          FS(FSOS14 + ((middleLayerNeuronNumMax * middleLayerCountMax) * skipIdx) + (layIdx) * middleLayerNeuronNumMax + (aryIdx))

#define fsOutputLayerDendriteNum(aryIdx)            FS(FSOS15 + (aryIdx))
#define fsOutputLayerXVal(skipIdx, aryIdx, dIdx)    FS(FSOS16 + ((middleLayerNeuronNumMax * 1) * skipIdx) + (aryIdx) * middleLayerNeuronNumMax + (dIdx))
#define fsOutputLayerYVal(skipIdx, aryIdx)          FS(FSOS18 + (1 * skipIdx) + (aryIdx)) // 1個

#define fizNnNo(skipIdx)                      FI0(skipIdx)

#define fizInputLayerWVal(skipIdx, aryIdx)                  FI0(FIZOS1 + ((1 * inputNeuronNumMax) * skipIdx + aryIdx))
#define fizMiddleLayerWVal(skipIdx, layIdx, aryIdx, dIdx)   FI0(FIZOS2 + (((middleLayerNeuronNumMax * middleLayerNeuronNumMax) * middleLayerCountMax) * skipIdx) + (layIdx) * middleLayerNeuronNumMax * middleLayerNeuronNumMax + (aryIdx) * middleLayerNeuronNumMax + (dIdx))
#define fizOutputLayerWVal(skipIdx, aryIdx, dIdx)           FI0(FIZOS3 + ((middleLayerNeuronNumMax * 1) * skipIdx) + (aryIdx) * middleLayerNeuronNumMax + (dIdx))

#define fiOne           FI(0)

#define fwOne           FW(0)

#define foPredictValueList(aryIdx)           FO(0 + (aryIdx))

// Config ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

#include "Export.h"

// 並列処理 ↓↓↓↓↓↓↓↓↓↓↓↓↓↓

// FuncKernel関数から呼ばれる関数 ↓↓↓↓↓↓↓↓↓↓↓↓↓↓

//__device__ void ComputeOutputValue()
//{
//
//}

// FuncKernel関数から呼ばれる関数 ↑↑↑↑↑↑↑↑↑↑↑↑↑↑

__global__ void FuncKernel(float* devFloatOutputArray, float* devFloatInput0Array, float* devFloatInputArray, float* devFloatWorkArray, float* devFloatShareArray)
{
    int bIdx = blockIdx.x;
    int tIdx = threadIdx.x;  // threadは1024まで
    //long long startClock;
//    __shared__ float inputArrayShare[inputNum];  // MAX 0xC000 byte(49KB)

    //ComputeOutputValue();

    //__syncthreads();  // 同期化

    // ここに並列処理のコードを書く ↓↓↓↓↓↓↓↓↓↓↓↓↓↓
    {
        int inputNeuronNum = fsInputNeuronNum;
        int nnNo = (int)(fizNnNo(tIdx));

        // 開始時間を記録
        //startClock = clock64();

        // 入力層にパラメータを渡す
        for (int i = 0; i < inputNeuronNum; i++)
        {
            fsInputLayerXVal(tIdx, i) = fsInputValueList(i);
        }

        //printf("入力層にパラメータを渡す Clock : %lld\n", clock64() - startClock);  // 19753

        //startClock = clock64();

        // 出力値を計算する
        {
            //startClock = clock64();

            // 入力層の各ニューロンの y の値を計算する。
            for (int row = 0; row < inputNeuronNum; row++)
            {
                float yVal = 0;

                // ｙ ＝ ｘ0ｗ0 ＋ ｘ1ｗ1 ＋ ｘ2ｗ2 ＋ ｘ3ｗ3
                yVal += fsInputLayerXVal(tIdx, row) * fizInputLayerWVal(tIdx, row);

                fsInputLayerYVal(tIdx, row) = yVal;

                //printf("fsInputLayerYVal(tIdx, row) = %f\n", fsInputLayerYVal(tIdx, row));
            }

            //printf("入力層の各ニューロンの y の値を計算する。 Clock : %lld\n", clock64() - startClock);  // 19815

            //startClock = clock64();  // ここが一番時間がかかる

            //printf("fsMiddleLayerCount = %f\n", fsMiddleLayerCount);

            // 中間層の各ニューロンに前列から y の値を伝達させつつ、
            // 各ニューロンの y の値を計算する。
            {
                int middleLayerCount = fsMiddleLayerCount;

                for (int c = 0; c < middleLayerCount; c++)
                {
                    int middleNeuronNumsC = fsMiddleNeuronNums(c);

                    if (c == 0)
                    {
                        for (int row = 0; row < middleNeuronNumsC; row++)
                        {
                            // このようなintへの置き換えは処理速度向上効果あり
                            int middleLayerDendriteNumCRow = fsMiddleLayerDendriteNum(c, row);

                            for (int x = 0; x < middleLayerDendriteNumCRow; x++)
                            {
                                fsMiddleLayerXVal(tIdx, c, row, x) = fsInputLayerYVal(tIdx, x);
                            }

                            //middleLayer[c].neurons[r].ComputeYVal();
                            {
                                float yVal = 0;

                                // ｙ ＝ ｘ0ｗ0 ＋ ｘ1ｗ1 ＋ ｘ2ｗ2 ＋ ｘ3ｗ3
                                for (int i = 0; i < middleLayerDendriteNumCRow; i++)
                                {
                                    yVal += fsMiddleLayerXVal(tIdx, c, row, i) * fizMiddleLayerWVal(tIdx, c, row, i);
                                }

                                fsMiddleLayerYVal(tIdx, c, row) = yVal;

                                //printf("c = %d   row = %d   fsMiddleLayerYVal(tIdx, c, row) = %f\n", c, row, fsMiddleLayerYVal(tIdx, c, row));
                            }
                        }
                    }
                    else
                    {
                        //printf("middleNeuronNumsC = %f\n", middleNeuronNumsC);

                        for (int row = 0; row < middleNeuronNumsC; row++)
                        {
                            int middleLayerDendriteNumCRow = fsMiddleLayerDendriteNum(c, row);

                            //printf("middleLayerDendriteNumCRow = %d\n", middleLayerDendriteNumCRow);

                            for (int x = 0; x < middleLayerDendriteNumCRow; x++)
                            {
                                //printf("c - 1 = %d   x = %d   fsMiddleLayerYVal(tIdx, c - 1, x) = %f\n", c - 1, x, fsMiddleLayerYVal(tIdx, c - 1, x));

                                fsMiddleLayerXVal(tIdx, c, row, x) = fsMiddleLayerYVal(tIdx, c - 1, x);

                                //printf("c = %d   row = %d   x = %d   fsMiddleLayerXVal(tIdx, c, row, x) = %f\n", c, row, x, fsMiddleLayerXVal(tIdx, c, row, x));
                            }

                            //middleLayer[c].neurons[r].ComputeYVal();
                            {
                                float yVal = 0;

                                // ｙ ＝ ｘ0ｗ0 ＋ ｘ1ｗ1 ＋ ｘ2ｗ2 ＋ ｘ3ｗ3
                                for (int i = 0; i < middleLayerDendriteNumCRow; i++)
                                {
                                    //printf("c = %d   row = %d   i = %d   fsMiddleLayerXVal(tIdx, c, row, i) = %f\n", c, row, i, fsMiddleLayerXVal(tIdx, c, row, i));
                                    //printf("fizMiddleLayerWVal(tIdx, c, row, i) = %f\n", fizMiddleLayerWVal(tIdx, c, row, i));

                                    yVal += fsMiddleLayerXVal(tIdx, c, row, i) * fizMiddleLayerWVal(tIdx, c, row, i);
                                }

                                fsMiddleLayerYVal(tIdx, c, row) = yVal;

                                //printf("c = %d   row = %d   fsMiddleLayerYVal(tIdx, c, row) = %f\n", c, row, fsMiddleLayerYVal(tIdx, c, row));
                            }
                        }
                    }
                }
            }

            // ここが一番時間がかかる
            //printf("中間層の各ニューロンに前列から y の値を伝達させつつ Clock : %lld\n", clock64() - startClock);  // 1995285 => 8789465

            //startClock = clock64();

            {
                int outputLayerDendriteNum0 = fsOutputLayerDendriteNum(0);

                // 中間層の最後の列から y の値を出力層のニューロンに伝達させる。
                for (int x = 0; x < outputLayerDendriteNum0; x++)
                {
                    fsOutputLayerXVal(tIdx, 0, x) = fsMiddleLayerYVal(tIdx, (int)fsMiddleLayerCount - 1, x);

                    //printf("x = %d   fsMiddleLayerYVal(tIdx, (int)fsMiddleLayerCount - 1, x) = %f\n", x, fsMiddleLayerYVal(tIdx, (int)fsMiddleLayerCount - 1, x));
                    //printf("fsOutputLayerXVal(tIdx, 0, x) = %f\n", fsOutputLayerXVal(tIdx, 0, x));
                }

                //printf("y の値を出力層のニューロンに伝達させる Clock : %lld\n", clock64() - startClock);  // 16522

                //startClock = clock64();

                // 出力層のニューロンの y の値を計算する。
                //outputLayer.ComputeYVal();
                {
                    float yVal = 0;

                    // ｙ ＝ ｘ0ｗ0 ＋ ｘ1ｗ1 ＋ ｘ2ｗ2 ＋ ｘ3ｗ3
                    for (int i = 0; i < outputLayerDendriteNum0; i++)
                    {
                        yVal += fsOutputLayerXVal(tIdx, 0, i) * fizOutputLayerWVal(tIdx, 0, i);
                    }

                    fsOutputLayerYVal(tIdx, 0) = yVal;
                }
            }

            //printf("出力層のニューロンの y の値を計算する Clock : %lld\n", clock64() - startClock);  // 14932
        }

        //printf("出力値を計算する Clock : %lld\n", clock64() - startClock);  // 2042214

        //startClock = clock64();

        // 出力値を取得する
        foPredictValueList(nnNo) = fsOutputLayerYVal(tIdx, 0);
        //printf("foPredictValueList(nnNo) = %f\n", foPredictValueList(nnNo));

        //printf("後処理 Clock : %lld\n", clock64() - startClock);  // 2786

        // 終了時間を記録
        //printf("終了時間 Clock : %lld\n", clock64() - startClock);  // 2907097002
    }
    // ここに並列処理のコードを書く ↑↑↑↑↑↑↑↑↑↑↑↑↑↑
}

// 並列処理 ↑↑↑↑↑↑↑↑↑↑↑↑↑↑

#include "Post.h"

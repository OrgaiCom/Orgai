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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonClass
{
    public class LibSp
    {
        [DllImport("SimpleParallel.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SimpleParallelDeviceSet();

        [DllImport("SimpleParallel.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SimpleParallelSetShare();

        [DllImport("SimpleParallel.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SimpleParallelMain();

        [DllImport("SimpleParallel.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SimpleParallelGetOutput();

        [DllImport("SimpleParallel.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SimpleParallelDeviceReset();

        [DllImport("SimpleParallel.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SimpleParallelGetShareArray();

        [DllImport("SimpleParallel.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SimpleParallelGetInput0Array();

        [DllImport("SimpleParallel.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SimpleParallelGetInputArray();

        [DllImport("SimpleParallel.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SimpleParallelGetOutputArray();

        [DllImport("SimpleParallel.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SimpleParallelGetThreadNum();

        [DllImport("SimpleParallel.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SimpleParallelGetShareNum();

        [DllImport("SimpleParallel.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SimpleParallelGetInput0Num();

        [DllImport("SimpleParallel.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SimpleParallelGetInputNum();

        [DllImport("SimpleParallel.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SimpleParallelGetOutputNum();

        public int threadNum;
        public int shareNum;
        public int input0Num;
        public int inputNum;
        public int outputNum;

        public IntPtr ptrShare;
        public IntPtr ptrInput0;
        public IntPtr ptrInput;
        public IntPtr ptrOutput;

        public int bIdx;

        /// <summary>
        /// GPUデバイスの使用を開始します。
        /// </summary>
        /// <returns>int | 0:成功   0以外:失敗</returns>
        public int DeviceSet()
        {
            return SimpleParallelDeviceSet();
        }

        /// <summary>
        /// ホストPCのShareメモリをGPUデバイスのShareメモリにコピーします。
        /// </summary>
        /// <returns>int | 0:成功   0以外:失敗</returns>
        public int SetShare()
        {
            return SimpleParallelSetShare();
        }

        /// <summary>
        /// GPUデバイス上でメイン処理（並列処理）を行います。
        /// </summary>
        /// <returns>int | 0:成功   0以外:失敗</returns>
        public int Main()
        {
            return SimpleParallelMain();
        }

        /// <summary>
        /// GPUデバイスのOutputメモリをホストPCのOutputメモリにコピーします。
        /// </summary>
        /// <returns>int | 0:成功   0以外:失敗</returns>
        public int GetOutput()
        {
            return SimpleParallelGetOutput();
        }

        /// <summary>
        /// GPUデバイスの使用を終了します。
        /// </summary>
        /// <returns>int | 0:成功   0以外:失敗</returns>
        public int DeviceReset()
        {
            return SimpleParallelDeviceReset();
        }

        /// <summary>
        /// ホストPCのShareメモリのポインタを返します。
        /// </summary>
        /// <returns>IntPtr | ホストPCのShareメモリのポインタ</returns>
        public IntPtr GetShareArray()
        {
            return SimpleParallelGetShareArray();
        }

        /// <summary>
        /// ホストPCのInput0メモリのポインタを返します。
        /// </summary>
        /// <returns>IntPtr | ホストPCのInput0メモリのポインタ</returns>
        public IntPtr GetInput0Array()
        {
            return SimpleParallelGetInput0Array();
        }

        /// <summary>
        /// ホストPCのInputメモリのポインタを返します。
        /// </summary>
        /// <returns>IntPtr | ホストPCのInputメモリのポインタ</returns>
        public IntPtr GetInputArray()
        {
            return SimpleParallelGetInputArray();
        }

        /// <summary>
        /// ホストPCのOutputメモリのポインタを返します。
        /// </summary>
        /// <returns>IntPtr | ホストPCのOutputメモリのポインタ</returns>
        public IntPtr GetOutputArray()
        {
            return SimpleParallelGetOutputArray();
        }

        /// <summary>
        /// スレッド数を取得します。
        /// </summary>
        /// <returns>int | スレッド数</returns>
        public int GetThreadNum()
        {
            return SimpleParallelGetThreadNum();
        }

        /// <summary>
        /// Shareパラメータの数を返します。
        /// </summary>
        /// <returns>int | Shareパラメータの数</returns>
        public int GetShareNum()
        {
            return SimpleParallelGetShareNum();
        }

        /// <summary>
        /// Input0パラメータの数を返します。
        /// </summary>
        /// <returns>int | Input0パラメータの数</returns>
        public int GetInput0Num()
        {
            return SimpleParallelGetInput0Num();
        }

        /// <summary>
        /// Inputパラメータの数を返します。
        /// </summary>
        /// <returns>int | Inputパラメータの数</returns>
        public int GetInputNum()
        {
            return SimpleParallelGetInputNum();
        }

        /// <summary>
        /// Outputパラメータの数を返します。
        /// </summary>
        /// <returns>int | Outputパラメータの数</returns>
        public int GetOutputNum()
        {
            return SimpleParallelGetOutputNum();
        }

        /// <summary>
        /// 各種パラメータを取得します。
        /// </summary>
        public void GetParams()
        {
            threadNum = GetThreadNum();
            shareNum = GetShareNum();
            input0Num = GetInput0Num();
            inputNum = GetInputNum();
            outputNum = GetOutputNum();

            ptrShare = GetShareArray();
            ptrInput0 = GetInput0Array();
            ptrInput = GetInputArray();
            ptrOutput = GetOutputArray();
        }

        /// <summary>
        /// floatのShareメモリ（配列）に値をセットします。
        /// </summary>
        /// <param name="idx">配列内のインデックス</param>
        /// <param name="value">セットする値</param>
        public void SetFS(int idx, float value)
        {
            unsafe
            {
                float* shareArray = (float*)ptrShare.ToPointer();

                shareArray[bIdx * shareNum + idx] = value;
            }
        }

        /// <summary>
        /// floatのShareメモリ（配列）の値をゲットします。
        /// </summary>
        /// <param name="idx">配列内のインデックス</param>
        /// <returns>float | ゲットした値</returns>
        public float GetFS(int idx)
        {
            unsafe
            {
                float* shareArray = (float*)ptrShare.ToPointer();

                return shareArray[bIdx * shareNum + idx];
            }
        }

        /// <summary>
        /// floatのShareメモリ（配列）の値をゲットします。
        /// </summary>
        /// <param name="blockIdx">threadのインデックス</param>
        /// <param name="idx">配列内のインデックス</param>
        /// <returns>float | ゲットした値</returns>
        public float GetFS(int blockIdx, int idx)
        {
            unsafe
            {
                float* shareArray = (float*)ptrShare.ToPointer();

                return shareArray[blockIdx * shareNum + idx];
            }
        }

        /// <summary>
        /// floatのInput0メモリ（配列）に値をセットします。
        /// </summary>
        /// <param name="idx">配列内のインデックス</param>
        /// <param name="value">セットする値</param>
        public void SetFI0(int idx, float value)
        {
            unsafe
            {
                float* input0Array = (float*)ptrInput0.ToPointer();

                input0Array[idx] = value;
            }
        }

        /// <summary>
        /// floatのInput0メモリ（配列）の値をゲットします。
        /// </summary>
        /// <param name="idx">配列内のインデックス</param>
        /// <returns>float | ゲットした値</returns>
        public float GetFI0(int idx)
        {
            unsafe
            {
                float* input0Array = (float*)ptrInput0.ToPointer();

                return input0Array[idx];
            }
        }

        /// <summary>
        /// floatのInputメモリ（配列）に値をセットします。
        /// </summary>
        /// <param name="idx">配列内のインデックス</param>
        /// <param name="value">セットする値</param>
        public void SetFI(int idx, float value)
        {
            unsafe
            {
                float* inputArray = (float*)ptrInput.ToPointer();

                inputArray[bIdx * inputNum + idx] = value;
            }
        }

        /// <summary>
        /// floatのInputメモリ（配列）の値をゲットします。
        /// </summary>
        /// <param name="idx">配列内のインデックス</param>
        /// <returns>float | ゲットした値</returns>
        public float GetFI(int idx)
        {
            unsafe
            {
                float* inputArray = (float*)ptrInput.ToPointer();

                return inputArray[bIdx * inputNum + idx];
            }
        }

        /// <summary>
        /// floatのInputメモリ（配列）の値をゲットします。
        /// </summary>
        /// <param name="blockIdx">threadのインデックス</param>
        /// <param name="idx">配列内のインデックス</param>
        /// <returns>float | ゲットした値</returns>
        public float GetFI(int blockIdx, int idx)
        {
            unsafe
            {
                float* inputArray = (float*)ptrInput.ToPointer();

                return inputArray[blockIdx * inputNum + idx];
            }
        }

        /// <summary>
        /// floatのOutputメモリ（配列）の値をゲットします。
        /// </summary>
        /// <param name="idx">配列内のインデックス</param>
        /// <returns>float | ゲットした値</returns>
        public float GetFO(int idx)
        {
            unsafe
            {
                float* outputArray = (float*)ptrOutput.ToPointer();

                return outputArray[bIdx * outputNum + idx];
            }
        }

        /// <summary>
        /// floatのOutputメモリ（配列）の値をゲットします。
        /// </summary>
        /// <param name="blockIdx">threadのインデックス</param>
        /// <param name="idx">配列内のインデックス</param>
        /// <returns>float | ゲットした値</returns>
        public float GetFO(int blockIdx, int idx)
        {
            unsafe
            {
                float* outputArray = (float*)ptrOutput.ToPointer();

                return outputArray[blockIdx * outputNum + idx];
            }
        }

        /// <summary>
        /// floatのShareメモリ（配列）に値をセットします。
        /// </summary>
        /// <param name="aryOffset">配列のオフセット</param>
        /// <param name="idx">配列内のインデックス</param>
        /// <param name="value">セットする値</param>
        public void SetFSAry(int aryOffset, int idx, float value)
        {
            unsafe
            {
                float* shareArray = (float*)ptrShare.ToPointer();

                shareArray[bIdx * shareNum + aryOffset + idx] = value;
            }
        }

        /// <summary>
        /// floatのShareメモリ（配列）の値をゲットします。
        /// </summary>
        /// <param name="aryOffset">配列のオフセット</param>
        /// <param name="idx">配列内のインデックス</param>
        /// <returns>float | ゲットした値</returns>
        public float GetFSAry(int aryOffset, int idx)
        {
            unsafe
            {
                float* shareArray = (float*)ptrShare.ToPointer();

                return shareArray[bIdx * shareNum + aryOffset + idx];
            }
        }

        /// <summary>
        /// floatのInput0メモリ（配列）に値をセットします。
        /// </summary>
        /// <param name="aryOffset">配列のオフセット</param>
        /// <param name="idx">配列内のインデックス</param>
        /// <param name="value">セットする値</param>
        public void SetFI0Ary(int aryOffset, int idx, float value)
        {
            unsafe
            {
                float* input0Array = (float*)ptrInput0.ToPointer();

                input0Array[aryOffset + idx] = value;
            }
        }

        /// <summary>
        /// floatのInput0メモリ（配列）の値をゲットします。
        /// </summary>
        /// <param name="aryOffset">配列のオフセット</param>
        /// <param name="idx">配列内のインデックス</param>
        /// <returns>float | ゲットした値</returns>
        public float GetFI0Ary(int aryOffset, int idx)
        {
            unsafe
            {
                float* input0Array = (float*)ptrInput0.ToPointer();

                return input0Array[aryOffset + idx];
            }
        }

        /// <summary>
        /// floatのInputメモリ（配列）に値をセットします。
        /// </summary>
        /// <param name="aryOffset">配列のオフセット</param>
        /// <param name="idx">配列内のインデックス</param>
        /// <param name="value">セットする値</param>
        public void SetFIAry(int aryOffset, int idx, float value)
        {
            unsafe
            {
                float* inputArray = (float*)ptrInput.ToPointer();

                inputArray[bIdx * inputNum + aryOffset + idx] = value;
            }
        }

        /// <summary>
        /// floatのInputメモリ（配列）の値をゲットします。
        /// </summary>
        /// <param name="aryOffset">配列のオフセット</param>
        /// <param name="idx">配列内のインデックス</param>
        /// <returns>float | ゲットした値</returns>
        public float GetFIAry(int aryOffset, int idx)
        {
            unsafe
            {
                float* inputArray = (float*)ptrInput.ToPointer();

                return inputArray[bIdx * inputNum + aryOffset + idx];
            }
        }
    }
}

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

#include <Windows.h>
#include <stdio.h>
#include <cuda.h>

#include "cuda_runtime.h"
#include "device_launch_parameters.h"
//#include "device_functions.h"

// DLLエクスポートのためのマクロ
#define DLL_EXPORT __declspec(dllexport)

#define SP_FSHARE0(fszOffset)   (devFloatShare0Array[fszOffset])
#define SP_FSHARE(fsOffset)		(devFloatShareArray[bIdx * fShareNum  + fsOffset])

#define SP_FINPUT0(fizOffset)   (devFloatInput0Array[fizOffset])
#define SP_FINPUT(fiOffset)		(devFloatInputArray[bIdx * fInputNum  + fiOffset])

#define SP_FWORK0(fwzOffset)    (devFloatWork0Array  [fwzOffset])
#define SP_FWORK(fwOffset)		(devFloatWorkArray  [bIdx * fWorkNum   + fwOffset])

#define SP_FOUTPUT0(fozOffset)  (devFloatOutput0Array[fozOffset])
#define SP_FOUTPUT(foOffset)	(devFloatOutputArray[bIdx * fOutputNum + foOffset])

#define FS0(fszOffset)	(SP_FSHARE0(fszOffset))
#define FI0(fizOffset)	(SP_FINPUT0(fizOffset))
#define FW0(fwzOffset)	(SP_FWORK0(fwzOffset))
#define FO0(fozOffset)	(SP_FOUTPUT0(fozOffset))

#define FS(fsOffset)	(SP_FSHARE(fsOffset))
#define FI(fiOffset)	(SP_FINPUT(fiOffset))
#define FW(fwOffset)	(SP_FWORK(fwOffset))
#define FO(foOffset)	(SP_FOUTPUT(foOffset))

cudaError_t FuncWithCuda(float* output, float* input0, float* input);
int SimpleParallelDeviceSetSub();
int SimpleParallelSetShareSub();
int SimpleParallelSub();
int SimpleParallelGetOutputSub();
int SimpleParallelDeviceResetSub();

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

//float fShare0ArrayGlobal[fShare0Num];
float fShareArrayGlobal[threadNum * fShareNum];

float fInput0ArrayGlobal[fInput0Num];
float fInputArrayGlobal[fInputNum > 0 ? threadNum * fInputNum : 1];

//float fWork0ArrayGlobal[fWorkNum];
//float fWorkArrayGlobal[threadNum * fWorkNum];

//float fOutput0ArrayGlobal[fOutput0Num];
float fOutputArrayGlobal[threadNum * fOutputNum];

extern "C"
{
    // エクスポートする関数
    DLL_EXPORT int SimpleParallelDeviceSet()
    {
        return SimpleParallelDeviceSetSub();
    }

    DLL_EXPORT int SimpleParallelSetShare()
    {
        return SimpleParallelSetShareSub();
    }

    DLL_EXPORT int SimpleParallelMain()
    {
        return SimpleParallelSub();
    }

    DLL_EXPORT int SimpleParallelGetOutput()
    {
        return SimpleParallelGetOutputSub();
    }

    DLL_EXPORT int SimpleParallelDeviceReset()
    {
        return SimpleParallelDeviceResetSub();
    }

    DLL_EXPORT float* SimpleParallelGetShareArray()
    {
        return fShareArrayGlobal;
    }

    DLL_EXPORT float* SimpleParallelGetInput0Array()
    {
        return fInput0ArrayGlobal;
    }

    DLL_EXPORT float* SimpleParallelGetInputArray()
    {
        return fInputArrayGlobal;
    }

    DLL_EXPORT float* SimpleParallelGetOutputArray()
    {
        return fOutputArrayGlobal;
    }

    DLL_EXPORT int SimpleParallelGetThreadNum()
    {
        return threadNum;
    }

    DLL_EXPORT int SimpleParallelGetShareNum()
    {
        return fShareNum;
    }

    DLL_EXPORT int SimpleParallelGetInput0Num()
    {
        return fInput0Num;
    }

    DLL_EXPORT int SimpleParallelGetInputNum()
    {
        return fInputNum;
    }

    DLL_EXPORT int SimpleParallelGetOutputNum()
    {
        return fOutputNum;
    }
}

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

float* dev_share = 0;
float* dev_input0 = 0;
float* dev_input = 0;
float* dev_work = 0;
float* dev_output = 0;

LARGE_INTEGER frequency;
LARGE_INTEGER start;
LARGE_INTEGER end;
double interval;

void StopwatchInit()
{
    // タイマーの周波数を取得
    QueryPerformanceFrequency(&frequency);
}

void StopwatchStart()
{
    // 開始時間を取得
    QueryPerformanceCounter(&start);
}

double StopwatchStop()
{
    // 終了時間を取得
    QueryPerformanceCounter(&end);

    // 経過時間を計算
    interval = (double)(end.QuadPart - start.QuadPart) / frequency.QuadPart;

    start = end;

    return interval;
}

void CudaMemFree()
{
    if (dev_output != 0)
    {
        cudaFree(dev_output);

        dev_output = 0;
    }

    if (dev_work != 0)
    {
        //cudaFree(dev_work);

        //dev_work = 0;
    }

    if (dev_input != 0)
    {
        cudaFree(dev_input);

        dev_input = 0;
    }

    if (dev_input0 != 0)
    {
        cudaFree(dev_input0);

        dev_input0 = 0;
    }

    if (dev_share != 0)
    {
        cudaFree(dev_share);

        dev_share = 0;
    }
}

int SimpleParallelDeviceSetSub()
{
    cudaError_t cudaStatus;

    dev_share = 0;
    dev_input0 = 0;
    dev_input = 0;
    //dev_work = 0;
    dev_output = 0;

    StopwatchInit();

    // Choose which GPU to run on, change this on a multi-GPU system.
    cudaStatus = cudaSetDevice(0);
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaSetDevice failed!  Do you have a CUDA-capable GPU installed?");
        
        return 1;
    }

    if (fShareNum > 0)
    {
        cudaStatus = cudaMalloc((void**)&dev_share, threadNum * fShareNum * sizeof(float));
        if (cudaStatus != cudaSuccess) {
            fprintf(stderr, "cudaMalloc failed!(float share)");

            return 2;
        }
    }

    if (fInput0Num > 0)
    {
        cudaStatus = cudaMalloc((void**)&dev_input0, fInput0Num * sizeof(float));
        if (cudaStatus != cudaSuccess) {
            fprintf(stderr, "cudaMalloc failed!(float input0)");

            CudaMemFree();

            return 3;
        }
    }

    if (fInputNum > 0)
    {
        cudaStatus = cudaMalloc((void**)&dev_input, threadNum * fInputNum * sizeof(float));
        if (cudaStatus != cudaSuccess) {
            fprintf(stderr, "cudaMalloc failed!(float input)");

            CudaMemFree();

            return 4;
        }
    }

    //if (fWorkNum > 0)
    //{
        //cudaStatus = cudaMalloc((void**)&dev_work, threadNum * workNum * sizeof(float));
    //if (cudaStatus != cudaSuccess) {
    //    fprintf(stderr, "cudaMalloc failed!(float work)");
    // 
    //    CudaMemFree();
    // 
    //    return 5;
    //}
    //}

    if (fOutputNum > 0)
    {
        cudaStatus = cudaMalloc((void**)&dev_output, threadNum * fOutputNum * sizeof(float));
        if (cudaStatus != cudaSuccess) {
            fprintf(stderr, "cudaMalloc failed!(float output)");

            CudaMemFree();

            return 6;
        }
    }

    return 0;
}

int SimpleParallelSetShareSub()
{
    cudaError_t cudaStatus;

    //StopwatchStart();

    // Copy input vectors from host memory to GPU buffers.
    cudaStatus = cudaMemcpy(dev_share, fShareArrayGlobal, threadNum * fShareNum * sizeof(float), cudaMemcpyHostToDevice);
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaMemcpy failed!(float share)");

        return 1;
    }

    //printf("経過時間(cudaMemcpy2): %f ms\n", StopwatchStop() * 1000);  // 60 - 100 msぐらいかかる（80倍して、4800 - 8000 ms）

    return 0;
}

int SimpleParallelSub()
{
    // Add vectors in parallel.
    cudaError_t cudaStatus = FuncWithCuda(fOutputArrayGlobal, fInput0ArrayGlobal, fInputArrayGlobal);
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "FuncWithCuda failed!");
        return 1;
    }

    return 0;
}

// Helper function for using CUDA to add vectors in parallel.
cudaError_t FuncWithCuda(float* outputGlobal, float* input0Global, float* inputGlobal)
{
    cudaError_t cudaStatus;

    //StopwatchStart();

    // Copy input vectors from host memory to GPU buffers.
    if (fInput0Num > 0)
    {
        cudaStatus = cudaMemcpy(dev_input0, input0Global, fInput0Num * sizeof(float), cudaMemcpyHostToDevice);
        if (cudaStatus != cudaSuccess) {
            fprintf(stderr, "cudaMemcpy failed!(float input0)");
            goto Error;
        }
    }

    // Copy input vectors from host memory to GPU buffers.
    if (fInputNum > 0)
    {
        cudaStatus = cudaMemcpy(dev_input, inputGlobal, threadNum * fInputNum * sizeof(float), cudaMemcpyHostToDevice);
        if (cudaStatus != cudaSuccess) {
            fprintf(stderr, "cudaMemcpy failed!(float input)");
            goto Error;
        }
    }

    //printf("経過時間(cudaMemcpy(float input)): %f ms\n", StopwatchStop() * 1000);  // 50 - 100 msぐらいかかる（80倍して、4000 - 8000 ms）
        // 24 - 38 ms に改善した。（80倍して、1920 - 3040 ms）(share array追加)

    // Launch a kernel on the GPU with one thread for each element.
    FuncKernel <<<threadNum, skipNum>>> (dev_output, dev_input0, dev_input, dev_work, dev_share);
    //FuncKernel <<<threadNum*1500, 1>>> (dev_output, dev_input, dev_work);  // エラーが出る

    //printf("経過時間(FuncKernel): %f ms\n", StopwatchStop() * 1000);  // 0.05 - 0.08 msぐらいかかる（80倍して、4 - 6.4 ms）

    // Check for any errors launching the kernel
    cudaStatus = cudaGetLastError();
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "FuncKernel launch failed: %s\n", cudaGetErrorString(cudaStatus));
        goto Error;
    }

    //printf("経過時間(cudaGetLastError): %f ms\n", StopwatchStop() * 1000);  // 0.005 msぐらいかかる（80倍して、0.4 ms）

    // cudaDeviceSynchronize waits for the kernel to finish, and returns
    // any errors encountered during the launch.
    cudaStatus = cudaDeviceSynchronize();
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaDeviceSynchronize returned error code %d after launching FuncKernel!\n", cudaStatus);
        goto Error;
    }

    //printf("経過時間(cudaDeviceSynchronize): %f ms\n", StopwatchStop() * 1000);  // 8 - 9 msぐらいかかる（80倍して、640 - 720 ms）

Error:

    return cudaStatus;
}

int SimpleParallelGetOutputSub()
{
    cudaError_t cudaStatus;

    //StopwatchStart();

    // Copy output vector from GPU buffer to host memory.
    cudaStatus = cudaMemcpy(fOutputArrayGlobal, dev_output, threadNum * fOutputNum * sizeof(float), cudaMemcpyDeviceToHost);
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaMemcpy failed!(outputArrayGlobal)");

        return 1;
    }

    //printf("経過時間(cudaMemcpy(float output)): %f ms\n", StopwatchStop() * 1000);  // 60 - 100 msぐらいかかる（80倍して、4800 - 8000 ms）
        // 0.2 ms に改善した。（PredictValueListを OutputArrayにして、ループ外でコピーした）

    return 0;
}

int SimpleParallelDeviceResetSub()
{
    cudaError_t cudaStatus;

    CudaMemFree();

    // cudaDeviceReset must be called before exiting in order for profiling and
    // tracing tools such as Nsight and Visual Profiler to show complete traces.
    cudaStatus = cudaDeviceReset();
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaDeviceReset failed!");
        return 1;
    }

    return 0;
}

/*
    The MIT License

    Copyright(c) 2024 OrgaiCom

    �ȉ��ɒ�߂�����ɏ]���A�{�\�t�g�E�F�A����ъ֘A�����̃t�@�C���i�ȉ��u�\�t�g�E�F�A�v�j
    �̕������擾���邷�ׂĂ̐l�ɑ΂��A�\�t�g�E�F�A�𖳐����Ɉ������Ƃ𖳏��ŋ����܂��B
    ����ɂ́A�\�t�g�E�F�A�̕������g�p�A���ʁA�ύX�A�����A�f�ځA�Еz�A�T�u���C�Z���X�A
    �����/�܂��͔̔����錠���A����у\�t�g�E�F�A��񋟂��鑊��ɓ������Ƃ������錠����
    �������Ɋ܂܂�܂��B

    ��L�̒��쌠�\������і{�����\�����A�\�t�g�E�F�A�̂��ׂĂ̕����܂��͏d�v�ȕ�����
    �L�ڂ�����̂Ƃ��܂��B

    �\�t�g�E�F�A�́u����̂܂܁v�ŁA�����ł��邩�Öقł��邩���킸�A����̕ۏ؂��Ȃ�
    �񋟂���܂��B�����ł����ۏ؂Ƃ́A���i���A����̖ړI�ւ̓K�����A����ь�����N�Q��
    ���Ă̕ۏ؂��܂݂܂����A����Ɍ��肳�����̂ł͂���܂���B ��҂܂��͒��쌠�҂́A
    �_��s�ׁA�s�@�s�ׁA�܂��͂���ȊO�ł��낤�ƁA�\�t�g�E�F�A�ɋN���܂��͊֘A���A
    ���邢�̓\�t�g�E�F�A�̎g�p�܂��͂��̑��̈����ɂ���Đ������؂̐����A���Q�A
    ���̑��̋`���ɂ��ĉ���̐ӔC������Ȃ����̂Ƃ��܂��B
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
    // �^�C�}�[�̎��g�����擾
    QueryPerformanceFrequency(&frequency);
}

void StopwatchStart()
{
    // �J�n���Ԃ��擾
    QueryPerformanceCounter(&start);
}

double StopwatchStop()
{
    // �I�����Ԃ��擾
    QueryPerformanceCounter(&end);

    // �o�ߎ��Ԃ��v�Z
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

    //printf("�o�ߎ���(cudaMemcpy2): %f ms\n", StopwatchStop() * 1000);  // 60 - 100 ms���炢������i80�{���āA4800 - 8000 ms�j

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

    //printf("�o�ߎ���(cudaMemcpy(float input)): %f ms\n", StopwatchStop() * 1000);  // 50 - 100 ms���炢������i80�{���āA4000 - 8000 ms�j
        // 24 - 38 ms �ɉ��P�����B�i80�{���āA1920 - 3040 ms�j(share array�ǉ�)

    // Launch a kernel on the GPU with one thread for each element.
    FuncKernel <<<threadNum, skipNum>>> (dev_output, dev_input0, dev_input, dev_work, dev_share);
    //FuncKernel <<<threadNum*1500, 1>>> (dev_output, dev_input, dev_work);  // �G���[���o��

    //printf("�o�ߎ���(FuncKernel): %f ms\n", StopwatchStop() * 1000);  // 0.05 - 0.08 ms���炢������i80�{���āA4 - 6.4 ms�j

    // Check for any errors launching the kernel
    cudaStatus = cudaGetLastError();
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "FuncKernel launch failed: %s\n", cudaGetErrorString(cudaStatus));
        goto Error;
    }

    //printf("�o�ߎ���(cudaGetLastError): %f ms\n", StopwatchStop() * 1000);  // 0.005 ms���炢������i80�{���āA0.4 ms�j

    // cudaDeviceSynchronize waits for the kernel to finish, and returns
    // any errors encountered during the launch.
    cudaStatus = cudaDeviceSynchronize();
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaDeviceSynchronize returned error code %d after launching FuncKernel!\n", cudaStatus);
        goto Error;
    }

    //printf("�o�ߎ���(cudaDeviceSynchronize): %f ms\n", StopwatchStop() * 1000);  // 8 - 9 ms���炢������i80�{���āA640 - 720 ms�j

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

    //printf("�o�ߎ���(cudaMemcpy(float output)): %f ms\n", StopwatchStop() * 1000);  // 60 - 100 ms���炢������i80�{���āA4800 - 8000 ms�j
        // 0.2 ms �ɉ��P�����B�iPredictValueList�� OutputArray�ɂ��āA���[�v�O�ŃR�s�[�����j

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

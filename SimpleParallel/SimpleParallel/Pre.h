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

#include <Windows.h>
#include <stdio.h>
#include <cuda.h>

#include "cuda_runtime.h"
#include "device_launch_parameters.h"
//#include "device_functions.h"

// DLL�G�N�X�|�[�g�̂��߂̃}�N��
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

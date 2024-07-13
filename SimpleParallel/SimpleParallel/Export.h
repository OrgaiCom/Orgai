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
    // �G�N�X�|�[�g����֐�
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

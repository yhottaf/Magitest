namespace fantec
{
    interface IAdvSkipSpeed
    {
        //ループアニメーションなど、スキップ解除後に速度変化させる必要があるものに使用する
        void OnChangeSkipSpeed(float speed);
    }
}

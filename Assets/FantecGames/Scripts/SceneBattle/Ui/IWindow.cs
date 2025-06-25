using UniRx;


namespace fantec.Battle
{
    public interface IWindow<T> : ICloseable
    {
        CompositeDisposable ClosedDisposable { get; }

        /// <summary>
        /// ウィンドウが作成された際に呼ばれる
        /// ※生成後1度しか呼ばれないためここで Subscribe を行う
        /// </summary>
        /// <returns></returns>
        T OnCreate();
    }
}
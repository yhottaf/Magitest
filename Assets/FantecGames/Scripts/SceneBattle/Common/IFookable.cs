using UniRx;

namespace fantec.Battle
{
    /// <summary>
    /// <see cref="bool"/> によるトリガーを用意して引っ掛ける感じの用途
    /// <see cref="FookableExtentions.FookOn(IFookable)"/> でフックを有効に
    /// <see cref="FookableExtentions.FookOff(IFookable)"/> でフックを無効に
    /// <see cref="IReadOnlyReactiveProperty{T}"/> への購読は可能
    /// </summary>
    public interface IFookable
    {
        public IReadOnlyReactiveProperty<bool> OnIsFook { get; }
    }

    public static class FookableExtentions
    {
        public static void FookOn(this IFookable @this)
        {
            (@this.OnIsFook as IReactiveProperty<bool>).Value = true;
        }

        public static void FookOff(this IFookable @this)
        {
            (@this.OnIsFook as IReactiveProperty<bool>).Value = false;
        }
    }
}
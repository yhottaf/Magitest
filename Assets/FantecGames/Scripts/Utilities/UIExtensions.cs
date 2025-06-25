using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;


public static class UIExtensions
{
    public static IObservable<Unit> OnLongTapAsObservable(this Button button, float pressSeconds = 2f)
    {
        return button
            .OnPointerDownAsObservable()
            .Throttle(TimeSpan.FromSeconds(pressSeconds))
            .TakeUntil(button.OnPointerExitAsObservable()) // 押したまま指がボタン領域から離れたら終了
            .TakeUntil(button.OnPointerUpAsObservable())
            .RepeatUntilDestroy(button)
            .AsUnitObservable();
    }
    public static IObservable<Unit> OnPressingAsObservable(this Button button, float pressSeconds = 2f)
    {
        return button
            .OnPointerDownAsObservable()
            .Throttle(TimeSpan.FromSeconds(pressSeconds))
            .TakeUntil(button.OnPointerUpAsObservable())
            .RepeatUntilDestroy(button)
            .AsUnitObservable();
    }

    public static IObservable<Unit> OnClickAsObservableSafety(this Button button, float duplicateSafetySeconds = 1f, float pressSafetySeconds = 1f)
    {
        return button
            .OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(duplicateSafetySeconds)) // 連打防止
            .SkipUntil(button.OnPointerDownAsObservable())
            .TakeUntil(button.OnLongTapAsObservable(pressSafetySeconds)) // 長押し後に指を離してもタップイベントを発行しない
            .RepeatUntilDestroy(button)
            .AsUnitObservable();
    }
}

using UnityEngine;

namespace fantec.Battle.Model
{
    public interface IStageGauge
    {
        int MaxHealth { get; }     // 最大体力値
        int CurrentHealth { get; } // 現在の体力値
    }

    public static class StateGaugeExtentions
    {
        public static ValueChangeInfo GetCurrentInfo(this IStageGauge @this)
        {
            return new ValueChangeInfo()
            {
                maxValue = @this.MaxHealth,
                oldValue=@this.CurrentHealth,
                newValue =@this.CurrentHealth,
            };
        }

        public static ValueChangeInfo GetHealValueInfo(this IStageGauge @this, int value)
        {
            return new ValueChangeInfo()
            {
                affectValue = value,
                maxValue = @this.MaxHealth,
                oldValue = @this.CurrentHealth,
                newValue = @this.CurrentHealth+value,
            };
        }

        public static ValueChangeInfo GetDamageValueInfo(this IStageGauge @this, int value)
        {
            return new ValueChangeInfo()
            {
                affectValue = value,
                maxValue = @this.MaxHealth,
                oldValue = @this.CurrentHealth,
                newValue = @this.CurrentHealth - value,
            };
        }

        public static ValueChangeInfo GetKillValueInfo(this IStageGauge @this)
        {
            return new ValueChangeInfo()
            {
                maxValue = @this.MaxHealth,
                oldValue = @this.CurrentHealth,
                newValue = 0,
            };
        }

        public static ValueChangeInfo GetReviveValueInfo(this IStageGauge @this)
        {
            return new ValueChangeInfo()
            {
                maxValue = @this.MaxHealth,
                oldValue = @this.CurrentHealth,
                newValue = @this.MaxHealth,
            };
        }

        public static bool GetIsZero(this IStageGauge @this)
        {
            return @this.CurrentHealth <= 0;
        }

        public static bool GetIsFull(this IStageGauge @this)
        {
            return @this.CurrentHealth >= @this.MaxHealth;
        }
    }
}
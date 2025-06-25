using DG.Tweening;
using System;
using System.Collections.Generic;
using UniRx;

namespace fantec.Battle
{
    public class Affect
    {
        /// <summary>
        /// スキルを発動する
        /// </summary>
        public static void Activation(AffectInfoBox infoBox, Action onCompleted)
        {
            var info = infoBox.GetMain();
            var notify = info.GetNotify();

            notify.Activation(info);
            notify.OnActionCompletedObservable.First().Subscribe(_ => onCompleted.Invoke());
        }

        /// <summary>
        /// 複数人分のスキルを staggered（順にずらして）発動し、すべて完了したら onCompleted を呼ぶ
        /// </summary>
        public static void Activation(List<AffectInfoBox> infoBoxes, Action onCompleted)
        {
            if (infoBoxes == null || infoBoxes.Count == 0)
            {
                onCompleted?.Invoke();
                return;
            }

            float delayPerIndex = 0.3f;
            int completed = 0;
            int total = infoBoxes.Count;

            for (int i = 0; i < total; i++)
            {
                var infoBox = infoBoxes[i];
                float delay = i * delayPerIndex;

                // 指定時間後に各演出開始
                DOVirtual.DelayedCall(delay, () =>
                {
                    Activation(infoBox, () =>
                    {
                        completed++;
                        if (completed == total)
                        {
                            onCompleted?.Invoke();
                        }
                    });
                });
            }
        }

        /// <summary>
        /// 効果を反映する
        /// </summary>
        public static void Excute(AffectInfoBox infoBox)
        {
            Execute(infoBox.infoList);
        }

        /// <summary>
        /// 効果を反映する
        /// </summary>
        public static void Execute(IEnumerable<AffectInfo> infos)
        {
            foreach (var info in infos) Execute(info);
        }

        /// <summary>
        /// 効果を反映する
        /// </summary>
        public static void Execute(AffectInfo info)
        {
            switch (info.Command.categoryType)
            {
                case AffectCategoryType.EMPTY:
                    // 何もしない
                    break;

                case AffectCategoryType.AttackNormal:      // 通常ダメージ(%)
                case AffectCategoryType.AttackFixed:       // 固定値ダメージ
                case AffectCategoryType.AttackRatio:       // 割合ダメージ
                    TakeDamage(info);
                    break;
                case AffectCategoryType.HealRatio:
                case AffectCategoryType.HealFixed:
                    TakeHeal(info);
                    break;
                case AffectCategoryType.Revival:
                    TakeRevive(info);
                    break;
                case AffectCategoryType.Kill:
                    TakeKill(info);
                    break;




                case AffectCategoryType.NONE:
                    throw new Exception($"[skillId: {info.Entity.skillId}] [targetType: {info.Command.categoryType.GetCommandString()}] のカテゴリが設定されていません。");
                default:
                    throw new Exception($"[{info.Command.categoryType}] は未対応です。");
            }
        }

        #region TakeAffects

        private static void TakeHeal(AffectInfo info)
        {
            foreach (var battler in info.Targets)
            {
                if (battler.State.Health.IsDead) continue;

                info.CalcAffect(battler);

                battler.State.Health.TakeHeal(info);
            }
        }

        private static void TakeDamage(AffectInfo info)
        {
            foreach (var target in info.Targets)
            {
                if (target.State.Health.IsDead) continue;

                info.CalcHitType(target);   // 攻撃が命中するかの計算
                info.CalcAffect(target);    // 効果量の計算

                target.State.Health.TakeDamage(info);

                if (target.GetIsBoss())
                {
                    var boss = target.ToBoss();

                }
            }
        }

        private static void TakeDispel(AffectInfo info)
        {
            foreach (var battler in info.Targets)
            {
                if (battler.State.Health.IsDead) continue;
                battler.State.TakeDispel(info);
            }
        }

        private static void TakeAbnormalRecovery(AffectInfo info)
        {
            foreach (var battler in info.Targets)
            {
                battler.State.TakeAbnormalRecobery(info);
            }
        }

        private static void TakeToken(AffectInfo info)
        {
            foreach (var battler in info.Targets)
            {
                if (battler.State.Health.IsDead) continue;

                info.CalcHitType(battler);

                battler.State.TakeToken(info);
            }
        }

        private static void TakeKill(AffectInfo info)
        {
            foreach (var battler in info.Targets)
            {
                if (battler.State.Health.IsDead) continue;

                battler.State.Health.Kill(info);

                if (battler.GetIsBoss())
                {
                    var bosss = battler.ToBoss();
                }
            }
        }

        private static void TakeRevive(AffectInfo info)
        {
            foreach (var battler in info.Targets)
            {
                if (battler.Unit.Entity == null) continue;
                if (battler.State.Health.IsDead == false) continue;

                battler.State.Health.Revive(info);
                battler.Reload();
            }
        }

        #endregion
    }
}
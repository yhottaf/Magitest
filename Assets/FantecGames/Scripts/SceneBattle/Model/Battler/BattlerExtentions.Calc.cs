using fantec.Battle.Model;
using UnityEngine;

namespace fantec.Battle
{
    // TODO: ダメージ計算周り
    public static partial class BattlerExtentions
    {
        /// <summary>
        /// 効果量の算出
        /// </summary>
        /// <param name="this">発動者</param>
        /// <param name="command">効果コマンド</param>
        /// <param name="target">対象</param>
        /// <returns></returns>
        public static int GetCalcAffectValue(this IBattler @this,AffectInfo info,IBattler target)
        {
            // 直値代入
            var command = info.Command;
            var affectValue = info.Command.affectValue;

            // べースをもとに計算するなら
            if(command.categoryType.GetIsCalcUseBaseValue())
            {
                affectValue = (int)(@this.State.CurrentATK * affectValue * Random.Range(95, 105) * 0.0001f);
            }

            // 割合で計算するなら
            if (command.categoryType.GetIsCalcRasio())
            {
                affectValue = (int)(target.State.Health.MaxHealth * (affectValue / 100.0f));
            }

            // TODO：耐性などをあとあと作成して考慮するならここに記載


            // 上限下限設定
            affectValue = Mathf.Clamp(affectValue, 0, @this.State.CurrentDMG);

            return affectValue;
        }

        public static HitResultType GetCalcHitType(this IBattler @this,AffectInfo info,IBattler target)
        {
            // TODO: 攻撃の命中率など
            // 今のところ成功のみ発生させているが、あとあと回避力、行動の成功値などを設定したい場合は
            // ここを拡張する
            return HitResultType.Success;
        }


        // TODO : 使用しないかもしれないが成功値などを設定する場合は使用する　例: 20％の確率で回避など
        private static bool GetIsRandom(int rate)
        {
            return Random.Range(0, 100) < rate;
        }
    }
}
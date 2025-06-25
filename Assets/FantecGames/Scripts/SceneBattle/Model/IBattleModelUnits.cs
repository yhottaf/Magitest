using Cysharp.Threading.Tasks;
using fantec.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace fantec.Battle.Model
{
    public interface IBattleModelUnits :ILocatable,IDisposable,IReloadable
    {
        /// <summary> 決着がついたか否か監視 </summary>
        IObservable<Unit> OnSettledObservable { get; }
        /// <summary> 敵が全滅したか監視 </summary>
        IObservable<Unit> OnDefeatEnemyObservable { get; }
        /// <summary> 味方が全滅したか監視 </summary>
        IObservable<Unit> OnDefeatPlayerObservable { get; }
        /// <summary> Battler の死亡監視 </summary>
        IObservable<IBattler> OnDeadBattlerObservable { get; }
        /// <summary> Battler の復活監視 </summary>
        IObservable<IBattler> OnReviveBattlerObservable { get; }
        /// <summary> アドベントスキルのアクション開始時 </summary>
        IObservable<AffectInfo> OnActionAdventObserveble { get; }
        /// <summary> オーバードライブスキルのアクション開始時 </summary>
        IObservable<AffectInfo> OnActionOverrideSkillObservable { get; }
        /// <summary> Battler のバフ実行監視 </summary>
        IObservable<AffectInfo> OnBuffedObservable { get; }
        /// <summary> 味方メンバーの更新監視 </summary>
        IObservable<IEnumerable<IBattler>> OnUpdatePlayerMemberObservable { get; }
        /// <summary> 敵メンバーの更新監視 </summary>
        IObservable<IEnumerable<IBattler>> OnUpdateEnemyMemberObservable { get; }

        IBattler[] PlayerDatas { get; }
        IBattler[] EnemyDatas { get; }
        IBattler BossData { get; }

        /// <summary> 味方全滅しているか否か </summary>
        bool IsPlayerDefeat { get; }
        /// <summary> 敵が全滅しているか否か </summary>
        bool IsEnemyDefeat { get; }

        /// <summary> 味方のHPの割合 </summary>
        float PlayerHPRatio { get; }

        /// <summary> 敵のHPの割合 </summary>
        float EnemyHPRatio { get; }

        /// <summary> 決着しているか否か </summary>
        bool IsSettled { get; }

        void ResetPlayerTeam();
        void ResetEnemyTeam();
        void SetPlayerTeam(TeamData team);
        void SetEnemyTeam(TeamData team);
    }
    public static class BattleModelUnitsExtentions
    {
        /// <summary>
        /// スキル情報を元に効果対象のバトラーを取得する
        /// </summary>
        public static IEnumerable<IBattler>GetBattler(this IBattleModelUnits @this,AffectInfo info)
        {
            var owner = info.Owner;
            var rangeType = info.Command.rangeType;
            var randomCount = info.Command.randomCount;
            var isEnemy = owner.GetIsEnemy();

            // 発動者のサイドをもとに大まかな対象を取得
            IEnumerable<IBattler>result=rangeType.GetIsMySide()
                ? isEnemy ? @this.EnemyDatas : @this.PlayerDatas
                :isEnemy ? @this.PlayerDatas : @this.EnemyDatas;

            // result=result.Getsur TODO: 誰を対象にするか決める

            // 通常の選定
            {
                switch (rangeType)
                {
                    // ----------------------------------------- //
                    // 「発動者からみた味方」
                    // ----------------------------------------- //

                    case AffectRangeType.MySideAll: return result;
                    case AffectRangeType.MySideSingle: return result.GetMinimumHpBattler().ToEnumerable();
                    //case AffectRangeType.MySideVanguard: return result.GetVanguardBattlers();
                    //case AffectRangeType.MySideRearguard: return result.GetRearguardBattlers();
                    case AffectRangeType.Myself: return result.GetMyselfBattlers(owner);
                    case AffectRangeType.BesidesMe: return result.GetBesidesMeBattler(owner);
                    case AffectRangeType.MySideAllRandom: return result.GetRandomBattler(randomCount);
                   // case AffectRangeType.MySideVanguardRandom: return result.GetVanguardBattlers().GetRandomBattler(randomCount);
                   // case AffectRangeType.MySideRearguardRandom: return result.GetRearguardBattlers().GetRandomBattler(randomCount);
                    case AffectRangeType.BesideMeRandom: return result.GetBesidesMeBattler(owner).GetRandomBattler(randomCount);



                    // ----------------------------------------- //
                    // 「発動者からみた敵」
                    // ----------------------------------------- //

                    case AffectRangeType.EnemyAll: return result;
                    case AffectRangeType.EnemySingle: return result.GetTargetBattler(info).ToEnumerable();
                   // case AffectRangeType.EnemyVanguard: return result.GetVanguardBattlers();
                    //case AffectRangeType.EnemyRearguard: return result.GetRearguardBattlers();
                    case AffectRangeType.EnemyAllRandom: return result.GetRandomBattler(randomCount);
                    //case AffectRangeType.EnemyVanguardRandom: return result.GetVanguardBattlers().GetRandomBattler(randomCount);
                   // case AffectRangeType.EnemyRearguardRandom: return result.GetRearguardBattlers().GetRandomBattler(randomCount);


                    default:
                        throw new Exception($"[{rangeType}] が設定されていません。");
                }
            }
        }

        // TODO : 生存しているバトラーの情報を取得できるようにしておく
        /// <summary>
        /// スキル情報を元に効果対象のバトラーを取得し、対象が存在しなかった場合補完する
        /// </summary>
        public static IEnumerable<IBattler>GetBattlerComplement(this IBattleModelUnits @this,AffectInfo info)
        {
            var result = @this.GetBattler(info);
            if(result.Any())
            {
                return result;
            }
            else
            {
                var isEnemy = info.Owner.GetIsEnemy();
                if (info.Command.rangeType.GetIsMySide())
                {
                    return isEnemy
                        ? @this.EnemyDatas.GetSurvivedBattlers().GetMinimumHpBattler().ToEnumerable()
                        : @this.PlayerDatas.GetSurvivedBattlers().GetMinimumHpBattler().ToEnumerable();
                }
                else
                {
                    return isEnemy
                        ? @this.PlayerDatas.GetSurvivedBattlers().GetTargetBattler(info).ToEnumerable()
                        : @this.EnemyDatas.GetSurvivedBattlers().GetTargetBattler(info).ToEnumerable();
                }
            }
        }
    }
}
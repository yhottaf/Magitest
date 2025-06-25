using fantec.Battle.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fantec.Battle
{
    /// <summary>
    /// 複数の AffectInfo を管理する
    /// </summary>
    public class AffectInfoBox
    {
        public readonly List<AffectInfo> infoList;

        public IBattler Owner { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AffectInfoBox(AbstructSkillEntity entity,IBattler owner)
        {
            // 初期化
            infoList = new List<AffectInfo>();
            Owner = owner;

            // 比較用の優先順位番号を用意
            var priority = 99;

            // コマンドを AffectInfo に入れ込み
            foreach(var command in entity.commands)
            {
                var tempInfo=new AffectInfo().SetCommand(command).SetEntity(entity);
                var tempPriority = tempInfo.Command.rangeType.GetPriority();

                if(tempPriority < priority) 
                {
                    // 優先数値更新
                    priority = tempPriority;

                    // メイン更新
                    infoList.SetMain(false);
                    tempInfo.SetMain(true);
                }

                tempInfo.SetOwner(owner);
                tempInfo.SetTarget(Locator.Resolve<IBattleModelUnits>().GetBattlerComplement(tempInfo));

                infoList.Add(tempInfo.Clone());
            }

            // 順番の並び変え
            infoList = infoList.SortByPriority().ToList();
        }

        /// <summary>
        /// 効果量の表示を隠すか否か設定
        /// </summary>
        public AffectInfoBox SetIsHideNumeral(bool enabled)
        {
            foreach (var info in infoList) info.SetIsHideNumeral(enabled);
            return this;
        }

        /// <summary>
        /// 計算種別の設定
        /// </summary>
        public AffectInfoBox SetCalcType(ValueCalcType calcType)
        {
            foreach (var info in infoList) info.SetCalcType(calcType);
            return this;
        }

        /// <summary>
        /// 発動者を設定
        /// </summary>
        public AffectInfoBox SetOwner(IBattler owner)
        {
            Owner = owner;
            foreach(var info in infoList) info.SetOwner(owner);
            return this;
        }

        /// <summary>
        /// 効果対象を設定
        /// </summary>
        /// <returns></returns>
        public AffectInfoBox SetTarget()
        {
            foreach (var info in infoList) info.SetTarget(Locator.Resolve<IBattleModelUnits>().GetBattlerComplement(info));
            return this;
        }

        /// <summary>
        /// 効果対象を直接入れ込む
        /// </summary>
        public AffectInfoBox SetTarget(IBattler[]targets)
        {
            foreach (var info in infoList) info.SetTarget(targets);
            return this;
        }

        /// <summary>
        /// 攻撃前に発動するスキルを取得する
        /// </summary>
        public IEnumerable<AffectInfo>GetFirstInfos()
        {
            return infoList.GetFirstInfos();
        }


        /// <summary>
        /// 攻撃後に発動するスキルを取得する
        /// </summary>
        public IEnumerable<AffectInfo>GetLateInfos()
        {
            return infoList.GetLateInfos();
        }

        /// <summary>
        /// 攻撃系のスキルを取得する
        /// </summary>
        public IEnumerable<AffectInfo>GetAttackInfos()
        {
            return infoList.GetAttackInfos();
        }

        /// <summary>
        /// メインとして設定されたやつを取得する
        /// </summary>
        public AffectInfo GetMain()
        {
            return infoList.GetMainInfos().FirstOrDefault();
        }

        /// <summary>
        /// 攻撃を受ける対象をかぶりなしで取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable <IBattler> GetAttackedTargets()
        {
            var result = new List<IBattler>();
            foreach(var info in GetAttackInfos())
            {
                foreach (var battler in info.Targets)
                {
                    if (result.Contains(battler) == false)
                    {
                        result.Add(battler);
                    }
                }
            }

            return result;
        }
    }
}
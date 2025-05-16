using UnityEngine;
using fantec.Battle.Model;
using System.Collections.Generic;

namespace fantec.Battle
{
    public enum SkillType
    {
        Override,        // オーバーライド
        ExsaOverride,    // エクサオーバーライド
        SectaOverride,   // ゼタオーバーライド
        QuetaOverride,   // クエタオーバーライド
        Normal,          // 通常攻撃
        Style,           // スキルによるバフ
        Gimmic,          // バトル開始時のバフ
        Token,           // スキル内の付与効果
        System,          // システム
        Dummy,           // ダミー
        Danger           // デンジャー保留
    }

     // 特殊なダメージ計算の種類
    public enum ValueCalcType
    {
        Normal,          // 通常計算
        Direct,          // AffectValue をそのまま使う
    }

    public enum HitResultType
    {
        Success,         // 攻撃成功
        Miss,            // 攻撃失敗、回避された
        Guard,           // シールド等、ガード状態時に当たった時
    }

    public class AffectInfo
    {
        private static Dictionary<System.Type, SkillType> m_SkillEntityDict = new Dictionary<System.Type, SkillType>()
        {
            {typeof(OverrideSkillEntity),SkillType.Override },
            {typeof(NormalAttackEntity),SkillType.Normal },// 通常攻撃
            {typeof(TokenEntity),SkillType.Token }, // スキル内の付与効果
            {typeof(DummyEntity),SkillType.Dummy },
            {typeof(DangerEntity),SkillType.Danger },
            {typeof(ReviveEntity),SkillType.System },
            { typeof(FullRecoveryEntity),SkillType.System },
            { typeof(FullSkillBoostEntity),SkillType.System },
        };


        public IBattler Owner { get; private set; } // スキルの発動者

        public IEnumerable<IBattler>Targets { get; private set; } // スキルの影響を受ける対象
        
        public SkillCommand Command { get; private set; }         // スキルコマンド

        public AbstructSkillEntity Entity { get; private set; }   // スキルの本体データ

        public SkillType SkillType { get; private set; }          // スキルの種別

        public HitResultType HitType {  get; private set; }       // 攻撃失敗か否か

        public ValueCalcType CalcType { get; private set; }       // 計算方法種別

        public int CalcedAffectValue {  get; private set; }       // 計算後の効果量
        
        public bool IsHideView {  get; private set; }             // 効果量の表示を隠すか否か

        public bool IsMain {  get; private set; }                 // メインとして扱われるスキルか否か

        public bool IsRangedAttacker =>
            Owner.Unit.Entity.moveType == AffectMoveType.遠;      // 遠隔攻撃キャラであるか否か

        public bool IsRangedAttackable =>                         // 遠隔攻撃で実行可能か否か
            Command.categoryType.GetIsAttack() == true &&  // 攻撃スキルで
            Command.rangeType.GetIsMySide() == false;      // 対象が自分サイドではなく


        public bool IsRangedAttackSkill =>                        // 遠隔攻撃で実行するスキルであるか否か
            SkillType == SkillType.Override;

        public AffectInfo Clone()
        {
            var clone = (AffectInfo)MemberwiseClone();
            clone.Command=clone.Command.Clone();
            return clone;
        }

        public AffectInfo Copy(AffectInfo info)
        {
            this.IsHideView =           info.IsHideView;
            this.SkillType =            info.SkillType;
            this.Entity =               info.Entity;
            this.CalcedAffectValue=     info.CalcedAffectValue;
            this.Owner=                 info.Owner;
            this.Targets=               info.Targets;
            this.Command =              info.Command;
            this.IsMain =               info.IsMain; 
            this.HitType=               info.HitType;
            return this;
        }

        public AffectInfo SetAffectValue(int value)
        {
            this.CalcedAffectValue = value;
            return this;
        }

        public AffectInfo SetEntity(AbstructSkillEntity data)
        {
            this.SkillType = m_SkillEntityDict[data.GetType()];
            this.Entity = data;
            return this;
        }

        public AffectInfo SetOwner(IBattler owner)
        {
            this.Owner = owner;
            return this;
        }

        public AffectInfo SetTarget(IEnumerable<IBattler>targets)
        {
            this.Targets= targets;
            return this;
        }

        public AffectInfo SetTarget(IBattler target)
        {
            this.Targets = new IBattler[] { target };
            return this;
        }
        
        public AffectInfo SetCommand(SkillCommand command)
        {
            this.Command = command;
            this.CalcedAffectValue = command.affectValue;
            return this;
        }

        public AffectInfo SetMain(bool enable)
        {
            this.IsMain = enable;
            return this;
        }

        public AffectInfo SetIsHideNumeral(bool enable)
        {
            this.IsHideView = enable;
            return this;
        }

        public AffectInfo SetCalcType(ValueCalcType calcType)
        {
            this.CalcType = calcType;
            return this;
        }

        public AffectInfo CalcHitType(IBattler target)
        {
             this.HitType = Owner.GetCalcHitType(this, target);
            return this;
        }

        public AffectInfo CalcAffect(IBattler target)
        {
            if(CalcType==ValueCalcType.Normal)
            {
                this.CalcedAffectValue=Owner.GetCalcAffectValue(this, target);
            }
            return this;
        }

        public AffectInfo Charge(AffectInfo chargeTarget)
        {
            if(this.Entity==null)
            {
                this.Copy(chargeTarget);                // 無ければそのままコピー
                this.SetCalcType(ValueCalcType.Direct); // 数値直通設定
            }
            else
            {
                this.SetAffectValue(chargeTarget.CalcedAffectValue + this.CalcedAffectValue); // あれば数値を加算
            }

            chargeTarget.SetCalcType(ValueCalcType.Direct); // 数値直通設定
            chargeTarget.SetAffectValue(0);                 // チャージ元の効果量を空にする
            chargeTarget.SetIsHideNumeral(true);            // 効果量の表示を行わない
            return this;
        }

        public IActionNotify GetNotify()
        {
            switch (this.SkillType)
            {
                case SkillType.Token:
                case SkillType.Dummy:
                case SkillType.Normal:return Owner.AdventSkill;
                case SkillType.Override: return Owner.OverrideSkill;
                case SkillType.ExsaOverride:return Owner.OverrideSkill;
                case SkillType.SectaOverride:return Owner.OverrideSkill;
                case SkillType.QuetaOverride:return Owner.OverrideSkill;
                default: throw new System.Exception($"[SkillType {this.SkillType}] は対象外です。");
            }
        }

        public bool GetIsAffectable()
        {
            switch (this.HitType)
            {
                case HitResultType.Success: return true;
                case HitResultType.Guard: return false;
                case HitResultType.Miss: return false;
                default: throw new System.Exception($"[HitType{this.HitType}] は対象外です。");
            }
        }
    }
}
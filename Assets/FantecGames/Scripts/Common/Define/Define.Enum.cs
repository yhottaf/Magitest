using System;
using UnityEngine;
using UnityEngine.UI;

/*
 * -----[ Enum 規約 ]-----
 * 
 * 1.Enum名の後ろに Type をつける
 * 2.要素の先頭には NONE を入れる
 * 3.Enum をインデックスとしても利用することがある場合は NONE に -1 を入れる
 * 4.肥大してきたら別ファイルに分割
 * 
 */

namespace fantec
{
    // ----------------------------------------------------------------------------------------------------
    // カード
    // ----------------------------------------------------------------------------------------------------

    /// <summary>
    /// カードのレアリティの指定
    /// </summary>
    public enum CardRarityType
    {
        NONE = -1,

        R1 = 0, R2 = 1, R3 = 2, R4 = 3, R5 = 4, R6 = 5,
    }
    public static partial class EnumExtentions
    {
        /// <summary>
        /// レアリティに応じた最大レベルを取得する
        /// </summary>
        public static int GetMaxLevel(this CardRarityType rarityType)
        {
            switch (rarityType)
            {
                case CardRarityType.R1: return 20;
                case CardRarityType.R2: return 30;
                case CardRarityType.R3: return 50;
                case CardRarityType.R4: return 70;
                case CardRarityType.R5: return 80;
                case CardRarityType.R6: return 100;
                default: throw new Exception($"[{rarityType}] は対象外です。");
            }
        }

        /// <summary>
        /// レアリティ帯に応じた最小レベルを取得する
        /// </summary>
        public static int GetMinLevel(this CardRarityType rarityType)
        {
            switch (rarityType)
            {
                case CardRarityType.R1: return 1;
                case CardRarityType.R2: return 30;
                case CardRarityType.R3: return 40;
                case CardRarityType.R4: return 50;
                case CardRarityType.R5: return 60;
                case CardRarityType.R6: return 80;
                default: throw new Exception($"[{rarityType}] は対象外です。");
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // 効果
    // ----------------------------------------------------------------------------------------------------

    /// <summary>
    /// 効果属性
    /// </summary>
    public enum AffectAttributeType
    {
        NONE,
        通常,
        炎,
        氷,
        木,
        空,
        金,
        黒,
    }


    /// <summary>
    /// 効果量種別（インデックス番号はスキルIDに加算する形で利用されます）
    /// </summary>
    public enum AffectEfficacyType
    {
        NONE = 0,
        微 = 1,
        小 = 2,
        中 = 3,
        大 = 4,
        超 = 5,
        極 = 6,
    }

    public static partial class EnumExtentions
    {
        /// <summary>
        /// 効果量種別のコマンド用文字列を取得する
        /// </summary>
        public static string GetCommandString(this AffectEfficacyType @this)
        {
            switch (@this)
            {
                case AffectEfficacyType.NONE: return "NONE";
                case AffectEfficacyType.微: return "微";
                case AffectEfficacyType.小: return "小";
                case AffectEfficacyType.中: return "中";
                case AffectEfficacyType.大: return "大";
                case AffectEfficacyType.超: return "超";
                case AffectEfficacyType.極: return "極";

                default: throw new Exception($"[{@this}] のコマンド用文字列が設定されていません。");
            }
        }
    }


    /// <summary>
    /// 効果範囲
    /// </summary>
    public enum AffectRangeType
    {
        NONE,

        EnemySingle,
        MySideSingle,
        EnemyAll,
        MySideAll,
       // EnemyVanguard,
        //EnemyRearguard,
        MySideVanguard,
        MySideRearguard,
        Myself,
        BesidesMe,
        Gimmick,
        MySideAllRandom,
        EnemyAllRandom,
       // MySideVanguardRandom,
        //MySideRearguardRandom,
        //EnemyVanguardRandom,
        //EnemyRearguardRandom,
        BesideMeRandom,
    }

    public static partial class EnumExtentions
    {
        public static string GetCommandString(this AffectRangeType rangeType)
        {
            switch (rangeType)
            {
                case AffectRangeType.NONE: return "NONE";
                case AffectRangeType.EnemySingle: return "敵単体N";
                case AffectRangeType.EnemyAll: return "敵全体N";
                case AffectRangeType.EnemyAllRandom: return "敵全体R";
                case AffectRangeType.MySideAll: return "味方全体N";
                case AffectRangeType.MySideVanguard: return "味方前衛N";
                case AffectRangeType.MySideRearguard: return "味方後衛N";
                case AffectRangeType.MySideAllRandom: return "味方全体R";
                case AffectRangeType.MySideSingle: return "味方単体N";
                case AffectRangeType.Myself: return "自分自身N";
                case AffectRangeType.BesidesMe: return "自分以外N";
                case AffectRangeType.BesideMeRandom: return "自分以外R";
                case AffectRangeType.Gimmick: return "ギミック";

                default:
                    throw new Exception($"[rangeType : {rangeType}] のコマンド用文字列が設定されていません。");
            }
        }


        /// <summary>
        /// メインスキルとして識別する際などに利用する優先順位
        /// </summary>
        public static int GetPriority(this AffectRangeType rangeType)
        {
            switch (rangeType)
            {
                case AffectRangeType.EnemySingle:
                    return 0;

                case AffectRangeType.EnemyAll:
                    return 1;

                case AffectRangeType.EnemyAllRandom:
                    return 2;

                case AffectRangeType.MySideAll:
                    return 3;

                case AffectRangeType.MySideVanguard:
                case AffectRangeType.MySideRearguard:
                case AffectRangeType.MySideAllRandom:
                    return 4;

                case AffectRangeType.MySideSingle:
                case AffectRangeType.Myself:
                case AffectRangeType.BesidesMe:
                case AffectRangeType.BesideMeRandom:
                case AffectRangeType.Gimmick:
                    return 5;

                default:
                    throw new Exception($"[rangeType : {rangeType}] の優先順位が設定されていません。");
            }
        }

        /// <summary>
        /// 自分サイドへの効果か否か
        /// </summary>
        public static bool GetIsMySide(this AffectRangeType rangeType)
        {
            switch (rangeType)
            {
                case AffectRangeType.Myself:
                case AffectRangeType.BesidesMe:
                case AffectRangeType.MySideSingle:
                case AffectRangeType.MySideAll:
                case AffectRangeType.MySideVanguard:
                case AffectRangeType.MySideRearguard:
                case AffectRangeType.MySideAllRandom:
                case AffectRangeType.BesideMeRandom:
                case AffectRangeType.Gimmick:
                    return true;

                default: return false;
            }
        }
    }


    /// <summary>
    /// ターンの消費タイミング種別
    /// </summary>
    public enum AffectTurnConsumeType
    {
        NONE,
        TurnEnd,            // ターン終了後
        SectionByOwner,     // 発動者側
        SectionByTarget,    // 効果対象側
    }

    /// <summary>
    /// 効果種別
    /// </summary>
    public enum AffectCategoryType
    {
        NONE,               // 取得失敗時などの例外用
        EMPTY,              // 何もしない
        DUMMY,              // デバッグ用ダミースキル

        // 攻撃
        AttackNormal,       // 通常ダメージ（％）
        AttackFixed,        // 固定値ダメージ
        AttackRatio,        // 割合ダメージ（％）

        // 回復
        HealRatio,          // 割合回復（％）
        HealFixed,          // 固定値回復
        RegenRatio,         // 割合持続回復（％）
        RegenFixed,         // 固定値持続回復
        Revival,            // 蘇る
        Kill,               // 殺る
        Dispel,             // バフを解除

        // 状態変化
        Shield,             // シールド

        // 状態異常
        Poison,
        Burn,
        Frost,
        Blind,
        Confusion,
        OverrideSealed,
        Break,
        AbnormalHeal,

        // バフ・デバフ
        ATK_Buff,           // 攻撃力バフ
        BRK_Buff,           // ブレイク力バフ
        DEF_Buff,           // 防御力バフ
        SPD_Buff,           // スピードバフ
        DEX_Buff,           // 命中率バフ
        LUK_Buff,           // 運バフ
        VIT_Buff,           // 状態異常耐性バフ
        DMG_Buff,           // ダメージ上限バフ
        MOVE_Buff,          // 移動バフ
        ATK_Debuff,         // 攻撃力デバフ
        BRK_Debuff,         // ブレイク力デバフ
        DEF_Debuff,         // 防御力デバフ
        SPD_Debuff,         // スピードデバフ
        DEX_Debuff,         // 命中率デバフ
        LUK_Debuff,         // 運デバフ
        VIT_Debuff,         // 状態異常耐性デバフ
        DMG_Debuff,         // ダメージ上限デバフ
        HP_Buff,            // 体力上限バフ
        HP_Debuff,          // 体力上限デバフ
        BP_Buff,            // ブレイク耐久値上限バフ
        BP_Debuff,          // ブレイク耐久値上限デバフ
        MOVE_Debuff,        // 移動デバフ
        AttributeResistBuff,    // 属性耐性バフ
        AttributeResistDebuff,  // 属性耐性デバフ
        KONSHIN_ATK,        // 逆境攻撃力バフ
        KONSHIN_BRK,        // 逆境ブレイク力バフ
        GYAKKYOU_ATK,       // 渾身攻撃力バフ
        GYAKKYOU_BRK,       // 渾身ブレイク力バフ


        // 特殊
        Guts,
    }


    public static partial class EnumExtentions
    {
        public static string GetCommandString(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.NONE: return "NONE";
                case AffectCategoryType.EMPTY: return "EMPTY";
                case AffectCategoryType.DUMMY: return "DUMMY";

                case AffectCategoryType.AttackNormal: return "攻撃N";
                case AffectCategoryType.AttackFixed: return "攻撃F";
                case AffectCategoryType.AttackRatio: return "攻撃R";
                case AffectCategoryType.HealRatio: return "回復R";
                case AffectCategoryType.HealFixed: return "回復F";
                case AffectCategoryType.AbnormalHeal: return "異常回復";
                case AffectCategoryType.Revival: return "蘇生";
                case AffectCategoryType.Kill: return "即死";
                case AffectCategoryType.Dispel: return "ディスペル";
                case AffectCategoryType.Shield: return "盾";
                case AffectCategoryType.Guts: return "根性";
                case AffectCategoryType.Poison: return "毒";
                case AffectCategoryType.Frost: return "凍傷";
                case AffectCategoryType.Burn: return "火傷";
                case AffectCategoryType.Blind: return "暗闇";
                case AffectCategoryType.Confusion: return "混乱";
                case AffectCategoryType.Break: return "ブレイク";
                case AffectCategoryType.OverrideSealed:return "オーバーライド封印";
                case AffectCategoryType.GYAKKYOU_ATK: return "逆境攻";
                case AffectCategoryType.GYAKKYOU_BRK: return "逆境破";
                case AffectCategoryType.KONSHIN_ATK: return "渾身攻";
                case AffectCategoryType.KONSHIN_BRK: return "渾身破";
                case AffectCategoryType.RegenRatio: return "リジェネR";
                case AffectCategoryType.RegenFixed: return "リジェネF";
                case AffectCategoryType.ATK_Buff: return "ATK+";
                case AffectCategoryType.BRK_Buff: return "BRK+";
                case AffectCategoryType.DEF_Buff: return "DEF+";
                case AffectCategoryType.SPD_Buff: return "SPD+";
                case AffectCategoryType.DEX_Buff: return "DEX+";
                case AffectCategoryType.LUK_Buff: return "LUK+";
                case AffectCategoryType.VIT_Buff: return "VIT+";
                case AffectCategoryType.DMG_Buff: return "DMG+";
                case AffectCategoryType.ATK_Debuff: return "ATK-";
                case AffectCategoryType.BRK_Debuff: return "BRK-";
                case AffectCategoryType.DEF_Debuff: return "DEF-";
                case AffectCategoryType.SPD_Debuff: return "SPD-";
                case AffectCategoryType.DEX_Debuff: return "DEX-";
                case AffectCategoryType.LUK_Debuff: return "LUK-";
                case AffectCategoryType.VIT_Debuff: return "VIT-";
                case AffectCategoryType.DMG_Debuff: return "DMG-";
                case AffectCategoryType.HP_Buff: return "HP+";
                case AffectCategoryType.BP_Buff: return "BP+";
                case AffectCategoryType.HP_Debuff: return "HP-";
                case AffectCategoryType.BP_Debuff: return "BP-";
                case AffectCategoryType.AttributeResistBuff: return "属耐+";
                case AffectCategoryType.AttributeResistDebuff: return "属耐-";

                default:
                    throw new Exception($"[categoryType : {categoryType}] のコマンド用文字列が設定されていません。");
            }
        }

        /// <summary>
        /// 発動順などに利用される優先順位
        /// </summary>
        public static int GetPriority(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.AttackNormal:
                case AffectCategoryType.AttackFixed:
                case AffectCategoryType.AttackRatio:
                    return 0;

                case AffectCategoryType.HealRatio:
                case AffectCategoryType.HealFixed:
                case AffectCategoryType.AbnormalHeal:
                case AffectCategoryType.Revival:
                case AffectCategoryType.Kill:
                case AffectCategoryType.Dispel:
                    return 1;

                case AffectCategoryType.Shield:
                case AffectCategoryType.Guts:
                case AffectCategoryType.Poison:
                case AffectCategoryType.Frost:
                case AffectCategoryType.Burn:
                case AffectCategoryType.Blind:
                case AffectCategoryType.Confusion:
                case AffectCategoryType.GYAKKYOU_ATK:
                case AffectCategoryType.GYAKKYOU_BRK:
                case AffectCategoryType.KONSHIN_ATK:
                case AffectCategoryType.KONSHIN_BRK:
                case AffectCategoryType.RegenRatio:
                case AffectCategoryType.RegenFixed:
                    return 2;

                case AffectCategoryType.ATK_Buff:
                case AffectCategoryType.BRK_Buff:
                case AffectCategoryType.DEF_Buff:
                case AffectCategoryType.SPD_Buff:
                case AffectCategoryType.DEX_Buff:
                case AffectCategoryType.LUK_Buff:
                case AffectCategoryType.VIT_Buff:
                case AffectCategoryType.ATK_Debuff:
                case AffectCategoryType.BRK_Debuff:
                case AffectCategoryType.DEF_Debuff:
                case AffectCategoryType.SPD_Debuff:
                case AffectCategoryType.DEX_Debuff:
                case AffectCategoryType.LUK_Debuff:
                case AffectCategoryType.VIT_Debuff:
                case AffectCategoryType.HP_Buff:
                case AffectCategoryType.BP_Buff:
                case AffectCategoryType.HP_Debuff:
                case AffectCategoryType.BP_Debuff:
                case AffectCategoryType.AttributeResistBuff:
                case AffectCategoryType.AttributeResistDebuff:
                    return 3;

                default:
                    throw new Exception($"[categoryType : {categoryType}] の優先順位が設定されていません。");
            }
        }


        /// <summary>
        /// ターン消費タイミング種別を取得する
        /// </summary>
        public static AffectTurnConsumeType GetTurnConsumeType(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.ATK_Buff:
                case AffectCategoryType.ATK_Debuff:
                case AffectCategoryType.BRK_Buff:
                case AffectCategoryType.BRK_Debuff:
                case AffectCategoryType.DEX_Buff:
                case AffectCategoryType.DEX_Debuff:
                case AffectCategoryType.LUK_Buff:
                case AffectCategoryType.LUK_Debuff:
                    return AffectTurnConsumeType.SectionByOwner;

                case AffectCategoryType.DEF_Buff:
                case AffectCategoryType.DEF_Debuff:
                case AffectCategoryType.AttributeResistBuff:
                case AffectCategoryType.AttributeResistDebuff:
                    return AffectTurnConsumeType.SectionByTarget;

                default:
                    return AffectTurnConsumeType.TurnEnd;
            }
        }

        /// <summary>
        /// ダメージ系か否か
        /// </summary>
        public static bool GetIsAttack(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.AttackNormal:
                case AffectCategoryType.AttackFixed:
                case AffectCategoryType.AttackRatio:
                    return true;

                default: return false;
            }
        }

        public static bool GetIsBuff(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.ATK_Buff:
                case AffectCategoryType.BRK_Buff:
                case AffectCategoryType.DEF_Buff:
                case AffectCategoryType.SPD_Buff:
                case AffectCategoryType.DEX_Buff:
                case AffectCategoryType.LUK_Buff:
                case AffectCategoryType.VIT_Buff:
                case AffectCategoryType.HP_Buff:
                case AffectCategoryType.BP_Buff:
                case AffectCategoryType.AttributeResistBuff:
                    return true;

                default: return false;
            }
        }

        public static bool GetIsDebuff(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.ATK_Debuff:
                case AffectCategoryType.BRK_Debuff:
                case AffectCategoryType.DEF_Debuff:
                case AffectCategoryType.SPD_Debuff:
                case AffectCategoryType.DEX_Debuff:
                case AffectCategoryType.LUK_Debuff:
                case AffectCategoryType.VIT_Debuff:
                case AffectCategoryType.HP_Debuff:
                case AffectCategoryType.BP_Debuff:
                case AffectCategoryType.AttributeResistDebuff:
                    return true;

                default: return false;
            }
        }

        /// <summary>
        /// 状態異常系であるか否か
        /// </summary>
        public static bool GetIsAbnormalCondition(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.Poison:
                case AffectCategoryType.Burn:
                case AffectCategoryType.Frost:
                case AffectCategoryType.Blind:
                case AffectCategoryType.Confusion:
                case AffectCategoryType.Break:
                    return true;

                default: return false;
            }
        }

        /// <summary>
        /// アイコンが存在するスキルか否か
        /// </summary>
        public static bool GetIsBuffIconable(this AffectCategoryType categoryType)
        {
            return
                categoryType.GetIsBuff() ||
                categoryType.GetIsDebuff() ||
                categoryType.GetIsAbnormalCondition();
        }

        /// <summary>
        /// 攻撃的なスキルか否か
        /// </summary>
        public static bool GetIsAggressive(this AffectCategoryType categoryType)
        {
            return
                categoryType.GetIsAttack() |
                categoryType.GetIsAbnormalCondition() |
                categoryType.GetIsDebuff();
        }

        /// <summary>
        /// 付属の専用効果があるか否か
        /// </summary>
        public static bool GetIsAttachedEffect(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.Burn:          // バーン
                case AffectCategoryType.Frost:         // 凍結
                case AffectCategoryType.Blind:         // 暗黒
                case AffectCategoryType.Confusion:     // 混乱
                case AffectCategoryType.RegenRatio:     // 割合持続回復 (%)
                case AffectCategoryType.RegenFixed:     // 固定値持続回復
                    return true;

                default: return false;
            }
        }

        /// <summary>
        /// 効果付与直後に鮮度が付与されないものであるか否か
        /// </summary>
        public static bool GetIsUnfreshable(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.Poison:
                case AffectCategoryType.Burn:
                case AffectCategoryType.RegenRatio:
                case AffectCategoryType.RegenFixed:
                case AffectCategoryType.Break:
                    return true;

                default: return false;
            }
        }

        /// <summary>
        /// スリップダメージを与える系であるか否か
        /// </summary>
        public static bool GetIsSlipDamage(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.Poison:
                case AffectCategoryType.Burn:
                    return true;

                default: return false;
            }
        }

        /// <summary>
        /// 最大体力値に影響を与えるか否か
        /// </summary>
        public static bool GetIsMaxHealthable(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.HP_Buff:
                case AffectCategoryType.HP_Debuff:
                    return true;

                default: return false;
            }
        }

        /// <summary>
        /// ディスペルの効果対象か否か
        /// </summary>
        public static bool GetIsDispelable(this AffectCategoryType categoryType)
        {
            if (categoryType.GetIsBuff()) return true;

            switch (categoryType)
            {
                case AffectCategoryType.ATK_Buff:
                case AffectCategoryType.BRK_Buff:
                case AffectCategoryType.DEF_Buff:
                case AffectCategoryType.DEX_Buff:
                case AffectCategoryType.LUK_Buff:
                case AffectCategoryType.HP_Buff:
                case AffectCategoryType.BP_Buff:
                case AffectCategoryType.AttributeResistBuff:
                case AffectCategoryType.KONSHIN_ATK:
                case AffectCategoryType.KONSHIN_BRK:
                case AffectCategoryType.GYAKKYOU_ATK:
                case AffectCategoryType.GYAKKYOU_BRK:
                    return true;

                default: return false;
            }
        }

        /// <summary>
        /// 持続回復系であるか否か
        /// </summary>
        public static bool GetIsRegen(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.RegenRatio:
                case AffectCategoryType.RegenFixed:
                    return true;

                default: return false;
            }
        }


        /// <summary>
        /// 防御値に影響するか否か
        /// </summary>
        public static bool GetIsProtectable(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.AttackNormal:
                    return true;

                default: return false;
            }

        }


        /// <summary>
        /// ベースの値を元に計算するか否か
        /// </summary>
        public static bool GetIsCalcUseBaseValue(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.AttackNormal:
                    return true;

                default: return false;
            }
        }

        /// <summary>
        /// 割合での計算か否か
        /// </summary>
        public static bool GetIsCalcRasio(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.HealRatio:
                case AffectCategoryType.AttackRatio:
                case AffectCategoryType.RegenRatio:
                    return true;

                default: return false;
            }
        }

        /// <summary>
        /// 特定数値以上が条件であるか否か
        /// </summary>
        public static bool GetIsConditionsAbove(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.KONSHIN_ATK:
                case AffectCategoryType.KONSHIN_BRK:
                    return true;

                default: return false;
            }
        }

        /// <summary>
        /// 特定数値以下が条件であるか否か
        /// </summary>
        public static bool GetIsConditionsBelow(this AffectCategoryType categoryType)
        {
            switch (categoryType)
            {
                case AffectCategoryType.GYAKKYOU_ATK:
                case AffectCategoryType.GYAKKYOU_BRK:
                    return true;

                default: return false;
            }
        }
    }

    /// <summary>
    /// 移動先種別
    /// </summary>
    public enum AffectMoveType
    {
        NONE = -1,
        近,     // 近接攻撃
        遠,     // 遠距離攻撃
    }

    /// <summary>
    /// ヒット種別
    /// </summary>
    public enum AffectHitType
    {
        NONE,
        打, 斬, 突,
    }

    /// <summary>
    /// オーバーライトの種類
    /// </summary>
    public enum AffectOverrideType
    {
        NONE,
        Override,
        エクサオーバーライド,
        ゼタオーバーライド,
        クエタオーバーライド,
    }


    // ----------------------------------------------------------------------------------------------------
    // ステージ
    // ----------------------------------------------------------------------------------------------------

    public enum StageCategoryType
    {
        NONE,
        Normal,
        Event,
        Daily,
        Charange,
    }

    public enum StagePlayType
    {
        NONE,
        Battle,
        Story,
    }

    public enum StageDifficultyType
    {
        NONE = -1,

        Lv1, Lv2, Lv3, Lv4, Lv5, Lv6, Lv7, Lv8, Lv9, Lv10,

        Normal,
        Hard,
        VearyHard,
    }

    // ----------------------------------------------------------------------------------------------------
    // アイテム
    // ----------------------------------------------------------------------------------------------------

    /// <summary>
    /// 消費アイテムの種類
    /// </summary>
    public enum ConsumeItemEffectType
    {
        NONE,
        強化,
        素材,
        ガチャ,
        スタミナ,
        クエスト,
        ウォレット,
    }

    // ----------------------------------------------------------------------------------------------------
    // フッターの種類
    // ----------------------------------------------------------------------------------------------------
    public enum FooterType
    {
        NONE = -1,
        Home = 0,
        Quest=1,
        //PartyEdit = 1,
        //CardList = 2,
        //Gacha = 3,
    }

    //------------------------------------------------------------------------------------------------------
    //  スクロール方向
    //------------------------------------------------------------------------------------------------------
    public enum ScrollDirection
    {
        Vertical,
        Horizontal,
    }

    //------------------------------------------------------------------------------------------------------
    //  スクロールの動き
    //------------------------------------------------------------------------------------------------------
    public enum MovementType
    {
        Unrestricted = ScrollRect.MovementType.Unrestricted,
        Elastic = ScrollRect.MovementType.Elastic,
        Clamped = ScrollRect.MovementType.Clamped
    }

    //------------------------------------------------------------------------------------------------------
    //  動く方向
    //------------------------------------------------------------------------------------------------------

    public enum MovementDirection
    {
        Left,
        Right,
        Up,
        Down,
    }
}

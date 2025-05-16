using fantec.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace fantec.Battle.Model
{
    public interface IBattlerParamState : IDisposable, IResetable, IReloadable
    {
        IObservable<List<TokenCell>> OnUpdateTokenCellList { get; }
        IObservable<AffectInfo> OnTakeBuffObservable { get; } // バフを受けた際を購読

        int CurrentUnitID{ get; } // そのカードの固有ID (オーバーライドスキルを発動するのに見る値)

        int CurrentATK { get; } // 現在の攻撃力
        int CurrentSPD { get; } // 現在の素早さ
        int CurrentDEX { get; } // 現在の命中率
        int CurrentLUK { get; } // 現在の運
        int CurrentVIT { get; } // 現在の異常耐性値
        int CurrentDMG { get; } // 現在のダメージ上限値
        int CurrentHP { get; } // 現在の体力値
        int CurrentMaxHP { get; } // 現在の最大体力値
        int CurrentMOVE { get; } // 現在の移動力

        int OriginalATK { get; } // 元の攻撃力
        int OriginalSPD { get; } // 元の素早さ
        int OriginalDEX { get; } // 元の命中率
        int OriginalLUK { get; } // 元の運
        int OriginalVIT { get; } // 元の状態異常耐性値
        int OriginalDMG { get; } // 元のダメージ上限値
        int OriginalMaxHP { get; }// 元の最大体力値
        int OriginalMOVE { get; } // 元の移動力

        int BuffATK { get; }      // 攻撃力のバフ値
        int BuffSPD { get; }      // 素早さのバフ値
        int BuffDEX { get; }      // 命中率のバフ値
        int BuffLUK { get; }      // 運のバフ値
        int BuffVIT { get; }      // 状態異常耐性のバフ値
        int BuffDMG { get; }      // ダメージ上限値のバフ値
        int BuffMaxHP { get; }    // 最大体力値のバフ値
        int BuffMOVE { get; }     // 移動力のバフ値
        int Spec { get; }         // バフ含めた能力の合計値

        float RatioHP { get; }    // HPの割合の取得

        bool IsConfusion { get; } // 混乱しているか否か

        bool IsShield { get; }    // シールドが付与されているか否か


        List<TokenCell> TokenList { get; }
        CardStateEntity Entity { get; }
        StateHealth Health { get; }


        void TakeToken(AffectInfo info);  // トークンを付与する
        void TakeDispel(AffectInfo info); // バフ系トークンを１つ取り除く
        void TakeAbnormalRecobery(AffectInfo info); // 状態異常を取り除く
        void RemoveToken(TokenCell tokenCell);      // 指定のトークンを取り除く
        void RemoveToken(IEnumerable<TokenCell> tokenCells); // 指定のトークンをまとめて取り除く
        void Progress(AffectTurnConsumeType buffType);       // 保有しているトークンのターンを進める
        int GetBuffValue(AffectCategoryType categoryType);   // 指定カテゴリのバグ合計値を取得
        int GetAttributeResisCurrentValue(AffectAttributeType attributeType); // 指定属性の耐性値を取得
    }

    public interface IBattlerParamStatePrivate:IBattlerParamState
    {

    }

    public static class BattlerParamStatePrivateExtentions
    {
        public static void UpdateProgress(this IBattlerParamStatePrivate @this,AffectTurnConsumeType turnConsumeType)
        {
            // リストに変換
            var tokenList = @this.TokenList.ToList();

            // トークンを後ろから列挙 (要素を取り除いた際のズレ対応のために後ろから)
            for (int i = @this.TokenList.Count - 1; i >= 0; i--)
            {
                var token = tokenList[i];

                // トークン付与直後でなく、バフタイプが一致していれば
                if(token.IsFresh==false&& token.GetTurnConsumeType()==turnConsumeType)
                {
                    // ターンを進行させる
                    token.Progress();
                }

                //  消費可能であれば
                if(token.IsConsumable)
                {
                    // 取り除く
                    @this.RemoveToken(token);
                }

                if(token.IsFresh==true&&AffectTurnConsumeType.TurnEnd==turnConsumeType)
                {
                    // 鮮度を落とす
                    token.FleshDrop();
                }
            }
        }

        public static int GetValue(this IBattlerParamStatePrivate @this,AffectCategoryType categoryType,float normalizedValue)
        {
            var result = 0;

            foreach(var token in @this.TokenList)
            {
                // 効果種別が一致していれば
                if(token.command.categoryType==categoryType)
                {
                    // 指定値以上を条件とする場合
                    if(token.command.parentCategoryType.GetIsConditionsAbove())
                    {
                        // 条件を満たしていれば効果量を加算
                        if (token.GetIsAboveNormalizedValue(normalizedValue))
                            result += token.command.affectValue;
                    }
                    else
                    // 指定値以下を条件とする場合
                    if(token.command.parentCategoryType.GetIsConditionsBelow())
                    {
                        // 条件を満たしていれば効果量を加算
                        if (token.GetIsBelowNormalizedValue(normalizedValue))
                            result += token.command.affectValue;
                    }
                    // それ以外であれば
                    else
                    {
                        // 基本はここで加算される
                        result+=token.command.affectValue;
                    }
                }
            }

            return result;
        }
    }
}
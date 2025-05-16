using DG.Tweening;
using fantec.Battle.Utiles;
using System;
using UnityEngine;

namespace fantec.Battle.Manager.Flow
{
    public interface ICombatSequence : IDisposable
    {
        SaveableSequence SavedSequence { get; }

        void Execute();
    }

    public static class CombatSequenceExtentions
    {
        /// <summary>
        /// 速度可変に対応したシーケンスを作成し返す
        /// </summary>
        public static Sequence CreateSequence(this ICombatSequence @this)
        {
            var sequence = DOTween.Sequence();
            @this.SavedSequence.Kill();
            @this.SavedSequence.Value = sequence;
            return sequence;
        }

        /// <summary>
        /// スキル効果反映時のシーケンス
        /// </summary>
        public static Sequence GetAffectSequence(this ICombatSequence @this,AffectInfoBox infoBox,Action<AffectInfo>onAffect)
        {
            var delay = 0.1f;    // 連撃の間隔

            // シーケンスのテンプレートを準備
            Sequence Template(AffectInfo info)
            {
                return DOTween.Sequence()
                    .AppendCallback(() =>
                    {
                        onAffect.Invoke(info);
                    })
                    .AppendInterval(delay)
                    .SetLoops(info.Command.actionCount);
            }

            // 戻り値の準備
            var result = DOTween.Sequence();

            // 攻撃前に発動するスキル
            foreach(var info in infoBox.GetFirstInfos())
            {
                result.Join(Template(info));
            }

            // 攻撃スキル
            foreach(var info in infoBox.GetAttackInfos())
            {
                result.Join(Template(info));
            }

            // 終わるまで待つ
            result.AppendInterval(delay);

            // 攻撃後に発動するスキル
            foreach (var info in infoBox.GetLateInfos())
            {
                result.Join(Template(info));
            }
            return result;

        }
    }
}
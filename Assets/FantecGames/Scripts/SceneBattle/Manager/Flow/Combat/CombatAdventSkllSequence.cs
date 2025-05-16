using DG.Tweening;
using fantec.Battle.Model;
using fantec.Battle.Utiles;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Manager.Flow
{
    public class CombatAdventSkillSequence : ICombatSequence
    {
        public SaveableSequence SavedSequence => m_Sequence;

        private readonly SaveableSequence m_Sequence;
        private readonly Action m_OnCompleted;
        private readonly float m_AttackInterval = 1.5f;
        private readonly float m_TurnEndInterval = 3.0f;
        public CombatAdventSkillSequence(SaveableSequence sequence, Action onCompleted)
        {
            m_Sequence = sequence;
            m_OnCompleted = onCompleted;
        }

        /// <summary>
        /// ターンを進める
        /// </summary>
        public void Execute()
        {
            // シーケンスの開始
            SectionSetup();
        }

        public void Dispose()
        {
            m_Sequence.Kill();
        }

        #region Section

        private void SectionSetup()
        {
            // 効果情報を作成
            var affector = Locator.Resolve<IBattleModelAdventSkill>().ReserveHead;
            var entity = affector.AdventSkill.HeadEntity;
            var infoBox = new AffectInfoBox(entity, affector);

            SectionMove(infoBox);
        }

        private void SectionActivate(AffectInfoBox infoBox)
        {
            Affect.Activation(infoBox, () =>
            {
                SectionAffection(infoBox);
            });
        }

        private void SectionAffection(AffectInfoBox infoBox)
        {
            var sequence = this.CreateSequence();
            var modelUnits = Locator.Resolve<IBattleModelUnits>();
            var mainInfo = infoBox.GetMain();

            sequence.Append(this.GetAffectSequence(infoBox, info =>
            {
                 Affect.Execute(info);                     // 効果の反映

                if (Locator.Resolve<IBattleModelUnits>().IsSettled) // 決着がついていれば
                {
                    SectionTurnEnd(infoBox);             // ターン終了へ
                }
            }));
            sequence.OnComplete(() =>
            {
                        infoBox.Owner.State.Progress(AffectTurnConsumeType.SectionByOwner);  // 発動者の効果持続ターンの更新

                        foreach (var battler in infoBox.GetAttackedTargets())                 // ターゲットの効果持続ターンの更新
                        {
                            battler.State.Progress(AffectTurnConsumeType.SectionByTarget);
                        }

                        if (Locator.Resolve<IBattleModelUnits>().IsSettled)              // 決着がついていれば
                        {
                            Locator.Resolve<IBattleModelAdventSkill>().ConsumeAll();  // 全て消費
                        }
                        else                                                            // 戦闘中であれば
                        {
                            Locator.Resolve<IBattleModelAdventSkill>().Consume();     // 発動分を消費
                        }

                        if (Locator.Resolve<IBattleModelAdventSkill>().IsConsumable)  // 発動待ちスキルがあれば
                        {
                            SectionReTurn();         // 次のスキル
                        }
                        else                         // すべて消費済みであれば
                        {
                            SectionTurnEnd(infoBox); // ターンの終了
                        }
       
            });
        }

        private void SectionMove(AffectInfoBox infoBox)
        {
            var owner = infoBox.Owner;
            int movePower = owner.State.CurrentMOVE; // 移動力
            MoveStepRecursive(owner, infoBox, movePower);
        }

        private void MoveStepRecursive(IBattler owner, AffectInfoBox infoBox, int remainingMoves)
        {
            if (remainingMoves <= 0)
            {
                if (Locator.Resolve<IBattleModelOverrideSkill>().OnIsFook.Value)
                {
                    IEnumerable<IBattler> samePositionOthers = Locator.Resolve<IBattleModelUnits>()
                                                            .PlayerDatas
                                                            .GetExistBattlers()
                                                            .GetSamePositionIndexOthers(owner);

                    foreach (IBattler other in samePositionOthers)
                    {
                        owner.OverrideSkill.ActivateBlur();
                    }
                    owner.OverrideSkill.ActivateBlur();

                    // DOTween でブラックアウトアニメーション後にターン終了
                    var sequenceBlackOut = DOTween.Sequence();
                    sequenceBlackOut.AppendCallback(() =>
                    {
                        Locator.Resolve<IBattleAnimationManager>().Play<IBlackoutAnimation>();
                    });
                    sequenceBlackOut.AppendInterval(0.1f); // 少し待ってから終了
                    sequenceBlackOut.AppendCallback(() =>
                    {
                        SectionTurnEnd(infoBox);
                    });
                }
                else
                {
                    // DOTween でディレイ後に SectionActivate 呼び出し
                    var Normalsequence = DOTween.Sequence();
                    Normalsequence.AppendInterval(m_AttackInterval);
                    Normalsequence.AppendCallback(() =>
                    {
                        SectionActivate(infoBox);
                    });
                }
                return;
            }

            // 次のマスを計算して設定
            owner.Unit.Entity.positionIndex = Locator.Resolve<IBattlePlacementManager>().GetNextPositionIndex(owner);

            if (remainingMoves == 1)
            {
                RegisterOverrideIfNeeded(owner);
            }

            var sequence = this.CreateSequence();

            sequence.AppendInterval(0.2f); // 前の処理との間隔

            // 少し間をおいてから移動開始
            sequence.AppendCallback(() =>
            {
                owner.Transform.OnMoveCompletedObservable
                    .First()
                    .Subscribe(__ =>
                    {
                        MoveStepRecursive(owner, infoBox, remainingMoves - 1); // 再帰的に次のステップへ
                    });

                owner.Transform.Move(Locator.Resolve<IBattlePlacementManager>().GetBattlerPosition(owner)); // 1マス移動
            });
        }

        /// <summary>
        /// 次のスキル
        /// </summary>
        private void SectionReTurn()
        {
            var sequence = this.CreateSequence();
            sequence.AppendInterval(0.4f);
            sequence.OnComplete(() =>
            {
                Execute();
            });
        }

        private void SectionTurnEnd(AffectInfoBox infoBox)
        {
            var owner = infoBox.Owner;

            var sequence = this.CreateSequence();

            owner.Transform.SuppressMoveCompleteNotify = true;

            sequence.AppendInterval(0.2f);

            if (Locator.Resolve<IBattleModelOverrideSkill>().OnIsFook.Value==false)
            {
                sequence.AppendCallback(() =>
            {
                Locator.Resolve<IBattleModelAdventSkill>().ConsumeAll();   // 全て消費
                Locator.Resolve<IBattleModelAdventSkill>().Lottery();
                Locator.Resolve<IBattleModelAdventSkill>().SortByHeadInsert(); // 攻撃順の更新
                owner.Transform.Move(Locator.Resolve<IBattlePlacementManager>().GetBattlerPosition(owner)); // 攻撃者が元の位置に戻る
               //  Debug.Log($"攻撃後の場所:{Locator.Resolve<IBattlePlacementManager>().GetBattlerPosition(owner)}");
            });
            }

            sequence.OnComplete(() =>
            {
                // DOTween で後続処理を追加
                var postSequence =this.CreateSequence();

                postSequence.AppendCallback(() =>
                {
                    owner.Transform.SuppressMoveCompleteNotify = false;
                });

                if (Locator.Resolve<IBattleModelOverrideSkill>().OnIsFook.Value)
                {
                    postSequence.AppendInterval(m_AttackInterval);
                    postSequence.AppendCallback(() =>
                    {
                        m_OnCompleted.Invoke();
                    });
                }
                else
                {
                    postSequence.AppendInterval(m_TurnEndInterval);
                    postSequence.AppendCallback(() =>
                    {
                        owner.State.Progress(AffectTurnConsumeType.TurnEnd);
                        m_OnCompleted.Invoke();
                    });
                }
            });
        }

        #endregion

        private void RegisterOverrideIfNeeded(IBattler owner)
        {
            IEnumerable<IBattler> samePositionOthers;

            if (owner.GetIsPlayer())
            {
                samePositionOthers = Locator.Resolve<IBattleModelUnits>()
                                            .PlayerDatas
                                            .GetSurvivedBattlers()
                                            .GetSamePositionIndexOthers(owner);
            }
            else
            {
                samePositionOthers = Locator.Resolve<IBattleModelUnits>()
                                            .EnemyDatas
                                            .GetSurvivedBattlers()
                                            .GetSamePositionIndexOthers(owner);
            }

            if (samePositionOthers.Any())
            {
                // 自分＋同位置の他者の originId を配列にまとめる
                int[] originIds = new[] { owner.State.Entity.originId }
                    .Concat(samePositionOthers.Select(b => b.State.Entity.originId))
                    .ToArray();

                owner.OverrideSkill.SetEntity(originIds);

                // オーバーライドスキルの予約
                Locator.Resolve<IBattleModelOverrideSkill>().Reserve(owner);
            }
        }
    }
}
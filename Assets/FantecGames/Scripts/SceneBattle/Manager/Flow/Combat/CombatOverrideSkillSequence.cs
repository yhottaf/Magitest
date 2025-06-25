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
    public class CombatOverrideSkillSequence : ICombatSequence
    {
        public SaveableSequence SavedSequence => m_Sequence;

        private readonly SaveableSequence m_Sequence;
        private readonly Action m_OnCompleted;

        public CombatOverrideSkillSequence(SaveableSequence sequence, Action onCompleted)
        {
            m_Sequence = sequence;
            m_OnCompleted = onCompleted;
        }

        /// <summary>
        /// ターンを進める
        /// </summary>
        public void Execute()
        {
            var affector = Locator.Resolve<IBattleModelOverrideSkill>().GetReserveHeadBattler();
            AffectOverrideType type = affector.OverrideSkill.Entity.overrideType;

            // シーケンスの開始
            switch(type)
            {
                case AffectOverrideType.Override:
                    SectionSetupOverride();
                    break;
                case AffectOverrideType.エクサオーバーライド:
                    SectionSetupExsaOverride();
                    break;
                case AffectOverrideType.ゼタオーバーライド:
                    SectionSetupExsaOverride(); // 仮置き
                    break;
                case AffectOverrideType.クエタオーバーライド:
                    SectionSetupExsaOverride(); // 仮置き
                    break;
            }
        }

        public void Dispose()
        {
            m_Sequence.Kill();
        }

        #region Section

        #region SectionOverride
        private void SectionSetupOverride()
        {
            // 行動主の効果情報を作成
            var affector = Locator.Resolve<IBattleModelOverrideSkill>().GetReserveHeadBattler();

            var affectInfoList = new List<AffectInfoBox>();

            // 一緒に攻撃する仲間の選定
            IEnumerable<IBattler> samePositionOthers;
            if (affector.GetIsPlayer())
            {
                // 行動主がプレイヤーならプレイヤー側で重なっていパートナー達の取得
                samePositionOthers = Locator.Resolve<IBattleModelUnits>()
                                    .PlayerDatas
                                    .GetExistBattlers()
                                    .GetSamePositionIndexOthers(affector);
            }
            else
            {
                // 行動主がエネミーならエネミー側で重なったいたパートナー達の取得
                samePositionOthers = Locator.Resolve<IBattleModelUnits>()
                                    .EnemyDatas
                                    .GetExistBattlers()
                                    .GetSamePositionIndexOthers(affector);
            }


            // 通常攻撃
            var normalEntity = affector.AdventSkill.NormalEntity;
            affectInfoList.Add(new AffectInfoBox(normalEntity, affector));

            // オーバーライドの場合は必ず2人で行うので
            // GetRandomBattler()でその1人を取得できる
            var PartnerChara = samePositionOthers.GetRandomBattler(); // 行動主の相方
            var normalEntity2 = PartnerChara.AdventSkill.NormalEntity;// 相方の通常攻撃
      //      var normalEntity2 = DummyEntity.GetEntity(); //ただ見ているだけ
            
            affectInfoList.Add(new AffectInfoBox(normalEntity2, PartnerChara));

            // 並列実行
            SectionAffection(affectInfoList);
        }


        ///// <summary>
        ///// 効果の反映
        ///// </summary>
        private void SectionAffection(List<AffectInfoBox> infoboxes)
        {
            Affect.Activation(infoboxes, () =>
            {
                SectionActivateStaggered(infoboxes);
            });
        }

        /// <summary>
        /// スキル行動と反映
        /// </summary>
        /// <param name="infoBox"></param>
        private void SectionActivateStaggered(List<AffectInfoBox> infoBoxes)
        {
            var cameraManager = Locator.Resolve<IBattleCameraManager>();

            if (infoBoxes.Count == 0)
            {
                m_OnCompleted?.Invoke();
                return;
            }

            var sequence = this.CreateSequence();


            float delayPerIndex = 0.1f;

            var allInfos = new List<AffectInfo>();

            foreach (var (infoBox, index) in infoBoxes.Select((box, i) => (box, i)))
            {
                sequence.Insert(index * delayPerIndex, GetActivationWaitTween(0, () =>
                {
                    allInfos.AddRange(infoBox.GetAttackInfos());
                }));
            }

            sequence.AppendCallback(() =>
            {
                foreach (var info in allInfos)
                {
                    Affect.Execute(info);
                }
            });

            sequence.OnComplete(() =>
            {
                SectionTurnEnd(infoBoxes[0]);
            });
        }

        private Tween GetActivationWaitTween(float delay, Action onComplete)
        {
            return DOVirtual.DelayedCall(delay, () =>
            {
                onComplete?.Invoke();
            });
        }
        #endregion

        #region SectionExsaOverride
        public void SectionSetupExsaOverride()
        {
            // 行動主の効果情報を作成
            var affector = Locator.Resolve<IBattleModelOverrideSkill>().GetReserveHeadBattler();

            var affectInfoList = new List<AffectInfoBox>();

            // 一緒に攻撃する仲間の選定
            IEnumerable<IBattler> samePositionOthers;
            if (affector.GetIsPlayer())
            {
                // 行動主がプレイヤーならプレイヤー側で重なっていパートナー達の取得
                samePositionOthers = Locator.Resolve<IBattleModelUnits>()
                                    .PlayerDatas
                                    .GetExistBattlers()
                                    .GetSamePositionIndexOthers(affector);
            }
            else
            {
                // 行動主がエネミーならエネミー側で重なったいたパートナー達の取得
                samePositionOthers = Locator.Resolve<IBattleModelUnits>()
                                    .EnemyDatas
                                    .GetExistBattlers()
                                    .GetSamePositionIndexOthers(affector);
            }

            // 通常攻撃
            var normalEntity = affector.AdventSkill.NormalEntity;
            affectInfoList.Add(new AffectInfoBox(normalEntity, affector));

            // オーバーライドの場合は必ず2人で行うので
            // GetRandomBattler()でその1人を取得できる
            var PartnerChara = samePositionOthers.GetRandomBattler(); // 行動主の相方
            var normalEntity2 = PartnerChara.AdventSkill.NormalEntity;// 相方の通常攻撃
            affectInfoList.Add(new AffectInfoBox(normalEntity2, PartnerChara));

            // 並列実行
            SectionAffection(affectInfoList);
        }
        #endregion


        /// <summary>
        /// ターンの終了
        /// </summary>
        private void SectionTurnEnd(AffectInfoBox infoBox)
        {
            // シーケンスの作成
            var sequence = this.CreateSequence();
           
            // 自身の取得(このターンの行動主)
            var owner = infoBox.Owner;

            // パートナー達の情報取得(一緒にオーバードライブした人達)
            IEnumerable<IBattler> samePositionOthers;
            if (owner.GetIsPlayer())
            {
                // 行動主がプレイヤーならプレイヤー側で重なっていパートナー達の取得
                samePositionOthers = Locator.Resolve<IBattleModelUnits>()
                                    .PlayerDatas
                                    .GetExistBattlers()
                                    .GetSamePositionIndexOthers(owner);
            }
            else
            {
                // 行動主がエネミーならエネミー側で重なったいたパートナー達の取得
                samePositionOthers = Locator.Resolve<IBattleModelUnits>()
                          .EnemyDatas
                          .GetExistBattlers()
                          .GetSamePositionIndexOthers(owner);
            }

            // 攻撃移動から元の位置に帰ってくる際の通知は消す
            // (trueで戻りの移動完了通知を消す)
            foreach (var chara in samePositionOthers)
            {
                chara.Transform.SuppressMoveCompleteNotify = true;
            }

            owner.Transform.SuppressMoveCompleteNotify = true;

            sequence.AppendInterval(0.2f);

            sequence.AppendCallback(() =>
            {
                // 暗点終了
                Locator.Resolve<IBattleAnimationManager>().Get<IBlackoutAnimation>().Hide();


                // スキル終了通知
                Locator.Resolve<IBattleModelOverrideSkill>().Deactivate();
                Locator.Resolve<IBattleModelAdventSkill>().ConsumeAll();   // 全て消費
                Locator.Resolve<IBattleModelAdventSkill>().Lottery();
                Locator.Resolve<IBattleModelAdventSkill>().SortByHeadInsert(); // 攻撃順の更新

                // 攻撃者が元の位置に戻る
                infoBox.Owner.Transform.Move(Locator.Resolve<IBattlePlacementManager>().GetBattlerPosition(infoBox.Owner));


            });
            sequence.AppendInterval(0.2f);

            sequence.AppendCallback(() =>
            {
                // 攻撃主から0.2秒遅れてパートナーたちも元の位置に戻す
                foreach (var chara in samePositionOthers)
                {
                    chara.Transform.Move(Locator.Resolve<IBattlePlacementManager>().GetBattlerPosition(infoBox.Owner));
                }
            });

            sequence.AppendInterval(3.5f);
            sequence.AppendCallback(() =>
            {
                // 全ての味方キャラクターの移動の完了通知が飛ぶように、再び有効にもどしておく(falseで有効)
                owner.Transform.SuppressMoveCompleteNotify = false;
                foreach (var chara in samePositionOthers)
                {
                    chara.Transform.SuppressMoveCompleteNotify = false;
                }

                // 今回行動したキャラのターンを終了する
                owner.State.Progress(AffectTurnConsumeType.TurnEnd);
                // 終了通知
                m_OnCompleted();
            });
        }

        #endregion
    }
}
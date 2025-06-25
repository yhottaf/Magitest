using DG.Tweening;
using fantec.Battle.Model;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public partial class BattleFlowManager
    {
        public class FlowRevive : FlowBase
        {
            public override void OnEnter(BattleFlowManager manager, FlowBase prevFlow)
            {
                var modelUnits = Locator.Resolve<IBattleModelUnits>();
                var modelStage=Locator.Resolve<IBattleModelStage>();
                var modelAdvent = Locator.Resolve<IBattleModelAdventSkill>();

                // Bgmを再生する
                Locator.Resolve<IBattleSoundManager>().PlayBgm(modelStage.GetBattleBgmName());

                {
                    // 味方復活
                    var entity = ReviveEntity.GetEntity();
                    var owner = modelUnits.PlayerDatas.GetExistBattlers().GetRandomBattler();// 適当に味方を発動者に指定
                    var infoBox=new AffectInfoBox(entity, owner);
                    Affect.Execute(infoBox.infoList);
                }

                {
                    // 味方全回復処理
                    var entity = FullRecoveryEntity.GetEntity();
                    var owner = modelUnits.PlayerDatas.GetExistBattlers().GetRandomBattler();  // 適当に味方を発動者とする
                    var infoBox = new AffectInfoBox(entity, owner).SetIsHideNumeral(true);
                    Affect.Execute(infoBox.infoList);
                }

                {
                    // その他効果を乗せるならここに記載

                }

                // 順番の再抽選
                modelAdvent.SortByHeadInsertDefinitelyFirstPlayer();

                // ターン数のリセットを行う
                modelStage.ResetTurn();

                // TODO: 入力可能なUIを復帰させるならここで行う

                manager.m_Sequence.Value = DOTween.Sequence(manager)
                    .AppendInterval(1.0f)
                    .OnComplete(() =>
                    {
                        manager.ChangeFlow<FlowCombat>();
                    });
            }
        }
    }
}
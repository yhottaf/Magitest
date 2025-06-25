using fantec.Battle.Model;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public partial class BattleFlowManager
    {
        public class FlowJudge : FlowBase
        {
            public override void OnEnter(BattleFlowManager manager, FlowBase prevFlow)
            {
                var modelUnits = Locator.Resolve<IBattleModelUnits>();
                var modelStage=Locator.Resolve<IBattleModelStage>();

                // 最大ウェーブに達し、エネミーを全滅させた場合か、
                // 最大ウェーブに達し、最大ターンに到達した段階でプレイヤーのHP割合がエネミーより勝っていたら
                // 勝利へ
                if ((modelStage.IsMaxWave && modelUnits.IsEnemyDefeat) ||
                    (modelStage.IsMaxTurn && modelStage.IsMaxWave &&
                    (modelUnits.PlayerHPRatio > modelUnits.EnemyHPRatio))
                    )
                {
                    manager.ChangeFlow<FlowWin>();
                }
                // 味方が全滅した場合か、最大ターンに到達していてエネミーが優勢の場合敗北へ
                else if (modelUnits.IsPlayerDefeat ||
                    (modelStage.IsMaxTurn &&
                    (modelUnits.PlayerHPRatio <= modelUnits.EnemyHPRatio)))
                {
                     manager.ChangeFlow<FlowLose>();
                }
                // まだウェーブが残っている場合次ウェーブ準備へ
                else
                {
                    modelStage.NextWave();
                    manager.ChangeFlow<FlowWaveSetup>();
                }
            }
        }
    }
}
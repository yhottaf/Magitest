using DG.Tweening;
using fantec.Battle.Model;
using fantec.Common;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public partial class BattleFlowManager
    {
        public class FlowGimmick : FlowBase
        {
            public override void OnEnter(BattleFlowManager manager, FlowBase prevFlow)
            {
                var modelUnits = Locator.Resolve<IBattleModelUnits>();

                // バフデータのマスターデータなどあれば読み込み DOTOs

                var sequence = DOTween.Sequence();

                sequence.AppendInterval(1.0f);
                sequence.OnComplete(() => manager.ChangeFlow<FlowWaveSetup>());

                manager.m_Sequence.Value= sequence;
            }
        }
    }
}
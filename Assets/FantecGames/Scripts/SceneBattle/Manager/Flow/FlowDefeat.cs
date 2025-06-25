using DG.Tweening;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public partial class BattleFlowManager
    {
        public class FlowDefeat : FlowBase
        {
            public override void OnEnter(BattleFlowManager manager, FlowBase prevFlow)
            {
                // 敵、味方の撃破演出を少し待ってから判定へ
                manager.m_Sequence.Value = DOTween.Sequence()
                    .AppendInterval(1.0f)
                    .AppendCallback(() =>
                    {
                        manager.ChangeFlow<FlowJudge>();
                    })
                    .SetLink(manager.gameObject);
            }
        }
    }
}
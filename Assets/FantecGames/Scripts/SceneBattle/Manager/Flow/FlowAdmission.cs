using DG.Tweening;
using UnityEngine;


namespace fantec.Battle.Manager
{
    public partial class BattleFlowManager
    {
        public class FlowAdmission : FlowBase
        {
            public override void OnEnter(BattleFlowManager manager, FlowBase prevFlow)
            {
                manager.m_Sequence.Value = DOTween.Sequence()
                    .AppendInterval(0.5f)
                    .OnComplete(() =>
                    {
                        manager.ChangeFlow<FlowGimmick>();
                    })
                    .SetLink(manager.gameObject);
            }
        }
    }
}
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
                // “GA–¡•û‚ÌŒ‚”j‰‰o‚ð­‚µ‘Ò‚Á‚Ä‚©‚ç”»’è‚Ö
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
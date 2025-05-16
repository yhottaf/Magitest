using Cysharp.Threading.Tasks;
using fantec.Common;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public partial class BattleFlowManager
    {
        public class FlowRematch : FlowBase
        {
            public override void OnEnter(BattleFlowManager manager, FlowBase prevFlow)
            {
                Loading.Show(0.5f, 1f, () =>
                {
                    Locator.Resolve<IBattleWindowManager>().Clear();

                    // TODO:UI‚Ü‚Æ‚ß‚ÄƒŠƒZƒbƒg


                    manager.ChangeFlow<FlowInit>();
                });
            }
        }
    }
}
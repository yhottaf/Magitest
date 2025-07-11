using fantec.Common;
using UnityEngine;

namespace fantec
{

    // コマンド：ルール画像付きのフェードイン
    internal class AdvCommandRuleFadeIn : AdvCommandRuleFadeBase
    {
        public AdvCommandRuleFadeIn(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row, dataManager)
        {
        }

        //フェード開始
        protected override void OnStartFade(GameObject target, AdvEngine engine, AdvScenarioThread thread)
        {
            Fade.RuleFadeIn(engine, TransitionArgs, () => OnComplete(thread));
        }
    }
}

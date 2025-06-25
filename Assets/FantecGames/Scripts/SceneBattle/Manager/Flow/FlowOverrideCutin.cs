using Cysharp.Threading.Tasks;
using fantec.Battle.Model;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public partial class BattleFlowManager
    {
        public class FlowOverrideCutin : FlowBase
        {
            public override void OnEnter(BattleFlowManager manager, FlowBase prevFlow)
            {
                var modelOverride = Locator.Resolve<IBattleModelOverrideSkill>();
                var resourceMng = Locator.Resolve<IBattleResourceManager>();

                var masterManager = Locator.Resolve<IBattleMasterManager>();

                var battler = modelOverride.GetReserveHeadBattler();

                modelOverride.FookOff();
                modelOverride.Activation();


                void PlayCutin()
                {
                    // ボイスが存在していれば再生

                    // アニメーション再生
                    var cutinAnim = Locator.Resolve<IBattleAnimationManager>().Play<IOverrideSkillCutinAnimation>();

                    cutinAnim.SetCharaSprite(resourceMng.GetCutinSprite(battler))
                        .SetBackSprite(battler.OverrideSkill.Entity.overrideType)
                        .SetSkillName(battler.OverrideSkill.Entity.skillName).OnEnd
                        .Subscribe(_ => manager.ChangeFlow<FlowCombat>())
                        .AddTo(cutinAnim.ClosedDisposables);
                }

                // カットインの再生
                PlayCutin();
            }
        }
    }
}
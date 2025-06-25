using Cysharp.Threading.Tasks;
using fantec.Battle.Model;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks.Linq;

namespace fantec.Battle.Manager
{
    public partial class BattleFlowManager
    {
        public class FlowBoot : FlowBase
        {
            public override void OnEnter(BattleFlowManager manager, FlowBase prevFlow)
            {
                var modelAdvent = Locator.Resolve<IBattleModelAdventSkill>();
                var modelAffect = Locator.Resolve<IBattleModelStage>();
                var modelStage=Locator.Resolve<IBattleModelStage>();
                var modelTime = Locator.Resolve<IBattleModelTime>();
            　　var modelUnits=Locator.Resolve<IBattleModelUnits>();


                //-----------------------------------------------------------------------------------------//
                //
                // モデル間の紐づけ
                //
                //-----------------------------------------------------------------------------------------//

                //---------------------------------------------------------
                // オーバーライドスキル
                //---------------------------------------------------------

                // キャラが死んだらアドベントスキルの監視対象から外す
                modelUnits.OnDeadBattlerObservable
                    .Subscribe(battler => modelAdvent.RemoveBattler(battler))
                    .AddTo(manager.m_OnDestroyDisposables);

                // キャラが蘇ったらアドベントスキルの監視対象に追加
                modelUnits.OnReviveBattlerObservable
                    .Where(battler => !modelAdvent.GetIsContains(battler))
                    .Subscribe(battler => modelAdvent.RegistBattler(battler))
                    .AddTo(manager.m_OnDestroyDisposables);

                // 味方メンバーが更新されたらアドベントスキルの監視対象を置き換える
                modelUnits.OnUpdatePlayerMemberObservable
                    .Subscribe(battlerList => modelAdvent.ReplacePlayerBattlers(battlerList))
                    .AddTo(manager.m_OnDestroyDisposables);

                // 敵メンバーが更新されたらアドベントスキルの監視対象を置き換える
                modelUnits.OnUpdateEnemyMemberObservable
                    .Subscribe(battlerList => modelAdvent.ReplaceEnemyBattlers(battlerList))
                    .AddTo(manager.m_OnDestroyDisposables);

                // --------------------------------------------------
                // オーバーライドスキル
                // --------------------------------------------------



                // ---------------------------------------------------------------------------------------------------- //
                //
                // Hud のイベント登録 TODO:HUDに登録がいるものはここで記載
                //
                // ---------------------------------------------------------------------------------------------------- //

                new Ui.HudHeaderPresenter(manager.m_OnDestroyDisposables);


                // 乱数初期化
                var timeNumeral = string.Join("", System.DateTime.Now.ToString().Split(' ', '/', ':', 'P', 'A', 'M'));
                var random = int.Parse(timeNumeral.Substring(5));
                Random.InitState(random);

                UniTask.Void(async () =>
                {
                    // マスターデータ読み込み
                    await Locator.Resolve<IBattleMasterManager>().LoadAsync(manager.m_OnDestroyCancellationToken);

                    // TODO:バトル前に抽選され次第削除
                    var bridgingData = Locator.Resolve<IBattleModelBridginData>().Data;
                    if(bridgingData.GetIsActive()==false)
                    {
                        await DummyServerForBattle.GetLotteryAsync(bridgingData.GetStageData().stageId);
                    }

                   // 次のフローへ
                   manager.ChangeFlow<FlowInit>();
                });
            }
        }
    }
}
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public partial class BattleFlowManager
    {
        public class FlowLose : FlowBase
        {
            public override void OnEnter(BattleFlowManager manager, FlowBase prevFlow)
            {
                Locator.Resolve<Ui.IHudInputGuardView>().Show();  // 入力制限
                Locator.Resolve<IBattleSoundManager>().StopBgm(); // BGMを止める
                // TODO: Loseアニメーション処理入れる 2025/04/09
                var anim = Locator.Resolve<IBattleAnimationManager>().Play<ILoseAnimation>(); // アニメーション再生

                anim.OnEnd.Subscribe(_ =>
                {
                    Locator.Resolve<Ui.IHudInputGuardView>().Hide();            // 入力解放
                    Locator.Resolve<IBattleSoundManager>().PlayBgmResultLose(); // BGM再生
                    CreateContinueModal();                                      // コンテニューモーダルの表示
                }).AddTo(anim.ClosedDisposables);

            }

            private void CreateReconfirmationModal()
            {
                var window = Locator.Resolve<IBattleWindowManager>().Open<IContinueDoubleWindow>();
                window.OnClickYes.Subscribe(_ => Locator.Resolve<IBattleFlowManager>().ChangeFlow<FlowExit>()).AddTo(window.ClosedDisposable);
                window.OnClickNo.Subscribe(_=>CreateContinueModal()).AddTo(window.ClosedDisposable);
            }

            private void CreateContinueModal()
            {
                var window = Locator.Resolve<IBattleWindowManager>().Open<IContinueWindow>();
                var stoneInfo = DummyServerForBattle.GetStones();
                window.SetText(stoneInfo.paidStone, stoneInfo.freeStone);
                window.OnClickYes.Subscribe(_=>OnClickYes()).AddTo(window.ClosedDisposable);
                window.OnClickNo.Subscribe(_=>CreateReconfirmationModal()).AddTo(window.ClosedDisposable);
            }

            private void OnClickYes()
            {
                UniTask.Void(async () =>
                {
                    var isSuccess = await DummyServerForBattle.TryConsumeStoneAsync();
                    if(isSuccess)
                    {
                        Locator.Resolve<IBattleFlowManager>().ChangeFlow<FlowRevive>();
                    }
                    else
                    {
                        // TODO:石が足りませんモーダル
                        Locator.Resolve<IBattleFlowManager>().ChangeFlow<FlowRevive>();
                    }
                });
            }
        }
    }
}
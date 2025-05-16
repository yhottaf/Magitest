using fantec.Battle;
using fantec.Battle.Manager;
using fantec.Battle.Model;
using UniRx;
using UnityEngine;

namespace fantec.Debugger
{
    public partial class DebuggerPresenter : MonoBehaviour
    {
        private void LoadItemBattle()
        {
            var modelTime = Locator.Resolve<IBattleModelTime>();
            var modelUnits=Locator.Resolve<IBattleModelUnits>();
            var modelStage=Locator.Resolve<IBattleModelStage>();

            var flowManager = Locator.Resolve<IBattleFlowManager>();
            var windowManager=Locator.Resolve<IBattleWindowManager>();
            var masterManager=Locator.Resolve<IBattleMasterManager>();

            modelStage.OnUpdateStageDataObservable.Subscribe(stage =>
            {
                Create("ウェーブ変更", new DebuggerItemData()
                {
                    inItemDatas=DebuggerInItemData.GetDatas("wave :",value=>
                    {
                        stage.SetWave(value);
                        stage.UpdateMember();
                        flowManager.ChangeFlowReserve<BattleFlowManager.FlowWaveSetup>();
                    },stage.Entity.maxWave)
                });

                Create("ターン数変更", new DebuggerItemData()
                {
                    inItemDatas=DebuggerInItemData.GetDatas("ターン数 : ",value=>
                    {
                        stage.SetTurn(value);
                    },31)
                });

                Create("ステージ", new DebuggerItemData()
                {
                    inItemDatas = new DebuggerInItemData[]
                    {
                        new DebuggerInItemData("開始",()=>
                        {
                            // ブラックアウト画面が存在しているか確かめる
                           if(Locator.Resolve<IBattleAnimationManager>().GetIsExist(out IBlackoutAnimation animation))
                            {
                                // 存在していたら消す
                                animation.Hide();
                            }
                            flowManager.ChangeFlowReserve<BattleFlowManager.FlowRematch>();
                        }),
                        new DebuggerInItemData("終了",()=>
                        {
                            if(Locator.Resolve<IBattleAnimationManager>().GetIsExist(out IBlackoutAnimation animation))
                            {
                                // 存在していたら消す
                                animation.Hide();
                            }
                           flowManager.ChangeFlowReserve<BattleFlowManager.FlowWin>();
                        })
                    }
                });
            });

            Create("ゲーム加速", new DebuggerItemData()
            {
                inItemDatas=DebuggerInItemData.GetDatas("x ",value=>
                {
                    modelTime.SetGameTimeScale(value + 1);
                },10)
            });

            Create("ゲーム減速", new DebuggerItemData()
            {
                inItemDatas=DebuggerInItemData.GetDatas("x 0.",value=>
                {
                    modelTime.SetGameTimeScale((value + 1) * 0.1f);
                },9)
            });

            Create("情報", new DebuggerItemData
            {
                inItemDatas=new DebuggerInItemData[]
                {
                    new DebuggerInItemData("キャラクター",()=>
                    {
                        if(windowManager.GetIs(out IDebugStateWindow window))
                            windowManager.Close<IDebugStateWindow>();
                        else
                            windowManager.Open<IDebugStateWindow>();
                    })
                }
            });
        }
    }
}
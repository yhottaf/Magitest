using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

using fantec.Common;
using fantec.Battle.Model;

namespace fantec.Battle.Manager
{
    public partial class BattleFlowManager
    {
        public class FlowWin : FlowBase
        {
            public override void OnEnter(BattleFlowManager manager, FlowBase prevFlow)
            {
                Locator.Resolve<Ui.IHudInputGuardView>().Show();         // 入力制限
                Locator.Resolve<IBattleSoundManager>().StopBgm();        // BGMを止める
                // 勝利演出の再生
                var anim = Locator.Resolve<IBattleAnimationManager>().Play<IWinAnimation>();
                
                var cts = manager.m_OnDestroyCancellationToken;

                UniTask.Void(async () =>
                {
                    // 通信処理
                    Connecting.Open();
                    var (_, _, dropItem, expInfo) = await UniTask.WhenAll(
                        anim.OnEnd.ToUniTask(useFirstValue: true, cancellationToken: cts),
                        DummyServerForBattle.SaveStageclearFlagAsync(),
                        DummyServerForBattle.GetDropItemAsync(),
                        DummyServerForBattle.UserRankUpAsync());
                    Connecting.Close();

                    Locator.Resolve<IBattleSoundManager>().PlayBgmResultWin();   // BGM再生
                    Locator.Resolve<Ui.IHudInputGuardView>().Hide();             // 入力解放
                    var rewardInfoList = RewerdInfo.CreateInfos(dropItem);       // 取得可能なデータに変換
                    this.OpenResult(rewardInfoList,expInfo.oldExp,expInfo.gainExp); // リザルトを開く
                });
            }

            private void OpenResult(IEnumerable<RewerdInfo>rewardInfoList,int oldExp,int gainExp)
            {
                var resultWindow = Locator.Resolve<IBattleWindowManager>().Open<IWinResultWindow>();
                var modelStage = Locator.Resolve<IBattleModelStage>();
                var master = Locator.Resolve<IBattleMasterManager>().UserRankMaster; // ユーザーランクマスターの取得

                var newExp = oldExp + gainExp;                               // 経験値の合計
                var oldRank = master.GetLevelByExp(oldExp);                  // 元のレベル
                var newRank=master.GetLevelByExp(newExp);                    // レベルアップ後のレベル
                var diffLevel = newRank - oldRank;                           // レベルアップ前と後の差(上がったレベル数)
                var start = master.GetTableExpNormalized(oldExp);            // 経験値ゲージの開始位置
                var end=diffLevel+master.GetTableExpNormalized(newExp);      // 経験値ゲージの終了位置(周回数も加算して含める)

                var tableExpList = new List<int>();  // 上がったレベルごとのテーブル経験値格納
                if(diffLevel==0)                     // レベルが上がっていなければ
                {
                    tableExpList.Add(master.GetTableExpByLevel(oldRank + 1));
                }
                else                                         // レベルが上がっていれば
                {
                    for(int i=oldRank+1;i<=newRank;i++)
                    {
                        tableExpList.Add(master.GetTableExpByLevel(i));
                    }
                }

                var consumeStamina = DummyServerForBattle.GetCurrentStamina();

                resultWindow.SetStaminaText(modelStage.Entity.stamina, consumeStamina); // スタミナ表示
                resultWindow.SetStageName(modelStage.Entity.stageName);                 // ステージ名
                resultWindow.SetInteractableNextButton(false);                          // 次へボタンを無効化
                resultWindow.SetInteractableRetryButton(false);                         // 再挑戦ボタンを無効化
                resultWindow.SetCurrentRank(oldRank);                                   // 現在のランク
                resultWindow.SetNextRank(oldRank + 1);                                  // 次のランク
                resultWindow.SetGainExp(gainExp);                                       // 現在の経験値
                resultWindow.SetTableExp(master.GetTableExpByLevel(oldRank + 1));       // 次の経験値

                resultWindow.OnRematch
                    .Subscribe(_ => OnClickRematch())   // 再挑戦
                    .AddTo(resultWindow.ClosedDisposable);
                resultWindow.OnNext
                    .Subscribe(_=>Locator.Resolve<IBattleFlowManager>().ChangeFlow<FlowExit>()) // 次へ
                    .AddTo(resultWindow.ClosedDisposable);

                resultWindow.PlayExpAnimation(tableExpList.ToArray(), start, end, rank =>  // レベルが上がったら
                {
                    var newRank = oldRank + rank;
                    var nextRank = newRank + 1;
                    var prevRank = newRank - 1;
                    var oldStamina = master.GetDataByLevel(prevRank).stamina;
                    var newStamina = master.GetDataByLevel(newRank).stamina;

                    resultWindow.PauseExpAnimation();                                     // ゲージ増加を一時的に止める
                    resultWindow.SetCurrentRank(newRank);                                 // レベル表示を更新
                    resultWindow.SetNextRank(nextRank);                          
                    resultWindow.SetTableExp(master.GetTableExpByLevel(newRank));         // 次の経験値テーブル

                    Locator.Resolve<IBattleWindowManager>().Open<IRankUpPopupWindow>()    // ランクアップウィンドウを表示
                    .SetRankValue(prevRank, newRank)                                      // 表示ランク設定
                    .SetStaminaValue(oldStamina, newStamina)                              // 表示スタミナ設定
                    .OnEnd.Subscribe(_ => resultWindow.ResumeExpAnimation());             // 閉じられたらゲージ増加を再開
                },
                () =>
                {
                    if (rewardInfoList.Count() > 0)                                       // ドロップアイテムがあれば
                    {
                        resultWindow.PlayDropItem(rewardInfoList, () =>                   // ドロップアイテム表示が完了したら
                        {
                            resultWindow.SetInteractableNextButton(true);                 // 次へボタンを有効化
                            resultWindow.SetInteractableRetryButton(true);                // 再挑戦ボタンを有効化
                        });
                    }
                    else                                                                  // ドロップアイテムがなければ
                    {
                        resultWindow.SetInteractableNextButton(true);                     // 次へボタンを有効化
                        resultWindow.SetInteractableRetryButton(true);                    // 再挑戦ボタンを有効化
                    }
                });

                resultWindow.OnRematch.Merge(resultWindow.OnNext)                         // どちらかのボタンを押したら
                    .Subscribe(_ =>
                    {
                        resultWindow.SetInteractableNextButton(false);                    // 次へボタンを無効化
                        resultWindow.SetInteractableRetryButton(false);                   // 再挑戦ボタンを無効化
                    })
                    .AddTo(resultWindow.ClosedDisposable);
            }

            private void OnClickRematch()
            {
                UniTask.Void(async () =>
                {
                    var isSuccess = await DummyServerForBattle.TryConsumeStaminaAsync();
                    if(isSuccess)
                    {
                        Locator.Resolve<IBattleFlowManager>().ChangeFlow<FlowRematch>();
                    }
                    else
                    {
                        // TODO スタミナが足りません　モーダルの表示を行う
                        Locator.Resolve<IBattleFlowManager>().ChangeFlow<FlowRematch>();
                    }
                });
            }
        }
    }
}
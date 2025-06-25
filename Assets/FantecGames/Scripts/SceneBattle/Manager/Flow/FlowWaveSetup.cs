using Cysharp.Threading.Tasks;
using DG.Tweening;
using fantec.Battle.Model;
using fantec.Common;
using fantec.Utilities;
using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public partial class BattleFlowManager
    {
        public class FlowWaveSetup : FlowBase
        {
            // TODO : 現在ここまでのウェーブ遷移が行われている。
            public override void OnEnter(BattleFlowManager manager, FlowBase prevFlow)
            {
                UniTask.Void(async () =>
                {
                    var modelUnits = Locator.Resolve<IBattleModelUnits>();
                    var modelStage = Locator.Resolve<IBattleModelStage>();
                    var modelAdvent = Locator.Resolve<IBattleModelAdventSkill>();
                    var resourceManager = Locator.Resolve<IBattleResourceManager>();

                    var cts = manager.m_OnDestroyCancellationToken;

                    // リロード
                    modelUnits.Reload();

                    // ステージ更新
                    modelStage.UpdateMember();

                    var EnemyDatas = modelStage.CurrentWaveTeamData;


                    foreach (var unitData in EnemyDatas.UnitList)
                    {
                        var cardData = Locator.Resolve<IBattleMasterManager>().EnemyCardMaster.GetData(
                            unitData.cardId);
                        string spinePath = AssetPath.GetCharacterSpinePath(cardData.originId);
                        string CutinSpritePath = AssetPath.GetCharacterSpriteCutinPath(cardData.originId);
                        await resourceManager.CasheSkeletonAsync(spinePath, cts);
                        await resourceManager.CasheSpriteAsync(CutinSpritePath, cts);
                    }

                    //敵チーム設定
                    modelUnits.SetEnemyTeam(modelStage.CurrentWaveTeamData);
                    // 攻撃順序を更新
                    modelAdvent.SortBySpeed();


                    //判定後か入場後であれば
                    if(prevFlow.GetType()==typeof(FlowJudge)||
                        prevFlow.GetType()==typeof(FlowAdmission))
                    {
                        // BGMを再生
                        Locator.Resolve<IBattleSoundManager>().PlayBgm(modelStage.GetBattleBgmName());
                    }

                    // ボスウェーブであれば
                    if (modelStage.IsBossWave)
                    {

                    }
                    else
                    {
                        // 通常のウェーブ数表示アニメーション再生
                        OnChangeFlow(manager);
                    }
                });
            }

            private void OnChangeFlow(BattleFlowManager manager)
            {
                var sequence = DOTween.Sequence();

                sequence.AppendInterval(2.5f);
                // 通常戦闘へ
                sequence.AppendCallback(() =>
                {
                    manager.ChangeFlow<FlowCombat>();
                });
            }
        }
    }
}
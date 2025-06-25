using Cysharp.Threading.Tasks;
using fantec.Battle.Field.Chara;
using fantec.Battle.Model;
using fantec.Common;
using fantec.Utilities;
using System.Linq;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public partial class BattleFlowManager
    {
        public class FlowInit : FlowBase
        {
            public override void OnEnter(BattleFlowManager manager, FlowBase prevFlow)
            {
                UniTask.Void(async () =>
                {
                    var bridgingData = Locator.Resolve<IBattleModelBridginData>().Data;
                    var modelStage = Locator.Resolve<IBattleModelStage>();
                    var modelUnits=Locator.Resolve<IBattleModelUnits>();
                    var modelOverride=Locator.Resolve<IBattleModelOverrideSkill>(); 

                    var resourceManager = Locator.Resolve<IBattleResourceManager>();
                    var masterManager = Locator.Resolve<IBattleMasterManager>();
                    var soundManager=Locator.Resolve<IBattleSoundManager>();
                    //var windowManager = Locator.Resolve<IBattleWindowManager>();
                    // TODO : アニメーションマネージャーもここに記載

                    var cts = manager.m_OnDestroyCancellationToken;

                    // チーム情報初期化
                    modelUnits.ResetPlayerTeam();
                    modelUnits.ResetEnemyTeam();

                    var PlayerDatas = bridgingData.GetCleanTeamData();

                    foreach (var unitData in PlayerDatas.UnitList)
                    {
                    var cardData = Locator.Resolve<IBattleMasterManager>().PlayerCardMaster.GetData(
                        unitData.cardId);
                        string spinePath = AssetPath.GetCharacterSpinePath(cardData.originId);
                        string CutinSpritePath = AssetPath.GetCharacterSpriteCutinPath(cardData.originId);
                        await resourceManager.CasheSkeletonAsync(spinePath, cts);
                        await resourceManager.CasheSpriteAsync(CutinSpritePath, cts);
                    }


                    // リセット
                    modelStage.Reset();
                    modelOverride.Reset();

  
                    // データの読み込み
                    modelStage.SetStageEntity(bridgingData.GetStageData().ToEntity());
                    modelUnits.SetPlayerTeam(bridgingData.GetCleanTeamData());


                    // リソース読み込み
                    await resourceManager.CasheSpriteAsync(AssetPath.SpriteFieldPath + modelStage.Entity.FieldImg, cts);
                    await resourceManager.CasheSpriteAsync(AssetPath.SpriteBackGroundPath+modelStage.Entity.normalBg,cts);
                    await resourceManager.CasheAudioAsync(AssetPath.BGMFolderPath+modelStage.Entity.normalBgm,cts);

                    // ボスBGMと画像フィールドの読み込み
                    //await resourceManager.CasheSpriteAsync(AssetPath.SpriteBackGroundPath+modelStage.Entity.bossBg,cts);
                    //await resourceManager.CasheAudioAsync(AssetPath.BGMFolderPath+modelStage.Entity.bossBgm,cts);
                    
                    // ボイスの読み込み処理やSpineデータの読み込み
                    foreach (var battler in modelUnits.PlayerDatas.GetExistBattlers())
                    {
                        //await resourceManager.LoadVoiceAsync(masterManager.CharavoiceMaster.GetData(battler.Unit.Entity.originId)
                        //    .Select(x => x.fileName), cts);
                    }

                    // リソースから読み込んできたデータをステージにセット
                    Locator.Resolve<IBattleSoundManager>().PlayBgm(modelStage.GetBattleBgmName()); // BGMを再生
                    Locator.Resolve<Field.IFieldBackgroundView>().SetSprite(resourceManager.GetBg(modelStage.GetBattleBgName())); // 背景更新
                    Locator.Resolve<Field.IFieldView>().SetSprite(resourceManager.GetFieldImg(modelStage.GetFieldImgName())); // ステージの画像取得
                    Locator.Resolve<Ui.IHudInputGuardView>().Hide(); // 入力解放
                    
                    // フェードイン
                    Loading.Hide(0.5f);

                    // 次のフロー処理へ
                    manager.ChangeFlow<FlowAdmission>();
                });
            }
        }
    }
}
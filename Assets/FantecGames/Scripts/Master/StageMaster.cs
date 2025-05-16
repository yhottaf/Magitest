using fantec;
using fantec.Common;
using fantec.Master;
using fantec.PlayFabClient;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fantec.Master
{
    [System.Serializable]
    public class StageData : IData
    {
        public int stageId;        // ステージID
        public int sortId;         // ソートID
        public int groupId;        // グループID
        public int chapterNumber;  // 章番号
        public int stageNumber;    // ステージ番号
        public string stageName;   // ステージ名
        public int[] conditionIds; // 解放に必要なステージID
        public string description; // ステージの説明
        public int stamina;        // 消費スタミナ
        public StagePlayType playType; // プレイする種別 (バトル or ストーリー)
        public StageCategoryType categoryType; // ステージ種別
        public StageDifficultyType difficultyType;   // 難易度
        public CardRarityType recommendedRarityType; // 推奨レアリティ
        public int recommendedLevel;                 // 推奨レベル

        public string normalBgm;  // 通常BGM
        public string bossBgm;    // ボス戦時のBGM
        public string normalBg;   // 通常背景
        public string bossBg;     // ボス背景
        public string fieldImg;   // フィールドの画像

        public string waveMasterKey;       // ウェーブマスター名
        public int rewardExp;              // 報酬経験値
        public int clearReword;            // 初回報酬テーブルID
        public int rewardTableId;          // 報酬テーブルID

        public string advSeetName;         // 再生する会話パート名
    }
}

[ExcelAsset(AssetPath =AssetPath.MasterLocalDataFolderPath), CreateAssetMenu(fileName = "StageMaster", menuName = "ScriptableObjects/StageMaster")]
public class StageMaster:MasterBase<StageData>
{
    public StageData GetData(int stageId)
    {
        try { return dataList.First(x => x.stageId == stageId); }
        catch { throw new InvalidOperationException($"[stageId : {stageId}] は存在しません。"); }
    }

    public List<StageData>GetStageDataList(StageCategoryType type)
    {
        return dataList.Where(x => x.categoryType == type).ToList();
    }

    public List<StageData>GetStageDataList(int groupId)
    {
        return dataList.Where(x=>x.groupId== groupId).ToList();
    }

    public bool IsExist(int stageId)
    {
        return dataList.Any(x => x.stageId == stageId);
    }

    /// <summary>
    /// 引数の GroupId のクエストを全てクリアしているかを返します
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns></returns>
    public bool IsClearByGroupId(int[]groupId)
    {
        var stageDatas=dataList.Where(x=>groupId.Contains(x.groupId));
        foreach (var stageData in stageDatas)
        {
            if(UserDataManager.IsQuestClear(stageData.stageId)==false)
            {
                return false;
            }
        }
        return true;
    }
}
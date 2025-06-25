using System;
using fantec.Common;
using System.Linq;
using System.Collections.Generic;

// カードのデータ管理
namespace fantec.Master
{
    public abstract class AbstructCardData:IData
    {
        public bool active;               // 有効か否か
        public int cardId;                // キャラID
        public int originId;              // オリジンID
        public int sortId;                // ソートID
        public string charaName;          // キャラ名
        public string readCharaname;      // 読み仮名
        public string[] tagNames;         // タグ名

        public CardRarityType rarityType; // 初期レアリティ
        public AffectMoveType moveType;   // 移動種別
         public AffectHitType hitType;     // ヒット種別(当たった時の音を変更したいのであれば)
        public AffectAttributeType attributeType; // 属性

        public int OverrideId;            // オーバーライド
        public int ExsaOverrideId;        // エクサオーバーライド
        public int SectaOverrideId;       // ゼタオーバーライド
        public int QuetaOverrideId;       // クエタオーバーライド

        public int maxLevel;              // 最大到達レベル
        public int minHP;                 // 最小体力値
        public int maxHP;                 // 最大体力値
        public int minATK;                // 最小攻撃力
        public int maxATK;                // 最大攻撃力
        public int minSPD;                // 最小素早さ
        public int maxSPD;                // 最大素早さ
        public int Move;                  // 移動力

        public int damageRange;           // ダメージの振れ幅

        public string growthTableKey;    // 成長曲線キー
        public List<int> behaviorAI;     // 移動するマス目順をint型で格納するリスト

        public int GetHpByLevel(int level)
        {
            var data = MasterDataManager.Instance.GetMaster<GrowthMaster>(growthTableKey);
            var value = minHP + (maxHP - minHP) * data.GetData(level).HP / 100;
            return (int)value;
        }

        public int GetAtkByLevel(int level)
        {
            var data=MasterDataManager.Instance.GetMaster<GrowthMaster>(growthTableKey);
            var value = minATK + (maxATK - minATK) * data.GetData(level).ATK / 100;
            return (int)value;
        }

        public int GetSpdByLevel(int level)
        {
            var data = MasterDataManager.Instance.GetMaster<GrowthMaster>(growthTableKey);
            var value = minSPD + (maxSPD - minSPD) * data.GetData(level).SPD / 100;
            return (int)value;
        }
    }

    public abstract class AbsturctCardMaster<T>:MasterBase<T>where T:AbstructCardData
    {
        public bool IsExist(int cardId)
        {
            return dataList.Any(x=>x.cardId == cardId);
        }

        public T GetData(int cardId)
        {
            try { return dataList.First(x => x.cardId == cardId); }
            catch { throw new InvalidOperationException($"[cardId : {cardId}] は存在しません。"); }
        }

        public T GetDataOrefault(int cardId)
        {
            return dataList.FirstOrDefault(x => x.cardId == cardId);
        }
    }
}
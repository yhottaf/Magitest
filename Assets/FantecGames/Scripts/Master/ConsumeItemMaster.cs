using fantec.Common;
using System;
using System.Linq;
using UnityEngine;

namespace fantec.Master
{
    [Serializable]
    public class ConsumeItemData : IData
    {
        public int itemId;                       // アイテムID
        public int sortNo;                       // ソート順
        public string name;                      // アイテム名
        public string categoryName;              // カテゴリ名
        public int categoryId;                   // カテゴリID
        public string description;               // 説明
        public ConsumeItemEffectType effectType; // 効果の種類
        public int effectValue;                  // 効果の値
        public int effectTime;                   // 効果時間
        public int maxItem;                      // 最大所持数
        public string imageName;                 // イメージデータ名
    }

    [ExcelAsset(AssetPath = AssetPath.MasterLocalDataFolderPath), CreateAssetMenu(fileName = "ConsumeItemMaster", menuName = "ScriptableObjects/ConsumeItemMaster")]
    public class ConsumeItemMaster : MasterBase<ConsumeItemData>
    {
        public ConsumeItemData GetData(int itemId)
        {
            try
            {
                return dataList.First(x => x.itemId == itemId);
            }
            catch
            {
                throw new InvalidOperationException($"[ItemId : {itemId}] は存在しません。");
            }
        }
    }
}
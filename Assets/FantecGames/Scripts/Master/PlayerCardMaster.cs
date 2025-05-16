using UnityEngine;
using fantec.Common;

namespace fantec.Master
{
    [System.Serializable]
    public class PlayerCardData:AbstructCardData
    {
        [Header("プレイヤー固有情報")]
        public string[] homeVoice; // ホーム用ボイス
        public string expTableKey; // カードの経験値テーブル
        public string getStartSchedule; // 入手開始スケジュール
        public string getEndSchedule;   // 入手終了スケジュール
        public Vector3 Live2DlocalPosition; // Live2Dのデフォルトポジション
        public Vector3 Live2DlocalScale; // Live2Dのデフォルトスケール
        public string[] AnimationClip; //Live2Dで使用するモーション

        public int GetLevelByExp(int totalExp)
        {
            var data = MasterDataManager.Instance.GetMaster<ExpMaster>(expTableKey);
            var level = data.GetLevelByExp(totalExp);
            return level;
        }

        public int GetExpByLevel(int level)
        {
            var data = MasterDataManager.Instance.GetMaster<ExpMaster>(expTableKey);

            if(level>EnumExtentions.GetMaxLevel(CardRarityType.R6))
            {
                level=EnumExtentions.GetMaxLevel(CardRarityType.R6);
            }

            var exp=data.GetExpByLevel(level);
            return exp;
        }

        public int GetNextLevelExp(int exp)
        {
            var data = MasterDataManager.Instance.GetMaster<ExpMaster>(expTableKey);
            var level=data.GetLevelByExp(exp);
            return level;
        }

        // 進化データ
        //限凸データを書くならここに記載


        //------------------------------------------------------------
        // 経験値に応じたパラメータ取得
        //------------------------------------------------------------

        public int GetHpByExp(int totalExp,CardRarityType rarityType=CardRarityType.R1,int overLimitCount=0)
        {
            return GetHpByLevel(GetLevelByExp(totalExp), rarityType, overLimitCount);
        }

        public int GetAtkByExp(int totalExp,CardRarityType rarityType=CardRarityType.R1,int overLimitCount=0)
        {
            return GetAtkByLevel(GetLevelByExp(totalExp),rarityType, overLimitCount);
        }

        public int GetSpdByExp(int totalExp,CardRarityType rarityType=CardRarityType.R1,int overLimitCount=0)
        {
            return GetSpdByLevel(GetLevelByExp(totalExp), rarityType, overLimitCount);
        }

        public int GetSpecByExp(int totalExp,CardRarityType rarityType=CardRarityType.R1 ,int overLimitCount=0)
        {
            return GetSpecByLevel(GetLevelByExp(totalExp), rarityType, overLimitCount);
        }

        //------------------------------------------------------------
        // レベルに応じたパラメータ取得
        //------------------------------------------------------------
        public int GetHpByLevel(int level,CardRarityType rarityType=CardRarityType.R1,int overLimitCount=0)
        {
            var bonus = 0;
            //if(rarityType!=CardRarityType.R1)bonus+=GetEvo
            //if (overLimitCount != 0) bonus += GetOverLimitData(overLimitCount).HP;
            return base.GetHpByLevel(level) + bonus;
        }

        public int GetAtkByLevel(int level,CardRarityType rarityType=CardRarityType.R1,int overLimitCount=0)
        {
            var bonus = 0;
            return base.GetAtkByLevel(level) + bonus;
        }

        public int GetSpdByLevel(int level,CardRarityType rarityType=CardRarityType.R1,int overLimitCount=0)
        {
            var bonus = 0;
            return base.GetSpdByLevel(level) + bonus;
        }

        public int GetSpecByLevel(int level, CardRarityType rarityType = CardRarityType.R1, int overLimitCount = 0)
        {
            return GetHpByLevel(level, rarityType, overLimitCount)
                + GetAtkByLevel(level, rarityType, overLimitCount)
                + GetSpdByLevel(level, rarityType, overLimitCount);
        }

        // キャラに属性の耐性などがあり、凸数によって上げたい場合はここに記載する
    }
    [ExcelAsset(AssetPath=AssetPath.MasterLocalDataFolderPath), CreateAssetMenu(fileName = "PlayerCardMaster", menuName = "ScriptableObjects/PlayerCardMaster")]
    public class PlayerCardMaster:AbsturctCardMaster<PlayerCardData>
    {

    }
}
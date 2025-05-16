using fantec.Common;
using System;
using UnityEngine;

namespace fantec.Master
{
    [System.Serializable]
    public class UserRankData: IData
    {
        public int level;
        public int exp;
        public int stamina;
    }
    
    [ExcelAsset(AssetPath =AssetPath.MasterLocalDataFolderPath),CreateAssetMenu(fileName = "UserRankMaster", menuName = "ScriptableObjects/UserRankMaster")]
    public class UserRankMaster:MasterBase<UserRankData>
    {
        /// <summary>
        /// 最大レベルを取得
        /// </summary>
        /// <returns></returns>
        public int GetMaxLevel()
        {
            return dataList.Count;
        }

        /// <summary>
        /// 引数の経験値に応じたレベルを取得する
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int GetLevelByExp(int exp)
        {
            var level = 0;

            if (exp >= dataList[dataList.Count - 1].exp)
            {
                level = dataList.Count;
            }
            else
            {
                for (int i = 0; i < GetMaxLevel(); i++)
                {
                    if (exp < dataList[i].exp)
                    {
                        level = i;
                        break;
                    }
                }
            }

            return level;
        }

        /// <summary>
        /// 引数の経験値をもとにデータを取得
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public UserRankData GetDataByExp(int exp)
        {
            return GetDataByLevel(GetLevelByExp(exp));
        }

        /// <summary>
        /// 引数のレベルをもとにデータを取得
        /// </summary>
        public UserRankData GetDataByLevel(int level)
        {
            try { return dataList[level - 1]; }
            catch { throw new IndexOutOfRangeException($"[index:{level - 1}(level:{level})] はデータの範囲外"); }
        }

        /// <summary>
        ///  引数のレベルまでに必要となる累計経験値を取得
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int GetExpByLevel(int level)
        {
            return GetDataByLevel(level).exp;
        }

        /// <summary>
        /// 引数のレベルテーブル帯の経験値を取得
        /// </summary>
        public int GetTableExpByLevel(int level)
        {
            if (level == GetMaxLevel() || level == 1) return 0;
            return GetExpByLevel(level) - GetExpByLevel(level - 1);
        }

        /// <summary>
        /// 引数の経験値をもとにテーブル帯での経験値を取得する
        /// </summary>
        public int GetTableExpByExp(int exp)
        {
            return exp-GetExpByLevel(GetLevelByExp(exp));
        }

        /// <summary>
        /// 引数の経験値をもとにテーブル帯での経験値を正規化した値を取得する
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public float GetTableExpNormalized(int exp)
        {
            var level=GetLevelByExp(exp);
            if (level == 1 || level == GetMaxLevel()) return 0;
            return (float)GetTableExpByExp(exp) / GetTableExpByLevel(level + 1);
        }
    }
}
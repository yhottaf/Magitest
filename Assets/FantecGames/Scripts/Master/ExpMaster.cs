using UnityEngine;
using fantec.Common;

namespace fantec.Master
{
    [System.Serializable]
    public class ExpData : IData
    {
        public int level;
        public int totalExp;
    }
    [ExcelAsset(AssetPath=AssetPath.MasterLocalDataExpFolderPath), CreateAssetMenu(fileName = "ExpMaster", menuName = "ScriptableObjects/ExpMaster")]

    public class ExpMaster :MasterBase<ExpData>
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
        /// 引数の経験値に応じたレベルを取得
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int GetLevelByExp(int exp)
        {
            var level = 0;
            if (exp >= dataList[dataList.Count-1].totalExp)
            {
                level = dataList.Count;
            }
            else
            {
                for(int i=0;i<GetMaxLevel();i++)
                {
                    if (exp < dataList[i].totalExp)
                    {
                        level = i;
                        break;
                    }
                }
            }

            return level;
        }

        /// <summary>
        /// 引数のレベルまでに必要となる累計経験値を取得
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int GetExpByLevel(int level)
        {
            return dataList[level - 1].totalExp;
        }

        /// <summary>
        /// 現在の経験値をもとに次のレベルまでに必要な経験値を取得
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int GetNextLevelExp(int exp)
        {
            var level = GetLevelByExp(exp);
            if (level == GetMaxLevel()) return 0;
            return dataList[level].totalExp - exp;
        }

        /// <summary>
        /// 現在のレベルテーブルで取得している経験値を取得
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int GetCurrentTableExp(int exp)
        {
            var level=GetLevelByExp(exp);
            if(level==GetMaxLevel()) return 0;
            return exp - dataList[level-1].totalExp;
        }
    }
}
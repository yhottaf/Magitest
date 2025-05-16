using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace fantec.Master
{
    [System.Serializable ]
    public abstract class AbstructSkillData: IData
    {
        public int skillId;        // スキルID
        public int originId;       // オリジンID
        public string skillName;   // スキル名
        public string detail;      // 詳細
        public int maxLevel;       // 最大レベル

        public string[] commandsSingleP = new string[0];   // 味方単体
        public string[] commandsAllP=new string[0];        // 味方全体
        public string[] commandsSingleE=new string[0];     // 敵単体
        public string[] commandsAllE=new string[0];        // 敵全体
        public string[] commandsMyself=new string[0];      // 自分自身
        public string[] commandsBesidesMe =new string[0];  // 自分以外
        public string[] commandsGimmic=new string[0];      // ギミック対象
        public string[] commandsCustom=new string[0];      // カスタム

        public virtual List<SkillCommand>ConvertToSkillCommandList()
        {
            var commandList = new List<SkillCommand>();
            commandList.AddRange(SkillCommandParser.ConvertToSkillCommand(commandsSingleP).SetRangeType(AffectRangeType.MySideSingle));
            commandList.AddRange(SkillCommandParser.ConvertToSkillCommand(commandsAllP).SetRangeType(AffectRangeType.MySideAll));
            commandList.AddRange(SkillCommandParser.ConvertToSkillCommand(commandsSingleE).SetRangeType(AffectRangeType.EnemySingle));
            commandList.AddRange(SkillCommandParser.ConvertToSkillCommand(commandsAllE).SetRangeType(AffectRangeType.EnemyAll));
            commandList.AddRange(SkillCommandParser.ConvertToSkillCommand(commandsMyself).SetRangeType(AffectRangeType.Myself));
            commandList.AddRange(SkillCommandParser.ConvertToSkillCommand(commandsBesidesMe).SetRangeType(AffectRangeType.BesidesMe));
            commandList.AddRange(SkillCommandParser.ConvertToSkillCommand(commandsGimmic).SetRangeType(AffectRangeType.Gimmick));
            commandList.AddRange(SkillCommandParser.ConvertToSkillCommand(commandsCustom));  // カスタムはコマンドに効果範囲が書き込まれるため SetRangeType を行わない
           
            // 付与効果を追加
            commandList.AddRange(SkillCommandParser.ConvertAddtionalCommand(commandList));

            return commandList;
        }

    }
    public abstract class AbstructSkillMaster<T>:MasterBase<T> where T : AbstructSkillData
    {
        public T GetData(int skillId)
        {
            if (skillId == -1) return null; 
            try { return dataList.First(x => x.skillId == skillId); }
            catch { throw new InvalidOperationException($"[skillId : {skillId}] は存在しません。"); }
        }

        public T GetData(int skillId,int level)
        {
            if (skillId == -1) return null;
            var targetId = int.Parse($"{skillId}{level}");
            var data = dataList.FirstOrDefault(x => x.skillId == targetId);
            if(data==null)
            {
             //   Debug.LogWarning($"[originId : {skillId} / level : {level} / targetId : {targetId}] は存在しません。 \n代わりに skillIdで検索します。");
                data=dataList.FirstOrDefault(x=>x.skillId== skillId);
            }
            if(data==null)
            {
                Debug.Log($"[skillId : {skillId}] は存在しません。 \n代わりにリストの一番最初のスキルを取得します。");
                return data = dataList.FirstOrDefault();
            }
            if(data==null)
            {
                Debug.LogWarning($"dataList にスキルが存在しません。");
                return data=dataList.FirstOrDefault();
            }
            if(data==null)
            {
                Debug.LogWarning($"スキルが存在しないため null を返します。");
                return null;
            }
            else
            {
                return data;
            }

        }
    }
}
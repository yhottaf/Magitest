
using fantec.Common;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace fantec
{
    public class SkillCommandParser
    {
        /// <summary>
        ///  スキルコマンドに変換する
        /// </summary>
        public static SkillCommand ConvertToSkillCommand(string commandStr)
        {
            return new SkillCommand
            {
                 categoryType=GetCategoryType(commandStr),
                 attributeType=GetAttributeType(commandStr),
                 rangeType=GetRangeType(commandStr),
                 efficacyType=GetEffecacyType(commandStr),
                 affectValue=ExtractToIntAbs(GetCategoryStr(commandStr)),
                 sustainTurn=ExtractToInt(GetTurnStr(commandStr)),
                 actionCount=ExtractToIntAbs(GetCountStr(commandStr),1), // 最低でも一回は行動するため「１」をデフォに設定
                 randomCount=ExtractToIntAbs(GetRandomStr(commandStr),1),// 最低でも1体は対象とするため「1」をデフォに設定
                 isFirst=ExtractToBool(GetIsFirstStr(commandStr)),
            };
        }

        public static List<SkillCommand>ConvertToSkillCommand(IEnumerable<string>commandStrs)
        {
            var result=new List<SkillCommand>();
            foreach(var commandStr in commandStrs)result.Add(ConvertToSkillCommand(commandStr));
            return result;
        }

        /// <summary>
        /// コマンドに付属した効果を付与、または作成しリストに含める
        /// </summary>
        public static List<SkillCommand>ConvertAddtionalCommand(SkillCommand command)
        {
            var resultCommandList=new List<SkillCommand>();
            var otherSkillMaster = MasterDataManager.Instance.OtherSkillMaster;

            // 状態異常の場合
            if (command.categoryType.GetIsAbnormalCondition())
            {
                if (command.efficacyType != AffectEfficacyType.NONE)
                {
                    // スキルデータの取得
                    var skillData = otherSkillMaster.GetData(command.categoryType, command.efficacyType);

                    // 付与率を割り当て
                    command.SetSuccessRate(skillData.successRate);
                }
                else
                {

                }
            }

            // 付属効果がある場合
            if(command.categoryType.GetIsAttachedEffect())
            {
                // スキルデータとコマンドリストを展開
                var skillData=otherSkillMaster.GetData(command.categoryType,command.efficacyType);
                var additionalCommandList = ConvertToSkillCommand(skillData.addtionalCommands);

                // 親コマンドの効果を引継ぎ
                additionalCommandList
                    .SetParentCategoryType(command.categoryType)
                    .SetRangeType(command.rangeType)
                    .SetTurn(command.sustainTurn)                // 持続ターンのセット
                    .SetIsFirst(command.isFirst);

                // 追加の専用効果を付与
                additionalCommandList.SetConditionValue(skillData.conditionValue);

                // 結果に追加
                resultCommandList.AddRange(additionalCommandList);
            }
            return resultCommandList;
        }

        public static List<SkillCommand>ConvertAddtionalCommand(IEnumerable<SkillCommand>skillCommandList)
        {
            var result=new List<SkillCommand>();
            foreach(var command in skillCommandList)result.AddRange(ConvertAddtionalCommand(command));
            return result;
        }

        /// <summary>
        /// コマンドのカテゴリーを取得する
        /// </summary>
        public static AffectCategoryType GetCategoryType(string commandStr)
        {
            foreach(AffectCategoryType value in Enum.GetValues(typeof(AffectCategoryType)))
            {
                if (commandStr.Contains(value.GetCommandString()))return value;
            }
            Debug.LogError($"[commandStr : {commandStr}] カテゴリが存在しません。");
            return AffectCategoryType.NONE;
        }

        /// <summary>
        /// コマンド文字列から属性種別を取得
        /// </summary>
        public static AffectAttributeType GetAttributeType(string commandStr)
        {
            foreach(AffectAttributeType value in Enum.GetValues (typeof(AffectAttributeType)))
            {
                if (commandStr.Contains(value.ToString())) return value;
            }
            return AffectAttributeType.NONE;
        }

        /// <summary>
        /// コマンド文字列から効果範囲種別を取得
        /// </summary>
        public static AffectRangeType GetRangeType(string commandStr)
        {
            foreach(AffectRangeType value in Enum.GetValues(typeof (AffectRangeType)))
            {
                if (commandStr.Contains(value.GetCommandString())) return value;
            }
            return AffectRangeType.NONE;
        }

        /// <summary>
        /// コマンド文字列から効果種別を取得
        /// </summary>
        public static AffectEfficacyType GetEffecacyType(string commandStr)
        {
            foreach(AffectEfficacyType value in Enum.GetValues(typeof(AffectEfficacyType)))
            {
                if (commandStr.Contains(value.GetCommandString())) return value;
            }
            return AffectEfficacyType.NONE;
        }

        public static string GetCategoryStr(string commandStr)
        {
            return ExtractToStr(commandStr, $@"{GetCategoryType(commandStr).GetCommandString()}[+\-]*([0-9]+)");
        }

        public static string GetTurnStr(string commandStr)
        {
           
            var category = ExtractToStr(commandStr, $"{GetCategoryType(commandStr).GetCommandString()}");  // カテゴリ名を抽出
            if (string.IsNullOrEmpty(category)) return ExtractToStr(commandStr, @"T[+\-]*([0-9]+)");       // A/カテゴリ取り除き版
            else return ExtractToStr(commandStr.Replace(category, ""), @"T[+\-]*([0-9]+)");                // B.通常版
        }

        public static string GetCountStr(string commandStr)
        {
            return ExtractToStr(commandStr, @"C([0-9]+)");
        }

        public static string GetRandomStr(string commandStr)
        {
            return ExtractToStr(commandStr, @"R([0-9]+)");
        }

        public static string GetIsFirstStr(string commandStr)
        {
            return ExtractToStr(commandStr, "速");
        }



        // --------------------------------------------------------------------------------------------------
        // Utils
        // --------------------------------------------------------------------------------------------------

        /// <summary>
        /// 文字列から数値を抽出し返す(符号を考慮しない)
        /// </summary>
        public static int ExtractToIntAbs(string a,int defaultValue=0)
        {
            try { return string.IsNullOrEmpty(a) ? defaultValue : int.Parse(ExtractToStr(a, @"([0-9]+)")); }
            catch { throw new FormatException($"[{a}] は数値にキャストできません。"); }
        }

        /// <summary>
        /// 文字列から数値を抽出し返す (符号を考慮する)
        /// </summary>
        public static int ExtractToInt(string a, int defaultValue = 0)
        {
            try { return string.IsNullOrEmpty(a) ? defaultValue : int.Parse(ExtractToStr(a, @"[+\-]*([0-9]+)")); }
            catch { throw new FormatException($"[{a}] は数値にキャストできません。"); }
        }

        /// <summary>
        /// 文字列から有効可否を抽出する
        /// </summary>
        public static bool ExtractToBool(string a)
        {
            return string.IsNullOrEmpty(a) == false;
        }

        /// <summary>
        /// 文字列から指定の文字列を抽出し返す
        /// </summary>
        /// <param name="a">抽出される文字列</param>
        /// <param name="b">抽出したい文字列</param>
        public static string ExtractToStr(string a,string b)
        {
            var result = "";
            foreach(var m in Regex.Matches(a, b)) { result += m; }
            return result;
        }
    }
}
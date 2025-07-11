using UnityEngine;

namespace fantec
{
    public enum ErrorMsg
    {
        NotFound,
        UnknownType,
        UnknownVersion,
        ColorParseError,
        SpriteMimMap,
        UnknownFontData,
        StringGridRowParseColumnName,
        StringGridRowParseColumnIndex,
        StringGridRowParse,
        StringGridParseHeader,
        StringGridGetColumnIndex,
        SoundNotReadyToPlay,
        TweenWrite,
        FileWrite,
        FileRead,
        MemoryLeak,
        FileReferecedIsNull,
        FileIsNotReady,
        DisableChangeFileLoadFlag,
        DisableChangeFileVersion,
        SingletonError,
        NoChacheTypeFile,
        ExpUnknownParameter,
        ExpResultNotBool,
        ExpIllegal,
        PivotParse,
        TextTagParse,
        TextCallbackCalcExpression,
        TextFailedCalcExpression,
        ExpressionOperateSubstition,
        ExpressionOperator,
    };

    /// <summary>
    /// システムとして使うテキスト
    /// </summary>
    public static class LanguageErrorMsg
    {
        /// <summary>
        /// データ名
        /// </summary>
        const string LanguageDataName = "ErrorMsg";

        /// <summary>
        /// 指定のキーのテキストを、設定された言語に翻訳して取得
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string LocalizeText(ErrorMsg type)
        {
            LanguageManagerBase language = LanguageManagerBase.Instance;
            if(language == null)
            {
                Debug.LogWarning("LanguageManager is NULL");
                return type.ToString();
            }
            else
            {
                string text;
                if(language.TryLocalizeText(type.ToString(),out text))
                {
                    return text;
                }
                else
                {
                    return language.DefaultLanguageText(type.ToString());
                }
            }
        }

        /// <summary>
        /// 指定のキーのテキストフォーマットを、設定された言語に翻訳して取得
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string LocalizeTextFormat(ErrorMsg type,params object[] args)
        {
            string format=LocalizeText(type);
            return string.Format(format, args);
        }
    }
}
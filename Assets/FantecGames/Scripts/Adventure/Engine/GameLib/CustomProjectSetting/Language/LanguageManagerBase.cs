using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace fantec
{
    /// <summary>
    /// 表示言語切り替え用のクラス
    /// </summary>
    public abstract class LanguageManagerBase : ScriptableObject
    {
        public enum LanguageBlankTextType
        {
            SwapDefaultLanguage,   // データがない場合でも基本言語に切り替え
            NoBlankText,           // データがない場合は想定しない。エラーを出す。
            AllowBlankText,        // データがない場合は想定しないが、空テキストを許す。
        }

        static LanguageManagerBase instance;

        /// <summary>
        /// シングルトンなインスタンスの取得
        /// </summary>
        public static LanguageManagerBase Instance
        {
            get
            {
                if (instance == null)
                {
                    if (CustomProjectSetting.Instance)
                    {
                        instance = CustomProjectSetting.Instance.Language;
                    }
                    if (instance != null)
                    {
                        instance.Init();
                    }
                }
                return instance;
            }
        }

        // 言語がオート設定のときは、システム環境に依存する
        const string Auto = "Auto";

        /// <summary>
        /// 設定言語
        /// </summary>
        public string Language
        {
            get { return language; }
        }
        [SerializeField]
        protected string language = Auto;

        // デフォルト言語
        public string DefaultLanguage { get { return defaultLanguage; } }
        [SerializeField]
        protected string defaultLanguage = "Japanese";

        // データの言語指定
        public string DataLanguage { get { return dataLanguage; } }
        [SerializeField]
        protected string dataLanguage = "";

        // 翻訳テキストのデータ
        [SerializeField]
        List<TextAsset> languageData = new List<TextAsset>();

        // UIのテキストローカライズを無視する
        public bool IgnoreLocalizeUiText { get { return ignoreLocalizeUiText; } }
        [SerializeField]
        bool ignoreLocalizeUiText = true;

        // ボイスのローカライズを無視する
        public bool IgnoreLocalizeVoice { get { return ignoreLocalizeVoice; } }
        [SerializeField]
        bool ignoreLocalizeVoice = true;

        // ボイスの対応言語
        public List<string> VoiceLanguages { get { return voiceLanguages; } }
        [SerializeField]
        List<string> voiceLanguages = new List<string>();

        // 空テキストの対応タイプ
        public LanguageBlankTextType BlankTextType { get { return blankTextType; } }
        [SerializeField]
        LanguageBlankTextType blankTextType = LanguageBlankTextType.SwapDefaultLanguage;

        // テキスト列の言語
        public List<string> TextColumnLanguages { get { return textColumnLanguages; } }
        [FormerlySerializedAs("textColumnErrorCheckLanguages")]
        [SerializeField]
        List<string> textColumnLanguages = new List<string>();

        // 言語切り替えで呼ばれるコールバック
        public Action OnChangeLanguage{
            get;
            set;
        }

        /// <summary>
        /// 現在の設定言語
        /// </summary>
        public string CurrentLanguage
        {
            get
            {
                return CurrentLanguage;
            }
            set
            {
                if(CurrentLanguage!=value)
                {
                    CurrentLanguage = value;
                    RefreshCurrentLanguage();
                }
            }
        }
        string currentLanguage;

        // ボイスの言語指定
        public string VoiceLanguage
        {
            get
            {
                return voiceLanguage;
            }
            set
            {
                if(voiceLanguage!=value)
                {
                    voiceLanguage = value;
                    RefreshCurrentLanguage();
                }
            }
        }
        string voiceLanguage = "";

        // ボイス言語を独立される可能性を考慮
        public string CurrentVoiceLanguage
        {
            get
            {
                if(!string.IsNullOrEmpty(VoiceLanguage))
                {
                    return VoiceLanguage;
                }
                else
                {
                    return CurrentLanguage;
                }
            }
        }

        LanguageData Data { get; set; }

        // 現在設定されている言語名のリスト
        public List<string> Languages { get { return Data.Languages; } }

        private void OnEnable()
        {
            Init();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Init();
        }
#endif

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Init()
        {
            Data = new LanguageData();
            foreach(var item in languageData)
            {
                if (item == null) continue;
                Data.OverwriteData(item);
            }
            Data.AddLanguage(this.dataLanguage);
            foreach(var item in TextColumnLanguages)
            {
                Data.AddLanguage(item);
            }

            // 設定された言語か、システムの言語に変更
            currentLanguage = (string.IsNullOrEmpty(language) || language == Auto) ? Application.systemLanguage.ToString() : language;
            voiceLanguage = "";
            RefreshCurrentLanguage();
        }

        // 現在の言語が変わった時の処理
        protected void RefreshCurrentLanguage()
        {
            if (Instance != this) return;
            if (OnChangeLanguage != null)
                OnChangeLanguage();
            OnRefreshCurrentLanguage();
        }

        // 現在の言語が変わった時の処理
        protected abstract void OnRefreshCurrentLanguage();

        /// <summary>
        /// 指定のキーのテキストを、指定のデータの、設定された言語に翻訳して取得
        /// </summary>
        /// <param name="dataName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string LocalizeText(string dataName,string key)
        {
            if(Data.ContainsKey(key))
            {
                string text;
                if(Data.TryLocalizeText(out text,CurrentLanguage,DefaultLanguage,key,dataName))
                {
                    return text;
                }
            }

            Debug.LogError($"データ名:{dataName} の中に指定したキー: {key} が見つかりませんでした");
            return key;
        }

        /// <summary>
        /// 指定のキーテキストを、全データ内から検索して、設定された言語に翻訳して取得
        /// </summary>
        /// <param name="key">テキストのキー</param>
        /// <returns>翻訳したテキスト</returns>
        public string LocalizeText(string key)
        {
            string text = key;
            TryLocalizeText(key, out text);
            return text;
        }

        /// <summary>
        /// 指定のキーのテキストを、全データ内から検索して、設定された言語に翻訳して取得
        /// </summary>
        /// <param name="key">テキストのキー</param>
        /// <returns>翻訳したテキスト</returns>
        public bool TryLocalizeText(string key,out string text)
        {
            text = key;
            if(Data.ContainsKey(key))
            {
                if(Data.TryLocalizeText(out text,CurrentLanguage,DefaultLanguage,key))
                {
                    return true;
                }
            }
            return false;
        }

        public string DefaultLanguageText(string key)
        {
            if(Data.ContainsKey(key))
            {
                string text;
                if(Data.TryLocalizeText(out text,DefaultLanguage,DefaultLanguage,key))
                {
                    return text;
                }
            }
            Debug.LogError(key + "言語キーが見つかりませんでした");
            return "";
        }

        internal void OverwriteData(StringGrid grid)
        {
            Data.OverwriteData(grid);
            
        }

        /// <summary>
        /// ローカライズ対象のテキスト系コマンドデータが空か？
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool IsEmptyTextCommand(StringGridRow row)
        {
            switch(this.BlankTextType)
            {
                case LanguageBlankTextType.NoBlankText:
                case LanguageBlankTextType.AllowBlankText:
                    foreach(var colum in TextColumnLanguages)
                    {
                        if(!row.IsEmptyCell(colum))
                        {
                            return false;
                        }
                    }
                    return true;
                default:
                    return true;
            }
        }

        /// <summary>
        /// 現在の設定言語にローカライズされたテキストを取得
        /// </summary>
        /// <param name="row"></param>
        /// <param name="defaultColumNane"></param>
        /// <returns></returns>
        public string ParseCellLocalizedText(StringGridRow row,string defaultColumNane)
        {
            switch(this.BlankTextType)
            {
                case LanguageBlankTextType.SwapDefaultLanguage:
                    return ParseCellLocalizedTextBySwapDefaultLanguage(row,defaultColumNane);
                case LanguageBlankTextType.NoBlankText:
                    return ParseCellLocalizedTextByNoSwap(row,defaultColumNane);
                    case LanguageBlankTextType.AllowBlankText:
                    return ParseCellLocalizedTextByNoSwap(row,defaultColumNane);
                default:
                    Debug.LogError(row.ToErrorString(this.BlankTextType.ToString()+" 判別できないタイプです"));
                    return "";
            }
        }

        /// <summary>
        /// 現在の設定言語にローカライズされたテキストを取得
        /// </summary>
        /// <param name="row"></param>
        /// <param name="defaultColumName"></param>
        /// <returns></returns>
        string ParseCellLocalizedTextBySwapDefaultLanguage(StringGridRow row,string defaultColumName)
        {
            string columName = defaultColumName;
            if(row.Grid.ContainsColum(CurrentLanguage))
            {
                // 現在の言語があるなら、その列を
                columName = currentLanguage;
            }
            else
            {
                if(DataLanguage==CurrentLanguage)
                {
                    columName = defaultColumName;
                }
                else if(!string.IsNullOrEmpty(DefaultLanguage))
                {
                    columName=DefaultLanguage;
                }
                else
                {
                    if(!string.IsNullOrEmpty(DataLanguage))
                    {
                        if(CurrentLanguage==DataLanguage)
                        {
                            // [DataLanguage] で言語指定がある場合、Text列は指定言語の場合にのみ表示される
                            columName = defaultColumName;
                        }
                        else
                        {
                            // DefaultLanguageの列のテキストが基本の表示テキストとして使用されます。
                            columName = DefaultLanguage;
                        }
                    }
                }
            }
            if(row.IsEmptyCell(columName))
            {
                // 指定の言語が空なら、デフォルトのText列を
                // [DefaultLanguageの列のテキストが空の場合は、やはりText列のテキストを表示]
                return row.ParseCellOptional<string>(defaultColumName, "");
            }
            else
            {
                // 指定の言語を
                return row.ParseCellOptional<string>(columName, "");
            }
        }

        // 現在の設定言語にローカライズされたテキストを取得
        string ParseCellLocalizedTextByNoSwap(StringGridRow row,string defaultColumName)
        {
            string columName = GetLocalizedColumName(defaultColumName);
            if(!row.Grid.ContainsColum(columName))
            {
                Debug.LogError(row.ToErrorString(columName + " is empty column.Set Localize text colum"));
                return "";
            }

            if (this.BlankTextType == LanguageBlankTextType.NoBlankText)
            {
                // テキストセルの内容が空で、PageCtrlの設定もない場合はエラーを出す
                if (row.IsEmptyCell(columName) && row.IsEmptyCell(AdvColumName.PageCtrl.QuickToString()))
                {
                    Debug.LogError(row.ToErrorString(columName + " is empty cell. Set localize text"));
                    return "";
                }
            }
                // 指定の言語を
                return row.ParseCellOptional<string>(columName, "");
        }

        string GetLocalizedColumName(string defaultColumName)
        {
            // ローカライズテキスト行が定義されているなら
            if(this.TextColumnLanguages.Contains(this.CurrentLanguage))
            {
                return CurrentLanguage;
            }

            // [DataLanguage] (Text列の言語指定)がないなら、デフォルト行名をそのまま
            if (string.IsNullOrEmpty(DataLanguage)) return defaultColumName;

            // [DataLanguage] で言語指定がある場合、Text列は指定言語の場合にのみ表示されるようになります。
            if(DataLanguage!=CurrentLanguage && !string.IsNullOrEmpty(DefaultLanguage))
            {
                return DefaultLanguage;
            }
            return defaultColumName;
        }

        /// <summary>
        /// ローカライズによってスキップページかどうかのチェック
        /// </summary>
        /// <param name="row"></param>
        /// <param name="defaultcolumName"></param>
        /// <returns></returns>
        public bool CheckSkipPage(StringGridRow row,string defaultcolumName)
        {
            if (!ContainsLocalizeText(row, defaultcolumName)) return false;
            return ParseCellLocalizedTextByNoSwap(row, defaultcolumName) == "<skip_page>";

        }

        /// <summary>
        /// ローカライズによってスキップしてよいかチェック
        /// </summary>
        /// <param name="row"></param>
        /// <param name="defaultcolumName"></param>
        /// <returns></returns>
        public bool CheckSkipByLocalize(StringGridRow row,string defaultcolumName)
        {
            if(!ContainsLocalizeText(row,defaultcolumName)) return false;
            bool isEmpty = ParseCellLocalizedTextByNoSwap(row, defaultcolumName).Length == 0;
            return isEmpty;
        }

        /// <summary>
        /// ローカライズテキストデータが何らかの言語に存在するか？
        /// </summary>
        /// <param name="row"></param>
        /// <param name="defaultColumName"></param>
        /// <returns></returns>
        bool ContainsLocalizeText(StringGridRow row,string defaultColumName)
        {
            if (!row.IsEmptyCell(defaultColumName)) return true;
            foreach(var colum in TextColumnLanguages)
            {
                if(!row.IsEmptyCell(colum))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace fantec
{
    /// <summary>
    /// 文字列グリッドデータの行
    /// </summary>
    [System.Serializable]
    public class StringGridRow
    {
        // 元になるグリッド
        public StringGrid Grid { get { return grid; } }
        [NonSerialized]
        StringGrid grid;

        // 行番号
        public int RowIndex { get { return this.rowIndex; } }
        [SerializeField]
        int rowIndex;

        /// <summary>
        /// デバッグ用のインデックス
        /// </summary>
        public int DebugIndex
        {
            get { return debugIndex; }
            set {  debugIndex = value; }
        }
#if UNITY_EDITOR
        [SerializeField]
#else
        [NonSerialized]
#endif
        int debugIndex = -1;

        // 文字列データ
        public string[] Strings { get { return this.strings; } }
        [SerializeField]
        string[] strings;

        // 文字列データの長さ
        public int Length { get { return strings.Length; } }

        // データが空かどうか
        public bool IsEmpty { get { return isEmpty; } }
        [SerializeField]
        bool isEmpty;

        // コメントアウトされているか
        public bool IsCommentOut { get { return isCommentOut; } }
        [SerializeField]
        bool isCommentOut;

        // データが空かどうか
        public bool IsEmptyOrCommentOut { get { return IsEmpty || IsCommentOut; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="grid">元になる文字列グリッド</param>
        /// <param name="rowIndex">行番号</param>
        public StringGridRow(StringGrid grid,int rowIndex)
        {
            this.rowIndex = this.DebugIndex = rowIndex;
            InitLink(grid);
        }

        /// <summary>
        /// 親とのリンクを初期化
        /// ScriptableObjectなどで読み込んだ場合、参照が切れているのでそれを再設定するために
        /// </summary>
        /// <param name="grid">元になる文字列グリッド</param>
        public void InitLink(StringGrid grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// CSVテキストから初期化
        /// </summary>
        /// <param name="type">CSVタイプ</param>
        /// <param name="text">CSVテキスト</param>
        public void InitFromCsvText(CsvType type,string text)
        {
            this.strings = text.Split(type==CsvType.Tsv? '\t':',');
            this.isEmpty = CheckEmpty();
            this.isCommentOut = CheckCommentOut();
        }

        /// <summary>
        /// 文字列リストから初期化
        /// </summary>
        /// <param name="stringList"></param>
        public void InitFromStringList(List<string>stringList)
        {
            InitFromStringArray(stringList.ToArray());
        }

        /// <summary>
        /// 文字列リストから初期化
        /// </summary>
        /// <param name="strings"></param>
        public void InitFromStringArray(string[]strings)
        {
            this.strings = strings;
            this.isEmpty = CheckEmpty();
            this.isCommentOut = CheckCommentOut();
        }

        /// <summary>
        /// 空かどうかチェック
        /// </summary>
        /// <returns></returns>
        bool CheckEmpty()
        {
            foreach(var str in strings)
            {
                if(!string.IsNullOrEmpty(str))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// コメントアウトされているかチェック
        /// </summary>
        /// <returns></returns>
        bool CheckCommentOut()
        {
            if (this.Strings.Length <= 0) return false;
            return this.Strings[0].StartsWith("//");
        }

        /// <summary>
        /// 指定した列名のセルが空かどうか
        /// </summary>
        /// <param name="columName"></param>
        /// <returns></returns>
        public bool IsEmptyCell(string columName)
        {
            int index;
            if(Grid.TryGetColumIndex(columName, out index))
            {
                return IsEmptyCell(index);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 列名がついたセル全て空かどうか
        /// </summary>
        /// <returns></returns>
        internal bool IsAllEmptyCellNamedColum()
        {
            foreach(var keyValue in Grid.ColumIndexTable)
            {
                if(!IsEmptyCell(keyValue.Value)&&!Grid.IsCommentCountColum(keyValue.Value))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 指定した列インデックスのセルが空かどうか
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsEmptyCell(int index)
        {
            return !(index < Length && !string.IsNullOrEmpty(strings[index]));
        }

        /// <summary>
        /// 指定した列名のセルを値に変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columName">列の名前</param>
        /// <returns></returns>
        public T ParseCell<T>(string columName)
        {
            T ret;
            if(!TryParseCell(columName, out ret))
            {
                Debug.LogError(ToErrorStringWithParseCollumName(columName));
            }
            return ret;
        }
        public T ParseCell<T>(int index)
        {
            T ret;
            if(!TryParseCell(index,out ret))
            {
                Debug.LogError(ToErrorStringWithParseColumIndex(index));
            }
                return ret;
        }

        /// <summary>
        /// 指定した列名のセルを値に変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columName"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public T ParseCellOptional<T>(string columName,T defaultVal)
        {
            T ret;
            return TryParseCell(columName,out ret) ? ret : defaultVal;
        }

        public T ParseCellOptional<T>(int index, T defaultVal)
        {
            T ret;
            return TryParseCell(index,out ret) ? ret : defaultVal;
        }

        /// <summary>
        /// 指定した列名のセルを値に変換を試みる
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columName"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool TryParseCell<T>(string columName,out T val)
        {
            int index;
            if(Grid.TryGetColumIndex(columName,out index))
            {
                return TryParseCell(index, out val);
            }
            else
            {
                val = default(T);
                return false;
            }
        }

        /// <summary>
        /// 指定した列インデックスのセルを値に変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool TryParseCell<T>(int index,out T val)
        {
            if(!IsEmptyCell(index))
            {
                if (TryParse<T>(strings[index],out val))
                {
                    return true;
                }
                else
                {
                    Debug.LogError(ToErrorStringWithParse(strings[index],index));
                    return false;
                }
            }
            else
            {
                val = default(T);
                return false;
            }
        }

        /// <summary>
        /// 型間違いを許容して、解析できなかった場合はデフォルト値を設定する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="defaultVal"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool TryParseCellTypeOptional<T>(int index,T defaultVal,out T val)
        {
            if(!IsEmptyCell(index))
            {
                if (TryParse<T>(strings[index],out val))
                {
                    return true;
                }
                else
                {
                    val = defaultVal;
                    return false;
                }
            }
            else
            {
                val = defaultVal;
                return false;
            }
        }

        /// <summary>
        /// 文字列を値に変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryParse<T>(string str,out T val)
        {
            try
            {
                System.Type type = typeof(T);
                if (type == typeof(string))
                {
                    val = (T)(object)str;
                }
                else if (type.IsEnum)
                {
                    val = (T)System.Enum.Parse(typeof(T), str);
                }
                else if (type == typeof(Color))
                {
                    Color color = Color.white;
                    bool ret = ColorUtil.TryParseColor(str, ref color);
                    val = ret ? (T)(object)color : default(T);
                    return ret;
                }
                else if (type == typeof(int))
                {
                    val = (T)(object)int.Parse(str);
                }
                else if (type == typeof(float))
                {
                    val = (T)(object)WrapperUnityVersion.ParseFloatGlobal(str);
                }
                else if (type == typeof(double))
                {
                    val = (T)(object)WrapperUnityVersion.ParseDoubleGlobal(str);
                }
                else if (type == typeof(bool))
                {
                    val = (T)(object)bool.Parse(str);
                }
                else
                {
                    System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(type);
                    val = (T)converter.ConvertFromString(str);
                }
                return true;
            }
            catch
            {
                val = default(T);
                return false;
            }
        }

        /// <summary>
        /// コメントアウトされている文字列を削除する
        /// </summary>
        public void EraseAllCommentOutStrings()
        {
            if(IsCommentOut)
            {
                // コメントアウトされている行を削除
                for(int i=0;i<this.strings.Length;++i)
                {
                    this.strings[i] = "";
                }
                return;
            }
        }
        /// <summary>
        /// 指定の列をコメントアウトされた文字列として削除する
        /// </summary>
        /// <param name="colum"></param>
        public void EraseCommentOutColum(int colum)
        {
            if (strings.Length <= colum) return;
            if (string.IsNullOrEmpty(strings[colum])) return;

            this.strings[colum] = "";
        }

        /// <summary>
        /// 指定した列名のセルを型Tのカンマ区切り配列として値に変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columName"></param>
        /// <returns></returns>
        public T[]ParseCellArray<T>(string columName)
        {
            T[] ret;
            if(!TryParseCellArray(columName,out ret))
            {
                Debug.LogError(ToErrorStringWithParseCollumName(columName));
            }
            return ret;
        }
        public T[]ParseCellArray<T>(int index)
        {
            T[] ret;
            if(!TryParseCellArray(index,out ret))
            {
                Debug.LogError(ToErrorStringWithParseColumIndex(index));
            }
            return ret;
        }

        /// <summary>
        /// 指定した列名のセルを型Tのカンマ区切り配列として値に変換
        /// 要素が空だった場合は、デフォルト値を返す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columName"></param>
        /// <param name="defaultVal"></param>
        /// <returns>変換後の結果</returns>
        public T[]ParseCellOptionalArray<T>(string columName, T[]defaultVal)
        {
            T[] ret;
            return TryParseCellArray(columName, out ret) ? ret : defaultVal;
        }
        public T[]ParseCellOptionalArray<T>(int index, T[]defaultVal)
        {
            T[] ret;
            return TryParseCellArray(index,out ret)? ret : defaultVal;
        }

        /// <summary>
        /// 指定した列名のセルを型Tのカンマ区切り配列として値に変換を試みる。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columName"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool TryParseCellArray<T>(string columName,out T[]val)
        {
            int index;
            if(Grid.TryGetColumIndex(columName,out index))
            {
                return TryParseCellArray(index, out val);
            }
            else
            {
                val = null;
                return false;
            }
        }

        /// <summary>
        /// 指定した列インデックスのセルを型Tのカンマ区切り配列として値に変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool TryParseCellArray<T>(int index,out T[]val)
        {
            if(!IsEmptyCell(index))
            {
                if (TryParseCellArray<T>(strings[index],out val))
                {
                    return true;
                }
                else
                {
                    Debug.LogError(ToErrorStringWithParse(strings[index], index));
                    return false;
                }
            }
            else
            {
                val = null;
                return false;
            }
        }

        bool TryPaseArray<T>(string str,out T[]val)
        {
            string[]strArray=str.Split(',');
            int count=strArray.Length;  
            val=new T[count];
            for(int i=0;i<count;++i)
            {
                T v;
                if (!TryParse<T>(strArray[i].Trim(),out v))
                {
                    return false;
                }
                else
                {
                    val[i] = v;
                }
            }
            return true;
        }

        /// <summary>
        /// デバッグ文字列に変換
        /// </summary>
        /// <returns>デバッグ文字列</returns>
        internal string ToDebugString()
        {
            char separator = Grid.CsvSeparator;

            string textOutput = "";
            foreach(string str in strings)
            {
                textOutput += " " + str + separator;
            }
            return textOutput;
        }

        // デバッグ用の情報(マクロなどでシート名が変わっているとき対策)
        // シリアライズはしないのでエディタ上でのみ有効
        internal string DebugInfo
        {
            get { return debugInfo; }
            set { debugInfo = value; }
        }
#if UNITY_EDITOR
        public UnityEngine.Object SourceAssetInEditor { get { return Grid.SourceAssetInEditor; } }
#else
        public UnityEngine.Object SourceAssetInEditor{get{return null;}}
#endif

#if UNITY_EDITOR
        [SerializeField]
#else
        [NonSerialized]
#endif
        string debugInfo;

        /// <summary>
        /// エラー用の文字列を取得
        /// </summary>
        /// <param name="msg">エラーメッセージ</param>
        /// <returns>エラー用のテキスト</returns>
        public string ToErrorString(string msg)
        {
            if(!msg.EndsWith("\n"))msg+= "\n";

            // デバッグ用の行番号
            int lineNo = this.DebugIndex + 1;
            if(string.IsNullOrEmpty(this.DebugInfo))
            {
                string sheetName = Grid.SheetName;
                msg += sheetName + ":" + lineNo + " ";
            }
            else
            {
                msg+= this.DebugInfo;
            }
            return msg
                + ColorUtil.AddColorTag(ToDebugString(), Color.red) + "\n"
                + "<b>" + Grid.Name + "</b>" + "  : " + lineNo;
        }

        /// <summary>
        /// エラー用の文字列を取得
        /// </summary>
        /// <returns></returns>
        public string ToStringOfFileSheetLine()
        {
            int lineNo = rowIndex + 1;
            return "<b>" + Grid.Name + "</b>" + " :" + lineNo;
        }

        /// <summary>
        /// 列名指定パースエラー出力
        /// </summary>
        /// <param name="columName"></param>
        /// <returns></returns>
        string ToErrorStringWithParseCollumName(string columName)
        {
            return ToErrorString(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.StringGridRowParseColumnName,columName));
        }

        /// <summary>
        /// 列インデックス指定パースエラー出力
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        string ToErrorStringWithParseColumIndex(int index)
        {
            return ToErrorString(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.StringGridRowParseColumnIndex, index));
        }

        /// <summary>
        /// パースエラー出力
        /// </summary>
        /// <param name="colum"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        string ToErrorStringWithParse(string colum,int index)
        {
            return ToErrorString(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.StringGridRowParse, index,colum));
        }
    }
}
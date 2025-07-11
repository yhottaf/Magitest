using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace fantec
{
    /// <summary>
    /// ファイルのタイプ
    /// </summary>
    public enum CsvType
    {
        Csv,
        Tsv,
    };

    /// <summary>
    /// 文字列のグリッド (CSVなどに使う)
    /// </summary>
    [System.Serializable]
    public class StringGrid
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        public StringGrid(string name,string sheetName,CsvType type)
        {
            this.name = name;
            this.sheetName = sheetName;
            this.type = type;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        public StringGrid(string name,CsvType type,string csvText,int headerRow)
        {
            Create(name, type, csvText, headerRow);
        }

        public StringGrid(string name,CsvType type,string csvText)
        {
            Create(name,type,csvText,0);
        }

        void Create(string name,CsvType type,string csvText,int headerRow)
        {
            this.name = name;
            this.type = type;
            Rows.Clear();
            // CSVデータを作成
            string[] stringSeparators = new string[] { "\r\n", "\n" };
            string[] lines = csvText.Split(stringSeparators, System.StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                StringGridRow row = new StringGridRow(this, Rows.Count);
                row.InitFromCsvText(type, lines[i]);
                Rows.Add(row);
            }
            ParseHeader(headerRow); 
            textLength = csvText.Length;
        }

        /// <summary>
        /// 行のデータ
        /// </summary>
        public List<StringGridRow> Rows { get { return this.rows ?? (rows = new List<StringGridRow>()); } }
        [SerializeField]
        List<StringGridRow> rows;

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get { return name; } }
        [SerializeField]
        string name;

        /// <summary>
        /// シート名
        /// </summary>
        public string SheetName
        {
            get
            {
                if(string.IsNullOrEmpty(sheetName))
                {
                    int sheetnameIndex = Name.LastIndexOf(":");
                    this.sheetName = Name;
                    if(sheetnameIndex>0)
                    {
                        this.sheetName = sheetName.Remove(0,sheetnameIndex+1);
                    }

                    if(sheetName.Contains("."))
                    {
                        //this.sheetName
                    }
                }
                return sheetName;
            }
        }
        string sheetName;

        /// <summary>
        /// CSVのタイプ
        /// </summary>
        public CsvType Type { get { return type; } }
        [SerializeField]
        CsvType type;

        /// <summary>
        /// CSVの区切り文字
        /// </summary>
        public char CsvSeparator { get { return (Type == CsvType.Csv) ? ',' : '\t'; } }

        /// <summary>
        /// テキストのサイズ(メモリ管理の目安にとっておく)
        /// </summary>
        public int TextLength { get { return textLength; } }
        [SerializeField]
        int textLength;

        // 列インデックスの名前引きテーブル
        Dictionary<string, int> columIndexTable;

        public Dictionary<string, int> ColumIndexTable
        {
            get { return columIndexTable; }
            set { columIndexTable = value; }
        }

        // ヘッダ情報の行番号
        public int HeaderRow { get { return HeaderRow; } }
        [SerializeField]
        protected int headerRow = 0;

        // データの先頭行番号
        public int DataTopRow { get { return HeaderRow + 1; } }
#if UNITY_EDITOR
        public Object SourceAssetInEditor { get { return sourceAssetInEditor; } set { sourceAssetInEditor = value; } }
        [SerializeField]
        Object sourceAssetInEditor = null;
#endif
        /// <summary>
        /// 行データとのリンクを設定
        /// ScriptableObjectなどで読み込んだ場合、参照が切れているのでそれを再設定するために
        /// </summary>
        public void InitLink()
        {
            Profiler.BeginSample("InitLink");
            foreach (var row in Rows)
            {
                Profiler.BeginSample("InitLinkCallBack");
                row.InitLink(this);
                Profiler.EndSample();
            }
            ParseHeader(headerRow);
            Profiler.EndSample();
        }

        /// <summary>
        /// 指定した列がコメントアウトされているか
        /// </summary>
        /// <param name="colum"></param>
        /// <returns></returns>
        internal bool IsCommentCountColum(int colum)
        {
            if (headerRow >= Rows.Count) return false;

            StringGridRow row = Rows[headerRow];
            if (colum >= row.Strings.Length) return false;
            return row.Strings[colum].StartsWith("//");
        }

        /// <summary>
        /// 文字列リストから行を追加
        /// </summary>
        /// <param name="stringList"></param>
        public void AddRow(List<string>stringList)
        {
            StringGridRow row = new StringGridRow(this,Rows.Count);
            row.InitFromStringList(stringList);
            Rows.Add(row);
            foreach(string str in stringList)
            {
                textLength += str.Length;
            }
        }

        /// <summary>
        /// 文字列リストから行を追加
        /// </summary>
        /// <param name="stringArray"></param>
        /// <returns></returns>
        public StringGridRow AddRow(string[]stringArray)
        {
            StringGridRow row=new StringGridRow(this,Rows.Count);
            row.InitFromStringArray(stringArray);
            Rows.Add(row);
            foreach(string str in stringArray)
            {
                textLength += str.Length;
            }
            return row;
        }

        /// <summary>
        /// ヘッダーの解析
        /// </summary>
        /// <param name="headerRow">ヘッダー情報のある行番号</param>
        public void ParseHeader(int headerRow)
        {
            Profiler.BeginSample("ParseHeader");
            this.headerRow = headerRow;
            ColumIndexTable = new Dictionary<string, int>();
            if(headerRow < Rows.Count)
            {
                StringGridRow row = Rows[headerRow];
                for(int i=0;i<row.Strings.Length;++i)
                {
                    string key = row.Strings[i];
                    if(ColumIndexTable.ContainsKey(key))
                    {
                        string errorMsg = "";
                        if(!string.IsNullOrEmpty(key))
                        {
                            errorMsg += row.ToErrorString(ColorUtil.AddColorTag(key, Color.red) + "is already contains");
                            Debug.LogError(errorMsg);
                        }
                    }
                    else
                    {
                        ColumIndexTable.Add(key,i);
                    }
                }
            }
            else
            {
                Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.StringGridParseHeader, headerRow, this.name));
            }
            Profiler.EndSample();
        }
        public void ParseHeader()
        {
            ParseHeader(0);
        }

        /// <summary>
        /// 列の名前があるか
        /// </summary>
        /// <param name="name">名前</param>
        /// <returns>正否</returns>
        public bool ContainsColum(string name)
        {
            return ColumIndexTable.ContainsKey(name);
        }

        /// <summary>
        /// 列の名前から列番号インデックスを取得を試みる
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool TryGetColumIndex(string name,out int index)
        {
            return ColumIndexTable.TryGetValue(name,out index);
        }

        public string ToText()
        {
            StringBuilder builder = new StringBuilder();
            char separator = CsvSeparator;
            foreach(StringGridRow row in Rows)
            {
                for(int i=0;i<row.Strings.Length;++i)
                {
                    // CSVの書式にあわせる
                    string line = row.Strings[i].Replace("\n","\\n");
                    builder.Append(line);
                    if(i<row.Strings.Length-1)
                    {
                        builder.Append(separator);
                    }
                }
                builder.Append("\n");
            }
            return builder.ToString();
        }

        // コメントアウトされている文字列を削除する
        public void EraseCommentOutStrings()
        {
            foreach (StringGridRow row in Rows)
            {
                row.EraseAllCommentOutStrings();
            }

            //コメントアウトされている列を削除
            for (int i = 0; i < this.ColumIndexTable.Count; ++i)
            {
                if (IsCommentCountColum(i))
                {
                    foreach (StringGridRow row in Rows)
                    {
                        row.EraseCommentOutColum(i);
                    }
                }
            }
        }

        /// <summary>
        /// 不要な末尾の行を削除する
        /// </summary>
        /// <param name="checkCount"></param>
        public void ShapeUpRow(int checkCount)
        {
            int lastDataRowIndex = 0;
            for(int i=0;i<Rows.Count;i++)
            {
                var row= Rows[i];
                if(!row.IsEmpty)
                {
                    lastDataRowIndex = i;
                }
            }
            int index = lastDataRowIndex + 1;
            if(index>=Rows.Count)
            {
                return;
            }

            int old = Rows.Count;
            int count = Rows.Count - index;
            Rows.RemoveRange(index, count);
            if(count>checkCount)
            {
                // ログ
                Debug.LogWarningFormat("Blank Row count is{0} . The data is very large. Delete the extra data at the end of the sheet. {1}",count,this.Name);
            }
        }
    }
}
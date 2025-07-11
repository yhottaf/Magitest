using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace fantec
{
    public class LanguageData
    {
        /// <summary>
        /// 対応する言語リスト
        /// </summary>
        public List<string> Languages { get { return languages; } }
        List<string> languages = new List<string>();

        // 言語による表示テキストデータ
        Dictionary<string, LanguageStrings> dataTbl = new Dictionary<string, LanguageStrings>();

        public class LanguageStrings
        {
            public List<string> Strings { get; private set; }

            public LanguageStrings()
            {
                Strings=new List<string>();
            }

            internal void SetData(List<string>strings)
            {
                Strings = strings;
            }
        }

        public void AddLanguage(string language)
        {
            if (string.IsNullOrEmpty(language)) return;
            if(!languages.Contains(language))
            {
                languages.Add(language);
            }
        }

        /// <summary>
        /// キーがあるかどうか
        /// </summary>
        /// <param name="key">テキストのキー</param>
        /// <returns>あればtrue。なければfalse</returns>
        public bool ContainsKey(string key)
        {
            return dataTbl.ContainsKey(key);
        }

        internal bool TryLocalizeText(out string text,string CurrentLanguage,string DefaultLanguage,string key,string dataName="")
        {
            text = key;
            if(!ContainsKey(key))
            {
                Debug.LogError(key+": 言語データが見つかりませんでした");
                return false;
            }
            string language = CurrentLanguage;
            if(!Languages.Contains(CurrentLanguage))
            {
                if (!Languages.Contains(DefaultLanguage)) return false;

                language = DefaultLanguage;
            }

            int index=Languages.IndexOf(language);
            LanguageStrings strings = dataTbl[key];
            if (index >= strings.Strings.Count) return false;

            text = strings.Strings[index];
            return true;
        }

        internal void OverwriteData(TextAsset tsv)
        {
            OverwriteData(new StringGrid(tsv.name, CsvType.Tsv, tsv.text));
        }
        internal void OverwriteData(StringGrid grid)
        {
            Dictionary<int,int>indexTable=new Dictionary<int,int>();
            StringGridRow header = grid.Rows[0];
            for(int i=0;i<header.Length;++i)
            {
                if (i == 0) continue;
                string language = header.Strings[i];
                AddLanguage(language);

                int index=languages.IndexOf(language);
                if (indexTable.ContainsKey(index))
                {
                    Debug.LogError(language + " already exists in " + grid.Name);
                    continue;
                }
                indexTable.Add(index, i);
            }

            foreach(StringGridRow row in grid.Rows)
            {
                if (row.IsEmptyOrCommentOut) continue;
                if (row.RowIndex == 0) continue;

                string key = row.Strings[0];
                if(string.IsNullOrEmpty(key)) continue; 

                if(!dataTbl.ContainsKey(key))
                {
                    dataTbl.Add(key, new LanguageStrings());
                }

                int count = languages.Count;
                List<string>strings= new List<string>(count);
                for(int i=0;i<count;++i)
                {
                    string text = "";
                    if(indexTable.ContainsKey(i))
                    {
                        int index = indexTable[i];
                        if(index<row.Strings.Length)
                        {
                            text = row.Strings[index].Replace("\\n","\n");
                        }
                    }
                    strings.Add(text);
                }
                dataTbl[key].SetData(strings);
            }
        }
    }
}
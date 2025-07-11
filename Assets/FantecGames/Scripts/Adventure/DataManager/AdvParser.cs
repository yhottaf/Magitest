namespace fantec
{

    /// <summary>
    /// ADVデータ解析
    /// </summary>
    public class AdvParser
    {
        public static string Localize(AdvColumName name)
        {
            //多言語化をしてみたけど、複雑になってかえって使いづらそうなのでやめた
            return name.QuickToString();
        }

        //指定の名前のセルを、型Tとして解析・取得（データがなかったらエラーメッセージを出す）
        public static T ParseCell<T>(StringGridRow row, AdvColumName name)
        {
            return row.ParseCell<T>(Localize(name));
        }

        //指定の名前のセルを、型Tとして解析・取得（データがなかったらデフォルト値を返す）
        public static T ParseCellOptional<T>(StringGridRow row, AdvColumName name, T defaultVal)
        {
            return row.ParseCellOptional<T>(Localize(name), defaultVal);
        }

        //指定の名前のセルを、型Tとして解析・取得（データがなかったらfalse）
        public static bool TryParseCell<T>(StringGridRow row, AdvColumName name, out T val)
        {
            return row.TryParseCell<T>(Localize(name), out val);
        }

        //セルが空かどうか
        public static bool IsEmptyCell(StringGridRow row, AdvColumName name)
        {
            return row.IsEmptyCell(Localize(name));
        }

        //ローカライズも含めてテキスト系コマンドデータが空かどうか
        public static bool IsEmptyTextCommand(StringGridRow row)
        {
            if (!IsEmptyCell(row, AdvColumName.PageCtrl) || !IsEmptyCell(row, AdvColumName.Text))
            {
                return false;
            }
            LanguageManagerBase languageManager = LanguageManagerBase.Instance;
            if (languageManager == null) return true;
            return languageManager.IsEmptyTextCommand(row);
        }


        //現在の設定言語にローカライズされたテキストを取得
        public static string ParseCellLocalizedText(StringGridRow row, AdvColumName defaultColumnName)
        {
            return ParseCellLocalizedText(row, defaultColumnName.QuickToString());
        }

        //現在の設定言語にローカライズされたテキストを取得
        public static string ParseCellLocalizedText(StringGridRow row, string defaultColumnName)
        {
            LanguageManagerBase languageManager = LanguageManagerBase.Instance;
            if (languageManager == null) return row.ParseCellOptional<string>(defaultColumnName, "");

            return languageManager.ParseCellLocalizedText(row, defaultColumnName);
        }
    }
}

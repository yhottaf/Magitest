using UnityEngine;

namespace fantec
{
    /// <summary>
    /// 表示言語切り替え用のクラス
    /// </summary>
    public class LanguageManager : LanguageManagerBase
    {
        protected override void OnRefreshCurrentLanguage()
        {
            if(!IgnoreLocalizeUiText)
            {
                UguiLocalizeBase[] localizeTable = FindObjectsOfType<UguiLocalizeBase>();
                foreach(var item in localizeTable)
                {
                    item.OnLocalize();
                }
            }
        }
    }
}
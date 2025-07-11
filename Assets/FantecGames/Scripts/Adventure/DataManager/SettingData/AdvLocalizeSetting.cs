
namespace fantec
{
    /// <summary>
    /// ローカライズの設定
    /// </summary>
    public class AdvLocalizeSetting : AdvSettingBase
    {
        protected override void OnParseGrid(StringGrid grid)
        {
            LanguageManagerBase languageManager = LanguageManagerBase.Instance;
            if (languageManager != null)
            {
                languageManager.OverwriteData(grid);
            }
        }
    }
}

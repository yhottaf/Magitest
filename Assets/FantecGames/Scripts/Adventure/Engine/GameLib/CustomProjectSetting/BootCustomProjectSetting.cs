using fantec;
using UnityEngine;

/// <summary>
/// ゲーム起動処理のサンプル
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("Fantec/Lib/Other/BootCustomProjectSetting")]
public class BootCustomProjectSetting : MonoBehaviour
{
    public CustomProjectSetting CustomProjectSetting
    {
        get { return customProjectSetting; }
        set { customProjectSetting = value; }
    }
    [SerializeField]
    CustomProjectSetting customProjectSetting;
}

using System.Linq;
using UnityEditor;
using UnityEditor.Build;

[InitializeOnLoad]
public static class AutoDefineSymbols
{
    static AutoDefineSymbols()
    {
        AddDefine(BuildTargetGroup.Android, "DOTWEEN");
        AddDefine(BuildTargetGroup.Android, "ENABLE_PLAYFABADMIN_API");
        AddDefine(BuildTargetGroup.Android, "ENABLE_PLAYFABSERVER_API");
        AddDefine(BuildTargetGroup.Android, "UNITASK_DOTWEEN_SUPPORT");
        AddDefine(BuildTargetGroup.Android, "!DISABLE_PLAYFAB_STATIC_API");

        AddDefine(BuildTargetGroup.iOS, "DOTWEEN");
        AddDefine(BuildTargetGroup.iOS, "ENABLE_PLAYFABADMIN_API");
        AddDefine(BuildTargetGroup.iOS, "ENABLE_PLAYFABSERVER_API");
        AddDefine(BuildTargetGroup.iOS, "UNITASK_DOTWEEN_SUPPORT");
        AddDefine(BuildTargetGroup.iOS, "!DISABLE_PLAYFAB_STATIC_API");

        AddDefine(BuildTargetGroup.Standalone, "DOTWEEN");
        AddDefine(BuildTargetGroup.Standalone, "ENABLE_PLAYFABADMIN_API");
        AddDefine(BuildTargetGroup.Standalone, "ENABLE_PLAYFABSERVER_API");
        AddDefine(BuildTargetGroup.Standalone, "UNITASK_DOTWEEN_SUPPORT");
        AddDefine(BuildTargetGroup.Standalone, "!DISABLE_PLAYFAB_STATIC_API");
    }
    static void AddDefine(BuildTargetGroup group, string define)
    {
        var namedTarget = NamedBuildTarget.FromBuildTargetGroup(group);
        var current = PlayerSettings.GetScriptingDefineSymbols(namedTarget);

        if (!current.Split(';').Contains(define))
        {
            PlayerSettings.SetScriptingDefineSymbols(namedTarget, current + ";" + define);
        }
    }
}

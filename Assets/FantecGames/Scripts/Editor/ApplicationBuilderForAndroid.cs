using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEngine;


public class ApplicationBuilderForAndroid
{
    [MenuItem("Build/ApplicationBuilder/Android")]
    public static void Build()
    {
        const string outputDirKey = "-output-dir";

        //AndroidにSwitch Platform
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        var args = Environment.GetCommandLineArgs();
        var locationPathName = GetArgumentValue(args, outputDirKey);
        if(string.IsNullOrEmpty(locationPathName))
        {
            Debug.Log("コマンドラインからの引数が見つからないためパスを直接指定します。");
            locationPathName = Application.dataPath.Replace("Assets", "Builds/Android/magi.apk");
        }
        else
        {

        }
        Debug.Log(locationPathName);

        var options = new BuildPlayerOptions();
        options.scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        options.locationPathName= locationPathName;
        options.target = BuildTarget.Android;
        options.options = BuildOptions.Development;

        PlayerSettings.applicationIdentifier = "com.DefaultCompany.magi";
        PlayerSettings.productName = "magi";
        PlayerSettings.companyName = "DefaultCompany";
        PlayerSettings.SplashScreen.show = false;
        PlayerSettings.SplashScreen.showUnityLogo = false;
        EditorUserBuildSettings.buildAppBundle = false;

        var buildReport = BuildPipeline.BuildPlayer(options);
        if(buildReport.summary.result==BuildResult.Succeeded)
        {
            Debug.Log("[Success]");
        }
        else
        {
            Debug.Log("[Failure]" + buildReport);
        }
    }

    private static string GetArgumentValue(IReadOnlyList<string> args,string key)
    {
        var index = args.ToList().FindIndex(arg => arg == key);
        var paramIndex = index + 1;

        if(index<0||args.Count()<=paramIndex)
        {
            return null;
        }

        return args[paramIndex];
    }
}

using fantec.Menu;
using UnityEngine;

/// <summary>
/// PlayerPrefs を管理するラッパークラス
/// </summary>
public static class PlayerPrefsManager
{
    public static string UserId
    {
        get => PlayerPrefs.GetString("UserId");
        set
        {
            PlayerPrefs.SetString("UserId",value);
            PlayerPrefs.Save();
        }
    }

    public static string AssetBundleVersion
    {
        get => PlayerPrefs.GetString("AssetBundleVersion","");
        set
        {
            PlayerPrefs.SetString("AssetBundleVersion",value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// メールアドレスを使ってログイン済みならtrue
    /// </summary>
    public static bool IsLoginEmailAddres
    {
        get => bool.TryParse(PlayerPrefs.GetString("IsLoginEmailAddress"), out var result) && result;
        set
        {
            PlayerPrefs.SetString("IsLoginEmailAddress",value.ToString());
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// ログインボーナスを獲得したときにtrueをセットする
    /// ログインボーナス演出を表示したらfalseに戻す
    /// </summary>
    public static bool HasLoginBonus
    {
        get => bool.TryParse(PlayerPrefs.GetString("HasLoginBonus"), out var result) && result;
        set
        {
            PlayerPrefs.SetString("HasLoginBonus", value.ToString());
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// パーティー名の設定
    /// </summary>
    /// <param name="name">パーティー名</param>
    /// <param name="index">パーティーのインデックス</param>
    public static void SetPartyName(string name,int index)
    {
        PlayerPrefs.SetString($"PartyName{index}",name);
        PlayerPrefs.Save();
    }

    public static string GetPartyName(int index)
    {
        return PlayerPrefs.GetString($"PartyName{index}", $"ディレクトリ{index + 1}");
    }

    /// <summary>
    /// 選択しているパーティーのインデックス
    /// </summary>
    public static int SelectPartyIndex
    {
        get => PlayerPrefs.GetInt(nameof(SelectPartyIndex), 0);
        set
        {
            PlayerPrefs.SetInt(nameof(SelectPartyIndex), value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// オプションの設定
    /// </summary>
    /// <param name="basic">基本設定</param>
    /// <param name="battle">バトル</param>
    /// <param name="sound">サウンド</param>
    public static void SetOptionData(OptionManager.BasicData basic,OptionManager.BattleData battle,OptionManager.SoundData sound)
    {
        SetBool("OptionBasicMessage", basic.bMessage);
        SetBool("OptionBasicQuality", basic.bQuality);
        SetBool("OptionBasicNoticeStamina", basic.bNoticeStamina);
        SetBool("OptionBasicNoticeMidnight", basic.bNoticeMidnight);
        SetBool("OptionBasicNoticeBilling", basic.bNoticeBilling);

        SetBool("OptionBattleAuto", battle.bAuto);
        SetBool("OptionBattleSkillCutIn", battle.bSkillCutIn);

        PlayerPrefs.SetFloat("OptionSoundVolumeBGM", sound.nVolumeBGM);
        PlayerPrefs.SetFloat("OptionSoundVolumeSE", sound.nVolumeSE);
        PlayerPrefs.SetFloat("OptionSoundVolumeVoice", sound.nVolumeVoice);
        SetBool("OptionSoundBGMToggle", sound.nBGMToggle);
        SetBool("OptionSoundSEToggle", sound.nSEToggle);
        SetBool("OptionSoundVoiceToggle", sound.nVoiceToggle);

        //SetBool("OptionStoryStaging", story.bStaging);
        //SetBool("OptionStoryCharacterFeed", story.bCharacterFeed);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// 基本設定取得
    /// </summary>
    /// <returns>基本設定</returns>
    public static OptionManager.BasicData GetOptionBasicData()
    {
        return new OptionManager.BasicData(
            GetBool("OptionBasicMessage", true),
            GetBool("OptionBasicQuality", true),
            GetBool("OptionBasicNoticeStamina", true),
            GetBool("OptionBasicNoticeMidnight", true),
            GetBool("OptionBasicNoticeBilling", true)
            );
    }

    /// <summary>
    /// バトル設定取得
    /// </summary>
    /// <returns></returns>
    public static OptionManager.BattleData GetOptionBattleData()
    {
        return new OptionManager.BattleData(
            GetBool("OptionBattleAuto", true),
            GetBool("OptionBattleSkillCutIn", true)
            );
    }

    /// <summary>
    /// サウンド設定取得
    /// </summary>
    private const float m_DefaultVolume = 0.65f;

    public static OptionManager.SoundData GetOptionSoundData()
    {
        if (!PlayerPrefs.HasKey("OptionSoundVolumeBGM") &&
            !PlayerPrefs.HasKey("OptionSoundVolumeSE") &&
            !PlayerPrefs.HasKey("OptionSoundVolumeVoice"))
        {
            return new OptionManager.SoundData(
         m_DefaultVolume, m_DefaultVolume, m_DefaultVolume,
          GetBool("OptionSoundBGMToggle", true),
          GetBool("OptionSoundSEToggle", true),
          GetBool("OptionSoundVoiceToggle", true));
        }
        else
        {
            return new OptionManager.SoundData(
             PlayerPrefs.GetFloat("OptionSoundVolumeBGM"),
             PlayerPrefs.GetFloat("OptionSoundVolumeSE"),
             PlayerPrefs.GetFloat("OptionSoundVolumeVoice"),
           GetBool("OptionSoundBGMToggle", true),
           GetBool("OptionSoundSEToggle", true),
           GetBool("OptionSoundVoiceToggle", true));
        }
    }

    /// <summary>
    /// 前回プレイしたクエストID
    /// </summary>
    public static int LastTimeQuestId
    {
        get => PlayerPrefs.GetInt("LastTimeQuestId");
        set
        {
            PlayerPrefs.SetInt("LastTimeQuestId",value);
            PlayerPrefs.Save();
        }
    }

    private static bool GetBool(string key,bool defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }

    private static void SetBool(string key,bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }
}

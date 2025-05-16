using UnityEngine;

namespace fantec.Common
{
    public class AssetPath
    {
        //-------------------------------------------------------------------------------------//
        // Project
        //-------------------------------------------------------------------------------------//
        public const string MasterFolderPath = "Assets/FantecGames/Master/";
        public const string MasterExcelDataFolderPath = MasterFolderPath + "Excel/";
        public const string MasterLocalDataFolderPath = MasterFolderPath + "Local/";
        public const string MasterLocalDataWaveFolderPath = MasterLocalDataFolderPath + "Wave";
        public const string MasterExcelDataWaveFolderPath = MasterExcelDataFolderPath + "Wave";
        public const string MasterLocalDataExpFolderPath = MasterLocalDataFolderPath + "Exp";
        public const string MasterExcelDataExpFolderPath = MasterExcelDataFolderPath + "Exp";
        public const string MasterLocalDataGrowthFolderPath = MasterLocalDataFolderPath + "Growth";
        public const string MasterExcelDataGrowthFolderPath = MasterExcelDataFolderPath + "Growth";
        public const string MasterLocalDataGachaTableFolderPath = MasterLocalDataFolderPath + "GachaTable";
        public const string MasterExcelDataGachaTableFolderPath = MasterExcelDataFolderPath + "GachaTable";



        //-------------------------------------------------------------------------------------//
        //Resources
        //-------------------------------------------------------------------------------------//
        public static readonly string ResourcesPath = "Assets/FantecGames/Resources_moved/";
        //Sprite
        public static readonly string SpriteFolderPath = ResourcesPath+"Sprite/";
        public static string GetSpriteRarityFramePath(int rarity) => SpriteFolderPath + "Card/RarityFrame/" + $"card_frame{rarity.ToString()}.png";
        public static string GetSpriteRarityBackgroundPath(int rarity) => SpriteFolderPath + "Card/RarityBackground/" + $"battle_card_bg{rarity.ToString()}";
        public static string GetSpriteRaritySortBasePath(int rarity) => SpriteFolderPath + "Card/RaritySortBase/" + $"card_sort_base_0{rarity.ToString()}";
        public static string GetSpriteItemIcon(int itemId) => SpriteFolderPath + "Item/" + itemId + ".png";
        public static readonly string SpriteBackGroundPath = SpriteFolderPath + "BackGround/";
        public static readonly string SpriteFieldPath = SpriteFolderPath + "StageField/";
        public static string GetSpriteQuestMapBackGround(int chapterNumber) => SpriteFolderPath + "QuestMap/" + chapterNumber.ToString();
        public static string GetSpriteSkillIcon(int iconId) => SpriteFolderPath + "SkillIcon/" + iconId;

        //プレハブ
        public static readonly string PrefabFolderPath = "Prefabs/";

        //UI
        public static readonly string UiPrefabFolderPath = PrefabFolderPath + "UI/";
        public static readonly string UiOverlayCanvas = UiPrefabFolderPath + "OverlayCanvas";
        public static readonly string UiFade = UiPrefabFolderPath + "FadeView";
        public static readonly string UiModal = UiPrefabFolderPath + "ModalView";
        public static readonly string UiConnecting = UiPrefabFolderPath + "ConnectingView";
        public static readonly string UiMessageCanvas = UiPrefabFolderPath + "MessageCanvas";
        public static readonly string UiLoading = UiPrefabFolderPath + "LoadingView";

        //サウンド
        public static readonly string SoundFolderPath = ResourcesPath+"Sound/";
        //BGM
        public static readonly string BGMFolderPath = SoundFolderPath + "BGM/";
        //SE
        public static readonly string SEFolderPath = SoundFolderPath + "SE/";
        //Voice
        public static readonly string VoiceFolderPath = SoundFolderPath + "Voice/";

        //スクリプタブルオブジェクト
        public static readonly string DataFolderPath = "Data/";

        //キャラクター
        public static readonly string CharacterFolderPath = ResourcesPath + "Character/";

        public static string GetCharacterSpriteSpherePath(int originId) => CharacterFolderPath + originId + "/SpriteSphere.png";

        public static string GetCharacterSpriteCutinPath(int originId) => CharacterFolderPath + originId + "/SpriteCutin.png";
        public static string GetCharacterSpinePath(int originId) => CharacterFolderPath + originId + "/Spine.asset";
        public static string GetCharacterSpriteCapsulePath(int originId) => CharacterFolderPath + originId + "/SpriteCapsule.png";

        // Live2D
        public static readonly string Live2DFolderPath = ResourcesPath + "Live2D/";
        public static string GetLive2DPath(int originId) => Live2DFolderPath + originId + "/Live2D.prefab";
        public static string GetIdleMotion(int originId) => Live2DFolderPath + originId + "/motions/Idle.anim";
        public static string GetLive2DMotion(int originId, string fileName) => Live2DFolderPath + originId + "/motions/" + fileName;
    }
}
namespace fantec.Menu
{
    /// <summary>
    /// オプションの設定内容を管理する
    /// </summary>
    public class OptionManager
    {
        /// <summary>
        /// 基本設定
        /// </summary>
        public struct BasicData
        {
            public bool bMessage;        // ストーリースキップ時に確認メッセージを表示するか？
            public bool bQuality;        // 画質の品質設定 (true:標準版 false :簡易版)
            public bool bNoticeStamina;  // スタミナ全回復通知を受け取るか？
            public bool bNoticeMidnight; // 深夜帯は通知を受け取らないか？
            public bool bNoticeBilling;  // 購入金額が一定金額に達したときに通知を受け取るか？

            public BasicData(bool bMessage,bool bQuality,
                bool bNoticeStamina,bool bNoticeMidnight,bool bNoticeBilling)
            {
                this.bMessage = bMessage;
                this.bQuality = bQuality;
                this.bNoticeStamina = bNoticeStamina;
                this.bNoticeMidnight = bNoticeMidnight;
                this.bNoticeBilling = bNoticeBilling;
            }
        }

        /// <summary>
        /// バトル設定
        /// </summary>
        public struct BattleData
        {
            public bool bAuto;       // バトル開始時にAUTOにするか？
            public bool bSkillCutIn; // スキルのカットインを表示するか？

            public BattleData(bool bAuto,bool bSkillCutIn)
            {
                this.bAuto = bAuto;
                this.bSkillCutIn = bSkillCutIn;
            }
        }

        /// <summary>
        /// サウンド設定
        /// </summary>
        public struct SoundData
        {
            public float nVolumeBGM;   //BGMのボリューム
            public float nVolumeSE;    //SEのボリューム
            public float nVolumeVoice;  //ボイスのボリューム
            public bool nBGMToggle;    //BGMのトグルのオンオフ
            public bool nSEToggle;     //SEのトグルのオンオフ
            public bool nVoiceToggle;  //Voiceのトグルのオンオフ

            public SoundData(float nVolumeBGM,float nVolumeSE,float nVolumeVoice,
                bool nBGMToggle,bool nSEToggle,bool nVoiceToggle)
            {
                this.nVolumeBGM= nVolumeBGM;
                this.nVolumeSE= nVolumeSE;
                this.nVolumeVoice = nVolumeVoice;
                this.nBGMToggle= nBGMToggle;
                this.nSEToggle= nSEToggle;
                this.nVoiceToggle= nVoiceToggle;
            }
        }
    }
}
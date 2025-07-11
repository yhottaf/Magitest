namespace fantec
{
    // サウンド管理のインターフェース
    public interface SoundManagerSystemInterfaceEx
    {
        // 指定のグループのサウンドが鳴っているか
        bool IsPlaying(string groupName);
    }
}

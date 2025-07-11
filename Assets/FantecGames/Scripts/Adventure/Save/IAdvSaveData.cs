namespace fantec
{
    //カスタムセーブデータの入出力用のインターフェース
    public interface IAdvSaveData : IBinaryIO
    {
        //クリアする
        void OnClear();
    }
}
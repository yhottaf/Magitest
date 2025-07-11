namespace fantec
{
    public enum AdvParticleStopType
    {
        Default,        //デフォルト操作のまま
        Clear,          //即座に消す。StopEmittingAndClearと同じ
        StopEmitting,   //新たな発生だけとめる。ループも切る
    }
}

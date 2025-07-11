namespace fantec
{
    interface IAdvGraphicObjectParticleController
    {
        bool EnableSave { get; }
        void Stop(AdvParticleStopType stopType);
    }
}

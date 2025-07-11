namespace fantec
{
    public interface IAdvInitOnCreateEntity
    {
        void InitFromPageData(AdvScenarioPageData page);
        void InitOnCreateEntity(AdvCommand command);
    }
}

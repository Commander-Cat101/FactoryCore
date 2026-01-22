namespace FactoryCore.API
{
    [JsonObject]
    public class ModuleInput
    {
        public string Name;
        public Guid Id;
        public Type Type;
        public Guid OutputGuid;
    }
}

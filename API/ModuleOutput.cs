namespace FactoryCore.API
{
    [JsonObject]
    public class ModuleOutput
    {
        public string Name;
        public Guid Id;
        public Type Type;
        public List<Guid> InputsGuids = new List<Guid>();
        [JsonIgnore]
        public Func<object> OutputFunc;
        
    }
}

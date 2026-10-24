using FactoryCore.API.ModuleValues;
using System.Text.Json.Serialization;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace FactoryCore.API
{
    public abstract class Module
    {
        [JsonIgnore]
        public virtual bool IsRemovable { get; } = true;

        [JsonIgnore]
        public virtual bool IsDuplicable { get; } = true;
        [JsonIgnore]
        public abstract string Name { get; }

        [JsonIgnore]
        public virtual string Description { get; } = string.Empty;

        [JsonInclude]
        public Guid Id;

        [JsonInclude]
        public Dictionary<string, object> PropertiesData = new Dictionary<string, object>();

        [JsonInclude]
        public List<ModuleOutput> Outputs = new List<ModuleOutput>();

        [JsonInclude]
        public List<ModuleInput> Inputs = new List<ModuleInput>();

        [JsonInclude]
        public float XPosition;

        [JsonInclude]
        public float YPosition;

        [JsonIgnore]
        public Template Template;

        [JsonIgnore]
        internal bool HasInited = false;

        [JsonIgnore]
        public Vector2 Position { get => new Vector2(XPosition, YPosition); set { XPosition = value.x; YPosition = value.y; } }

        [JsonIgnore] internal List<ModuleProperty> Properties = new List<ModuleProperty>();
        protected void AddInput<T>(string name)
        {
            if (Inputs.Any(a => a.Name == name))
                return;
            Inputs.Add(new ModuleInput() { Type = typeof(T), Id = Guid.NewGuid(), Name = name });
        }
        protected void AddOutput<T>(string name, Func<object> getValue)
        {
            var existingOutput = Outputs.FirstOrDefault(a => a.Name == name);
            if (existingOutput != null)
            {
                existingOutput.OutputFunc = getValue;
                return;
            }
            Outputs.Add(new ModuleOutput() { Type = typeof(T), Id = Guid.NewGuid(), Name = name, OutputFunc = getValue });
        }
        protected void AddProperty(ModuleProperty property)
        {
            if (Properties.Any(a => a.Name == property.Name))
                throw new Exception($"Trying to add new property with duplicate name ({property.Name}) - {property.GetType()}");
            property.Module = this;
            property.LoadData();
            Properties.Add(property);
        }
        public void SetValue(object value, string name)
        {
            PropertiesData[name] = value;
        }
        public T GetInputValue<T>(string name)
        {
            var input = Inputs.FirstOrDefault(a => a.Name == name);
            if (input == null)
                throw new Exception("Input is null");
            var linkedOutput = Template.modules.FirstOrDefault(a => a.Outputs.Any(a => a.InputsGuids.Contains(input.Id)));
            if (linkedOutput == null)
                throw new Exception("Output is null");

            ModuleOutput output = linkedOutput.Outputs.FirstOrDefault(a => a.Id == input.OutputGuid);
            if (output == null)
                throw new Exception("Output doesnt exist");
            return (T)output.OutputFunc.Invoke();
        }
        public List<Module> GetOutputsModules(string name)
        {
            var modules = new List<Module>();

            var output = Outputs.FirstOrDefault(a => a.Name == name);
            if (output == null)
                throw new Exception("Output is null");
            if (Template == null)
                throw new Exception("Template is null");

            foreach (var inputGuid in output.InputsGuids)
            {
                var module = Template.modules.FirstOrDefault(a => a.Inputs.Any(a => a.Id == inputGuid));
                modules.Add(module);
            }

            return modules;
        }
        public T GetValue<T>(string name)
        {
            if (!HasValue(name))
                throw new Exception("Module doesnt contain value entry.");
            var value = PropertiesData[name];

            if (value == null)
                return (T)value;
            if (typeof(T) == typeof(int) && value.GetType() == typeof(long))
                return (T)(object)Convert.ToInt32(value);
            if (typeof(T) == typeof(float) && value.GetType() == typeof(double))
                return (T)(object)Convert.ToSingle(value);

            if (value.GetType() != typeof(T))
                throw new Exception($"Module value ({value.GetType()}) isnt of the correct type ({typeof(T)})");

            return (T)value;
        }
        public bool HasValue(string name)
        {
            return PropertiesData.ContainsKey(name);
        }

        public virtual void GetModuleProperties()
        {

        }
        public virtual void GetLinkNodes()
        {

        }

        public abstract void ProcessModule();
        
        public virtual void SetupVisual(ModHelperPanel panel)
        {
            
        }

        public void Init()
        {
            if (HasInited) return;
            HasInited = true;
            
            GetLinkNodes();
            GetModuleProperties();
        }
    }
}

using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using FactoryCore.API.ModuleValues;
using Harmony;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using UnityEngine;
using UnityEngine.UI;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace FactoryCore.API
{
    public abstract class Module
    {
        public virtual bool IsRemovable { get; } = true;
        public abstract string Name { get; }

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
        public bool HasInited = false;

        [JsonIgnore]
        public Vector2 Position { get => new Vector2(XPosition, YPosition); set { XPosition = value.x; YPosition = value.y; } }

        [JsonIgnore]
        List<ModuleProperty> Properties = new List<ModuleProperty>();
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

            return (T)linkedOutput.Outputs.First(a => a.Id == input.OutputGuid).OutputFunc.Invoke();
        }
        public List<Module> GetOutputsModules(string name)
        {
            var modules = new List<Module>();

            var output = Outputs.FirstOrDefault(a => a.Name == name);
            if (output == null)
                throw new Exception("Output is null");

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
        public virtual void ProcessModule()
        {

        }
        public virtual void SetupVisual(ModHelperPanel panel, ModuleHolder holder)
        {
            var content = panel.AddPanel(new Info("Content", 0, 0, 650, 800, new Vector2(0.5f, 0.5f)));
            content.AddComponent<VerticalLayoutGroup>().padding = new RectOffset() { top = 35, bottom = 35 };
            ContentSizeFitter fitter = content.FitContent(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize);
            
            foreach (var value in Properties)
            {
                value.GetVisual(content);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.RectTransform);

            AddOutputVisuals(content, new Vector2(1, 0.5f), Outputs);
            AddInputVisuals(content, new Vector2(0, 0.5f), Inputs);
        }

        protected void AddOutputVisuals(ModHelperPanel content, Vector2 anchor, List<ModuleOutput> links)
        {
            var nodes = content.AddPanel(new Info("Nodes", 0, 0, 100, content.RectTransform.sizeDelta.y, anchor));

            nodes.RectTransform.sizeDelta = new Vector2(100, content.RectTransform.sizeDelta.y);
            nodes.LayoutElement.ignoreLayout = true;
            var layout = nodes.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 50;
            layout.childControlHeight = false;
            layout.childControlWidth = false;

            foreach (var link in links)
            {
                CreateNodeImage(nodes, link);
            }
        }
        protected void AddInputVisuals(ModHelperPanel content, Vector2 anchor, List<ModuleInput> links)
        {
            var nodes = content.AddPanel(new Info("Nodes", 0, 0, 100, content.RectTransform.sizeDelta.y, anchor));

            nodes.RectTransform.sizeDelta = new Vector2(100, content.RectTransform.sizeDelta.y);
            nodes.LayoutElement.ignoreLayout = true;
            var layout = nodes.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 50;
            layout.childControlHeight = false;
            layout.childControlWidth = false;

            foreach (var link in links)
            {
                CreateNodeImage(nodes, link);
            }
        }
        public void CreateNodeImage(ModHelperPanel root, ModuleInput link)
        {
            var image = root.AddImage(new Info("Node", 0, 0, 100, 100), Assets.ConnectionNode);
            image.AddText(new Info("Text", -100, 0, 200, 40, new Vector2(0, 0.5f)), link.Name, 40, Il2CppTMPro.TextAlignmentOptions.MidlineRight).EnableAutoSizing();
            image.Image.color = ValueColors.ColorByLinkType.ContainsKey(link.Type) ? ValueColors.ColorByLinkType[link.Type] : Color.white; ;
            var moduleLink = image.AddComponent<ModuleInputHolder>();
            moduleLink.link = link;
            ModuleInputHolder.GuidToHolder.Add(link.Id, moduleLink);
        }
        public void CreateNodeImage(ModHelperPanel root, ModuleOutput link)
        {
            var image = root.AddImage(new Info("Node", 0, 0, 100, 100), Assets.ConnectionNode);
            image.AddText(new Info("Text", 100, 0, 200, 40, new Vector2(1, 0.5f)), link.Name, 40, Il2CppTMPro.TextAlignmentOptions.MidlineLeft).EnableAutoSizing();
            image.Image.color = ValueColors.ColorByLinkType.ContainsKey(link.Type) ? ValueColors.ColorByLinkType[link.Type] : Color.white;
            var moduleLink = image.AddComponent<ModuleOutputHolder>();
            moduleLink.link = link;
            ModuleOutputHolder.GuidToHolder.Add(link.Id, moduleLink);
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

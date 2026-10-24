using Debug = UnityEngine.Debug;

namespace FactoryCore.UI;

[RegisterTypeInIl2Cpp(false)]
public class ModuleRootMonoBehavior : MonoBehaviour
{
    ModHelperPanel propertiesContainer;
    ModHelperText nameText;
    ModHelperButton deleteButton;
    ModHelperButton infoButton;
    ModHelperPanel contentPanel;

    internal Module module;

    private Stack<ModuleOutputHolder> outputHolders;
    private Stack<ModuleInputHolder> inputHolders;
    
    private static Dictionary<Type, ModHelperPanel> propertyTemplates = new Dictionary<Type, ModHelperPanel>();
    
    public void Update()
    {
        module?.Position = transform.localPosition;
    }
    public void SetModule(Module module)
    {
        this.module = module;
        
        propertiesContainer = transform.Find("PropertiesContainer").GetComponent<ModHelperPanel>();
        contentPanel = propertiesContainer.transform.Find("Content").GetComponent<ModHelperPanel>();
        
        var dragBar = transform.Find("DragBar").transform;
        dragBar.gameObject.AddComponent<DraggableTab>().Init(gameObject);
        
        nameText = dragBar.Find("Text").GetComponent<ModHelperText>();
        deleteButton = dragBar.Find("Delete").GetComponent<ModHelperButton>();
        infoButton = dragBar.Find("Info").GetComponent<ModHelperButton>();
        
        transform.localPosition = new Vector3(module.XPosition, module.YPosition, 0);
        
        module.SetupVisual(propertiesContainer);

        foreach (var value in module.Properties)
        {
            if (!propertyTemplates.TryGetValue(value.GetType(), out ModHelperPanel template) || template == null)
            {
                template = propertyTemplates[value.GetType()] = value.CreateTemplate();
                DontDestroyOnLoad(template.gameObject);
            }
            var property = Instantiate(template);
            value.SetupProperty(property);
            property.SetInfo(template.initialInfo);
            contentPanel.Add(property);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPanel.RectTransform);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(contentPanel.RectTransform);

        outputHolders = new Stack<ModuleOutputHolder>();
        inputHolders = new  Stack<ModuleInputHolder>();
        
        AddOutputVisuals(contentPanel, new Vector2(1, 0.5f), module.Outputs);
        AddInputVisuals(contentPanel, new Vector2(0, 0.5f), module.Inputs);
        
        nameText.SetText(module.Name);

        if (module.IsRemovable)
        {
            deleteButton.Button.onClick.SetListener(() =>
            {
                EditorUI.Template.modules.Remove(module);
                Delete();
            });
        }
        else
            deleteButton.gameObject.SetActive(false);

        if (module.Description != string.Empty)
        {
            infoButton.Button.onClick.SetListener(() =>
            {
                PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.inGameCenter, "Info",
                    module.Description, null, "Ok", null, null, Popup.TransitionAnim.Scale));
            });
        }
        else
            infoButton.gameObject.SetActive(false);
    }
    void AddOutputVisuals(ModHelperPanel content, Vector2 anchor, List<ModuleOutput> links)
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
            CreateOutputHolder(nodes, link);
        }
    }
    void AddInputVisuals(ModHelperPanel content, Vector2 anchor, List<ModuleInput> links)
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
            CreateInputHolder(nodes, link);
        }
    }
    public void CreateInputHolder(ModHelperPanel root, ModuleInput link)
    {
        var image = root.AddImage(new Info("Node", 0, 0, 100, 100), Assets.ConnectionNode);
        image.AddText(new Info("Text", -100, 0, 200, 40, new Vector2(0, 0.5f)), link.Name, 40, Il2CppTMPro.TextAlignmentOptions.MidlineRight).EnableAutoSizing();
        image.Image.color = EditorUI.GetColorForNode(link.Type); ;
        var moduleLink = image.AddComponent<ModuleInputHolder>();
        moduleLink.link = link;
        ModuleInputHolder.GuidToHolder.Add(link.Id, moduleLink);
        inputHolders.Push(moduleLink);
    }
    public void CreateOutputHolder(ModHelperPanel root, ModuleOutput link)
    {
        var image = root.AddImage(new Info("Node", 0, 0, 100, 100), Assets.ConnectionNode);
        image.AddText(new Info("Text", 100, 0, 200, 40, new Vector2(1, 0.5f)), link.Name, 40, Il2CppTMPro.TextAlignmentOptions.MidlineLeft).EnableAutoSizing();
        image.Image.color = EditorUI.GetColorForNode(link.Type);
        var moduleLink = image.AddComponent<ModuleOutputHolder>();
        moduleLink.link = link;
        ModuleOutputHolder.GuidToHolder.Add(link.Id, moduleLink);
        outputHolders.Push(moduleLink);
    }
    public void Delete()
    {
        while (outputHolders.Count > 0)
        {
            outputHolders.Pop().RemoveConnections();
        }
        while (inputHolders.Count > 0)
        {
            inputHolders.Pop().RemoveConnections();
        }
        gameObject.DestroyAllChildren();
        Destroy(gameObject);
    }
    public static ModuleRootMonoBehavior CreateTemplateModule()
    {
        var panel = ModHelperPanel.Create(new Info("ModuleRoot", 0, 0, 0, 0));
        DontDestroyOnLoad(panel.gameObject);
        panel.AddComponent<VerticalLayoutGroup>();
        var moduleRoot = panel.AddComponent<ModuleRootMonoBehavior>();

        moduleRoot.propertiesContainer = panel.AddPanel(new Info("PropertiesContainer", 1000, 700, new Vector2(0.5f, 0.5f)),
                    VanillaSprites.MainBGPanelBlue, RectTransform.Axis.Vertical, 0, 0);
        moduleRoot.propertiesContainer.AddLayoutElement().minHeight = 200;
        moduleRoot.propertiesContainer.FitContent(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize);
        
        moduleRoot.contentPanel = moduleRoot.propertiesContainer.AddPanel(new Info("Content", 0, 0, 650, 800, new Vector2(0.5f, 0.5f)));
        var vertLayout = moduleRoot.contentPanel.AddComponent<VerticalLayoutGroup>(); 
        vertLayout.childForceExpandHeight = vertLayout.childForceExpandWidth = false;
        vertLayout.padding = new RectOffset(){ top = 35, bottom = 35};
        moduleRoot.contentPanel.FitContent(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize);
        
        var dragBar = panel.AddPanel(new Info("DragBar", 0, 0, 1000, 150, new Vector2(0.5f, 0.5f)), VanillaSprites.MainBGPanelBlue);
            
        moduleRoot.nameText = dragBar.AddText(new Info("Text", 400, 0, 700, 100, new Vector2(0f, 0.5f)), "Empty Module", 60, Il2CppTMPro.TextAlignmentOptions.MidlineLeft);
        moduleRoot.nameText.EnableAutoSizing();
        moduleRoot.nameText.AddLayoutElement().ignoreLayout = true;

        var layoutGroup = dragBar.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.MiddleRight;
        layoutGroup.SetPadding(25);
        layoutGroup.spacing = 25;
        layoutGroup.childForceExpandHeight = layoutGroup.childForceExpandWidth = false;
        
        moduleRoot.deleteButton = dragBar.AddButton(new Info("Delete", 100), VanillaSprites.AddRemoveBtn, new Action(() =>
        {
            
        }));
        
        moduleRoot.infoButton = dragBar.AddButton(new Info("Info", 100), VanillaSprites.InfoBtn2, new Action(() =>
        {
            PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.inGameCenter, "Info", "This is description text.", null, "Ok", null, null, Popup.TransitionAnim.Scale));
        }));
        
        dragBar.transform.SetSiblingIndex(0);

        return moduleRoot;
    }
}
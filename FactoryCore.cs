

[assembly: MelonInfo(typeof(FactoryCore.FactoryCore), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace FactoryCore;

public class FactoryCore : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ModHelper.Msg<FactoryCore>("FactoryCore loaded!");
    }
    public override void OnApplicationQuit()
    {
        EditorUI.Instance?.SaveTemplate();
    }
}
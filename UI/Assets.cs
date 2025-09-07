global using FactoryCore.UI;
using BTD_Mod_Helper.Api;
using UnityEngine;
namespace FactoryCore.UI
{
    internal static class Assets
    {
        internal static Sprite ConnectionNode => ModContent.GetSprite<FactoryCore>("ConnectionNode");

        internal static Sprite OffBtn => ModContent.GetSprite<FactoryCore>("OffBtn");

        internal static Sprite OnBtn => ModContent.GetSprite<FactoryCore>("OnBtn");
    }
}

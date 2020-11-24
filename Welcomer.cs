﻿using InfinityScript;

namespace ExtensionScript
{
    class Welcomer
    {
        /// <summary>function <c>TellPlayer</c> Prints on the screen of the target a fancy message.</summary>
        public void TellPlayer(Entity player, string text = "^5Welcome")
        {
            HudElem welcomer = HudElem.CreateFontString(player, HudElem.Fonts.Objective, 1.8f);
            welcomer.SetPoint("CENTER", "CENTER", 0, -110);
            welcomer.SetText(text);
            welcomer.GlowAlpha = 1f;
            welcomer.SetPulseFX(100, 0x1b58, 600);
            welcomer.HideWhenInMenu = true;
        }
    }
}
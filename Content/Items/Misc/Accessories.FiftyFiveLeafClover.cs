﻿using Microsoft.Xna.Framework;
using StarlightRiver.Content.Items.BaseTypes;
using StarlightRiver.Core;
using StarlightRiver.Helpers;
using Terraria;
using Terraria.DataStructures;

namespace StarlightRiver.Content.Items.Misc
{
	public class FiftyFiveLeafClover : SmartAccessory
    {
        public override string Texture => AssetDirectory.MiscItem + Name;

        public FiftyFiveLeafClover() : base("Fifty Five Leaf Clover", "Critical strike chance increases up to 20% over 10 seconds\nEffect resets upon taking damage") { }

        public override void Load()
        {
            StarlightPlayer.PreHurtEvent += PreHurtAccessory;
            StarlightPlayer.ResetEffectsEvent += ResetEffectsAccessory;
        }

        public override void SafeUpdateEquip(Player Player)
        {
            StarlightPlayer starlightPlayer = Player.GetModPlayer<StarlightPlayer>();

            starlightPlayer.FiftyFiveLeafClover = (int)MathHelper.Clamp(starlightPlayer.FiftyFiveLeafClover + 1, 0, 600);

            float progress = starlightPlayer.FiftyFiveLeafClover / 600f;

            Player.BoostAllDamage(0, (int)(progress * 20));
        }

        private void ResetEffectsAccessory(StarlightPlayer starlightPlayer)
        {
            if (!Equipped(starlightPlayer.Player))
            {
                starlightPlayer.FiftyFiveLeafClover = 0;
            }
        }

        private bool PreHurtAccessory(Player Player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            Player.GetModPlayer<StarlightPlayer>().FiftyFiveLeafClover = 0;

            return true;
        }
    }
}
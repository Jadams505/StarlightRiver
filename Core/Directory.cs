using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using StarlightRiver.Codex;
using StarlightRiver.Core;
using StarlightRiver.Core.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;


namespace StarlightRiver.Core
{
    public static class Directory
    {
        public const string Assets =                "StarlightRiver/Assets/";

        public const string Invisible =             Assets + "Invisible";
        public const string Debug =                 Assets + "MarioCumming";
        public const string Symbol =                Assets + "Symbol";

        public const string Misc =                  Assets + "Misc/";

        public const string RiftCrafting =          Assets + "RiftCrafting/";

        public const string Dust =                  Assets + "Dusts/";

        public const string BrewingItem =           Assets + "Items/Brewing/";

        public const string EbonyIvoryItem =        Assets + "Items/EbonyIvory/";

        public const string AluminumItem =          Assets + "Items/AstralMeteor/";
        public const string CaveTempleItem =        Assets + "Items/UndergroundTemple/";

        public const string OvergrowTile =          Assets + "Tiles/Overgrow/";
        public const string OvergrowItem =          Assets + "Items/Overgrow/";
        public const string OvergrowBoss =          Assets + "Bosses/OvergrowBoss/";

        public const string VitricTile =            Assets + "Tiles/Vitric/";
        public const string VitricItem =            Assets + "Items/Vitric/";
        public const string GlassMiniboss =         Assets + "Bosses/GlassMiniboss/";
        public const string GlassBoss =             Assets + "Bosses/GlassBoss/";

        public const string PermafrostTile =        Assets + "Tiles/Permafrost/";
        public const string PermafrostItem =        Assets + "Items/Permafrost/";
        public const string SquidBoss =             Assets + "Bosses/SquidBoss/";

        public const string SlimeItem =             Assets + "Items/Slime/";
        public const string StarwoodItem =          Assets + "Items/Starwood/";
        public const string PalestoneItem =         Assets + "Items/Palestone/";
        public const string IvyItem =               Assets + "Items/ForestIvy/";
        public const string MiscItem =              Assets + "Items/Misc/";

        public const string TownNPC =               Assets + "NPCs/Town/";
    }
}
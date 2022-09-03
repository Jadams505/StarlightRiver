﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarlightRiver.Content.Abilities;
using StarlightRiver.Content.Abilities.Faewhip;
using StarlightRiver.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace StarlightRiver.Content.Tiles.Overgrow
{
	internal class NoxiousNode : DummyTile
	{
		public override string Texture => AssetDirectory.Debug;

		public override int DummyType => ModContent.ProjectileType<NoxiousNodeDummy>();
	}

	internal class NoxiousNodeDummy : Dummy, IFaeWhippable
	{
		public NoxiousNodeDummy() : base(ModContent.TileType<NoxiousNode>(), 8, 8) { }

		public override void Update()
		{
			if (Main.rand.NextBool(4))
			{
				var pos = Projectile.Center + Main.rand.NextVector2Circular(128, 128);
				var vel = Vector2.UnitY.RotatedByRandom(6.28f) * -Main.rand.NextFloat(1);
				Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.Cinder>(), vel, 0, new Color(20, 230, 255), 0.5f);
			}

			var gaspos = Projectile.Center + Main.rand.NextVector2Circular(100, 100);
			Dust.NewDustPerfect(gaspos, ModContent.DustType<Dusts.NoxiousGas>(), Vector2.Zero, 0, new Color(20, 230, 255) * 0.1f, 1.0f);

			base.Update();
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			return Helpers.Helper.CheckCircularCollision(Projectile.Center, 128, targetHitbox);
		}

		public override void Collision(Player Player)
		{
			Player.AddBuff(Terraria.ID.BuffID.Venom, 12, false);
		}

		public override void PostDraw(Color lightColor)
		{
			var tex = ModContent.Request<Texture2D>("StarlightRiver/Assets/Keys/GlowAlpha").Value;
			Vector2 pos = Projectile.Center - Main.screenPosition;
			var color = new Color(20, 230, 255) * (0.15f + 0.05f * (float)Math.Sin(Main.GameUpdateCount * 0.02f));
			var color2 = new Color(20, 255, 220) * (0.15f + 0.05f * (float)Math.Sin(Main.GameUpdateCount * 0.05f));

			color.A = 0;

			Main.spriteBatch.Draw(tex, pos, null, color, 0, tex.Size() / 2, 5, 0, 0);
			Main.spriteBatch.Draw(tex, pos, null, color2, 0, tex.Size() / 2, 5, 0, 0);

			var flowerTex = ModContent.Request<Texture2D>(AssetDirectory.OvergrowTile + "NoxiousNode").Value;
			Main.spriteBatch.Draw(flowerTex, pos, null, lightColor, 0, flowerTex.Size() / 2, 1, 0, 0);
		}

		public void UpdateWhileWhipped(Whip whip)
		{
			if (Vector2.Distance(Main.MouseWorld, Projectile.Center) < 100)
				whip.tipsPosition = Projectile.Center;
			else
			{
				WorldGen.KillTile(ParentX, ParentY);
				Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 10;
			}
		}

		public bool DetachCondition()
		{
			return !Projectile.active;
		}

		public bool IsWhipColliding(Vector2 whipPosition)
		{
			return Vector2.Distance(whipPosition, Projectile.Center) < 21;
		}
	}

	internal class NoxiousNodeItem : QuickTileItem
	{
		public override string Texture => AssetDirectory.Debug;

		public NoxiousNodeItem() : base("Noxious Node", "Debug item", "NoxiousNode") { }
	}
}

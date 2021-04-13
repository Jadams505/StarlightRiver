﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarlightRiver.Core;
using StarlightRiver.Physics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using static Terraria.ModLoader.ModContent;

using StarlightRiver.Core;
using StarlightRiver.Helpers;

namespace StarlightRiver.Content.CustomHooks
{
    class VitricBackground : HookGroup
    {
        //Its just drawing, but its alot of drawing, and some questionable vanilla RenderBlack replication stuff that likely needs to be removed at some point.
        public override SafetyLevel Safety => SafetyLevel.Questionable;

        public override void Load()
        {
            ForegroundParticles = new ParticleSystem("StarlightRiver/Assets/GUI/LightBig", UpdateForegroundBody, 3);
            BackgroundParticles = new ParticleSystem("StarlightRiver/Assets/GUI/Holy", UpdateBackgroundBody, 1);

            //crystalEffect = Main.dedServ ? null : Filters.Scene["Crystal"].GetShader().Shader;

            vitricBackgroundBannerTarget = Main.dedServ ? null : new RenderTarget2D(Main.instance.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            BackgroundBanner = Main.dedServ ? null : new VerletChainInstance(true)
            {
                segmentCount = 35,
                segmentDistance = 24,
                constraintRepetitions = 2,
                drag = 2.8f,
                forceGravity = new Vector2(0f, 0.20f),
                gravityStrengthMult = 1f
            };

            On.Terraria.Main.DrawBackgroundBlackFill += DrawVitricBackground;
            Main.OnPreDraw += BannerTarget;
        }

        public override void Unload()
        {
            crystalEffect = null;

            ForegroundParticles = null;
            BackgroundParticles = null;

            vitricBackgroundBannerTarget = null;
            BackgroundBanner = null;

            Main.OnPreDraw -= BannerTarget;
        }

        private static Effect crystalEffect;

        internal static ParticleSystem ForegroundParticles;
        internal static ParticleSystem BackgroundParticles;

        static RenderTarget2D vitricBackgroundBannerTarget;

        static VerletChainInstance BackgroundBanner;

        private void DrawBanner(SpriteBatch spriteBatch, Vector2 pos)
        {
            bannerTimer += 0.01f;

            BackgroundBanner.UpdateChain(Vector2.Zero);
            if (BackgroundBanner.init) BackgroundBanner.IterateRope(WindForce);

            spriteBatch.Draw(GetTexture("StarlightRiver/Assets/Backgrounds/GlassPin"), pos + new Vector2(-200, -300), null, new Color(100, 120, 150), 0, Vector2.Zero, 1, 0, 0);
            spriteBatch.Draw(vitricBackgroundBannerTarget, pos + new Vector2(-30, -240), null, Color.White, 0, Vector2.Zero, 2, 0, 0);
        }

        float bannerTimer = 0;

        private void WindForce(int index)//wind
        {
            float sin = (float)Math.Sin(bannerTimer - index / 3f);

            float cos = (float)Math.Cos(bannerTimer);
            float sin2 = (float)Math.Sin(bannerTimer + cos);

            Vector2 pos = new Vector2(BackgroundBanner.ropeSegments[index].posNow.X + 1 + sin2 * 1.2f, BackgroundBanner.ropeSegments[index].posNow.Y + sin * 1.8f);

            Color color = new Color(95, 20, 75).MultiplyRGB(Color.White * (1 - sin * 0.2f));

            BackgroundBanner.ropeSegments[index].posNow = pos;
            BackgroundBanner.ropeSegments[index].color = color;
        }

        private void BannerTarget(GameTime obj)
        {
            var graphics = Main.graphics.GraphicsDevice;
            graphics.SetRenderTarget(vitricBackgroundBannerTarget);
            graphics.Clear(Color.Transparent);

            BackgroundBanner?.DrawStrip(Main.screenPosition + new Vector2(0, 50));

            graphics.SetRenderTarget(null);
        }

        private static void UpdateForegroundBody(Particle particle)
        {
            particle.Timer--;
            particle.StoredPosition += particle.Velocity;
            particle.Position.X = particle.StoredPosition.X - Main.screenPosition.X + GetParallaxOffset(particle.StoredPosition.X, 0.25f) + (float)Math.Sin(particle.Timer / 400f * 6.28f) * 20;
            particle.Position.Y = particle.StoredPosition.Y - Main.screenPosition.Y + GetParallaxOffsetY(particle.StoredPosition.Y, 0.1f);

            particle.Color = new Color(130, 170, 255) * particle.Scale;
            particle.Scale = (particle.Timer / 1800f) * 2;
            particle.Rotation += 0.015f;
        }

        private static void UpdateBackgroundBody(Particle particle)
        {
            particle.Timer--;
            particle.StoredPosition += particle.Velocity;
            var randTime = (particle.GetHashCode() % 50) + 100f;
            particle.Position.X = particle.StoredPosition.X - Main.screenPosition.X + GetParallaxOffset(particle.StoredPosition.X, 0.5f) + (float)Math.Sin(particle.Timer / randTime * 6.28f) * 6;
            particle.Position.Y = particle.StoredPosition.Y - Main.screenPosition.Y + GetParallaxOffsetY(particle.StoredPosition.Y, 0.2f);
            particle.Color = Color.Lerp(Color.Red, new Color(255, 255, 200), particle.Timer / 2400f);
            particle.Scale = (particle.Timer / 2400f);
            particle.Rotation += 0.02f;
        }

        private void DrawVitricBackground(On.Terraria.Main.orig_DrawBackgroundBlackFill orig, Main self)
        {
            orig(self);

            if (Main.gameMenu || Main.dedServ) return;

            Player player = null;
            if (Main.playerLoaded) { player = Main.LocalPlayer; }

            if (player != null && StarlightWorld.VitricBiome.Intersects(new Rectangle((int)Main.screenPosition.X / 16, (int)Main.screenPosition.Y / 16, Main.screenWidth / 16, Main.screenHeight / 16)))
            {
                Vector2 basepoint = (StarlightWorld.VitricBiome != null) ? StarlightWorld.VitricBiome.TopLeft() * 16 + new Vector2(-2000, 0) : Vector2.Zero;

                DrawLayer(basepoint, GetTexture("StarlightRiver/Assets/Backgrounds/Glass5"), 0, 300); //the background

                DrawLayer(basepoint, GetTexture("StarlightRiver/Assets/Backgrounds/Glass1"), 6, 170, new Color(150, 175, 190)); //the back sand
                DrawLayer(basepoint, GetTexture("StarlightRiver/Assets/Backgrounds/Glass1"), 6.5f, 400, new Color(120, 150, 170), true); //the back sand on top

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(default, default, SamplerState.PointClamp, default, default, default, Main.GameViewMatrix.ZoomMatrix);

                float x = basepoint.X + GetParallaxOffset(basepoint.X, 0.6f) - Main.screenPosition.X;
                float y = basepoint.Y + GetParallaxOffsetY(basepoint.Y, 0.2f) - Main.screenPosition.Y;
                DrawBanner(Main.spriteBatch, new Vector2(x, y) + new Vector2(1200, 1100));

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

                for (int k = 4; k >= 0; k--)
                {
                    if(k == 3)
                        BackgroundParticles.DrawParticles(Main.spriteBatch);

                    int off = 140 + (340 - k * 110);
                    if (k == 0) off -= 100;
                    if (k == 1) off -= 25;
                    if (k == 4) off = 100;

                    DrawLayer(basepoint, GetTexture("StarlightRiver/Assets/Backgrounds/Glass" + k), k + 1, off, default, false); //the crystal layers and front sand

                    //if (k == 0) DrawLayer(basepoint, GetTexture("StarlightRiver/Assets/Backgrounds/Glass1"), 0.5f, 100, new Color(180, 220, 235), true); //the sand on top
                    if (k == 2) ForegroundParticles.DrawParticles(Main.spriteBatch);
                }

                int screenCenterX = (int)(Main.screenPosition.X + Main.screenWidth / 2);
                for (int k = (int)(screenCenterX - basepoint.X) - (int)(Main.screenWidth * 1.5f); k <= (int)(screenCenterX - basepoint.X) + (int)(Main.screenWidth * 1.5f); k += 30)
                {
                    Vector2 spawnPos = basepoint + new Vector2(2000 + Main.rand.Next(8000), 1800);
                    if (Main.rand.Next(550) == 0)
                        BackgroundParticles.AddParticle(new Particle(new Vector2(0, basepoint.Y + 1600), new Vector2(0, Main.rand.NextFloat(-1.3f, -0.3f)), 0, 0, Color.White, 2400, spawnPos));

                    if (Main.rand.Next(2000) == 0)
                        ForegroundParticles.AddParticle(new Particle(new Vector2(0, basepoint.Y + 1600), new Vector2(0, Main.rand.NextFloat(-1.6f, -0.6f)), 0, 0, Color.White, 1800, spawnPos));
                }

                Main.spriteBatch.End();
                DrawTilingBackground();
                Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

                DrawBlack();
            }
        }

        private void DrawTilingBackground()
        {
            Texture2D tex = GetTexture("StarlightRiver/Assets/Backgrounds/VitricSand");
            Texture2D texBot = GetTexture("StarlightRiver/Assets/Backgrounds/VitricSandBottom");
            Texture2D texTop = GetTexture("StarlightRiver/Assets/Backgrounds/VitricSandTop");
            Texture2D texLeft = GetTexture("StarlightRiver/Assets/Backgrounds/VitricSandLeft");
            Texture2D texRight = GetTexture("StarlightRiver/Assets/Backgrounds/VitricSandRight");

            Rectangle blacklist = new Rectangle(StarlightWorld.VitricBiome.X, StarlightWorld.VitricBiome.Y - 2, StarlightWorld.VitricBiome.Width, StarlightWorld.VitricBiome.Height);

            for (int x = -tex.Width; x <= Main.screenWidth + tex.Width; x += tex.Width)
                for (int y = -tex.Height; y <= Main.screenHeight + tex.Height; y += tex.Height)
                {
                    Vector2 pos = new Vector2(x, y) - new Vector2(Main.screenPosition.X % tex.Width, Main.screenPosition.Y % tex.Height);
                    Texture2D drawtex = null;

                    if (CheckBackground(pos + new Vector2(0, -tex.Height), tex.Size(), blacklist, true))
                        drawtex = texBot;
                    else if (CheckBackground(pos + new Vector2(0, tex.Height), tex.Size(), blacklist, true))
                        drawtex = texTop;
                    else if (CheckBackground(pos + new Vector2(tex.Width, 0), tex.Size(), blacklist, true))
                        drawtex = texLeft;
                    else if (CheckBackground(pos + new Vector2(-tex.Width, 0), tex.Size(), blacklist, true))
                        drawtex = texRight;
                    else
                        drawtex = tex;

                    if (CheckBackground(pos, drawtex.Size(), blacklist))
                        Helper.DrawWithLighting(pos, drawtex);
                }
        }

        private bool CheckBackground(Vector2 pos, Vector2 size, Rectangle biome, bool dontCheckScreen = false)
        {
            if (dontCheckScreen || Helper.OnScreen(new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y)))
            {
                if (!Main.BackgroundEnabled) return true;
                else if (!biome.Contains(((pos + Main.screenPosition) / 16).ToPoint()) || !biome.Contains(((pos + size + Main.screenPosition) / 16).ToPoint())) return true;
            }
            return false;
        }

        private static void DrawBlack()
        {
            for (int i = -2 + (int)(Main.screenPosition.X) / 16; i <= 2 + (int)(Main.screenPosition.X + Main.screenWidth) / 16; i++)
                for (int j = -2 + (int)(Main.screenPosition.Y) / 16; j <= 2 + (int)(Main.screenPosition.Y + Main.screenHeight) / 16; j++)
                    if (Lighting.Brightness(i, j) == 0 || ((Main.tile[i, j].active() && Main.tile[i, j].collisionType == 1) || Main.tile[i, j].wall != 0))
                    {
                        Color color = Color.Black * (1 - Lighting.Brightness(i, j) * 2);
                        Main.spriteBatch.Draw(Main.blackTileTexture, new Vector2(i * 16, j * 16) - Main.screenPosition, color);
                    }
        }

        private static void DrawLayer(Vector2 basepoint, Texture2D texture, float parallax, int offY = 0, Color color = default, bool flip = false)
        {
            if (color == default) color = Color.White;

            for (int k = 0; k <= 5; k++)
            {
                float x = basepoint.X + (k * 739 * 4) + GetParallaxOffset(basepoint.X, parallax * 0.1f) - (int)Main.screenPosition.X;
                float y = basepoint.Y + offY - (int)Main.screenPosition.Y + GetParallaxOffsetY(basepoint.Y + StarlightWorld.VitricBiome.Height * 8, parallax * 0.04f);

                if (x > -texture.Width && x < Main.screenWidth + 30)
                    Main.spriteBatch.Draw(texture, new Vector2(x, y), new Rectangle(0, 0, 2956, 1528), color, 0f, Vector2.Zero, 1f, flip ? SpriteEffects.FlipVertically : 0, 0);
            }
        }

        private static int GetParallaxOffset(float startpoint, float factor)
        {
            float vanillaParallax = 1 - (Main.caveParallax - 0.8f) / 0.2f;
            return (int)((Main.screenPosition.X + Main.screenWidth / 2 - startpoint) * factor * vanillaParallax);
        }

        private static int GetParallaxOffsetY(float startpoint, float factor) => (int)((Main.screenPosition.Y + Main.screenHeight / 2 - startpoint) * factor);
    }
}


﻿using System;
using Microsoft.Xna.Framework.Graphics;
using Quaver.Shared.Assets;
using Quaver.Shared.Helpers;
using Quaver.Shared.Screens.Menu.UI.Jukebox;
using Wobble.Assets;
using Wobble.Graphics;
using Wobble.Graphics.Sprites;
using Wobble.Graphics.Sprites.Text;
using Wobble.Graphics.UI.Buttons;
using Wobble.Managers;

namespace Quaver.Shared.Screens.Edit.UI.AutoMods
{
    public class EditorAutoModHeader : ImageButton
    {
        private EditorAutoModPanel Panel { get; }

        private Sprite Gear { get; set; }

        private SpriteTextPlus Text { get; set; }

        private IconButton CloseButton { get; set; }

        public EditorAutoModHeader(EditorAutoModPanel panel) : base(UserInterface.BlankBox)
        {
            Panel = panel;

            Image = UserInterface.AutoModPanelHeader;
            Size = new ScalableVector2(Panel.Width, Image.Height);

            CreateGearIcon();
            CreateText();
            CreateCloseButton();
            Depth = -1;
        }

        private void CreateGearIcon() => Gear = new Sprite
        {
            Parent = this,
            Alignment = Alignment.MidLeft,
            X = Height / 2,
            Size = new ScalableVector2(Height / 2, Height / 2),
            Image = UserInterface.AutoModHeaderGear
        };

        private void CreateText()
        {
            Text = new SpriteTextPlus(FontManager.GetWobbleFont(Fonts.LatoBlack), "AutoMod", 22)
            {
                Parent = this,
                Alignment = Alignment.MidLeft,
                X = Gear.X + Gear.Width + 10
            };
        }

        private void CreateCloseButton()
        {
            CloseButton = new IconButton(FontAwesome.Get(FontAwesomeIcon.fa_times))
            {
                Parent = this,
                Alignment = Alignment.MidRight,
                X = -Gear.X,
                Size = new ScalableVector2(Height / 2, Height / 2),
                Tint = ColorHelper.HexToColor("#eeeeee")
            };
        }
    }
}
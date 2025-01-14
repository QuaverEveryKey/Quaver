/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using Microsoft.Xna.Framework;
using Quaver.API.Helpers;
using Quaver.Shared.Audio;
using Quaver.Shared.Config;
using Quaver.Shared.Graphics;
using Quaver.Shared.Modifiers;
using Quaver.Shared.Skinning;
using Wobble.Graphics;
using Wobble.Graphics.UI;

namespace Quaver.Shared.Screens.Gameplay.UI
{
    public class SongTimeProgressBar : ProgressBar
    {
        /// <summary>
        ///     Reference to the parent gameplay screen.
        /// </summary>
        private GameplayScreen Screen { get; }

        /// <summary>
        ///     The display for the current time.
        /// </summary>
        public NumberDisplay CurrentTime { get; }

        /// <summary>
        ///     The display for the time left.
        /// </summary>
        public GameplayNumberDisplay TimeLeft { get; }

        /// <summary>
        ///    The time the number display was last updated.
        /// </summary>
        private double TimeLastProgressChange { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="size"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="defaultValue"></param>
        /// <param name="inactiveColor"></param>
        /// <param name="activeColor"></param>
        public SongTimeProgressBar(GameplayScreen screen, Vector2 size, double minValue, double maxValue, double defaultValue, Color inactiveColor, Color activeColor)
            : base(size, minValue, maxValue, defaultValue, inactiveColor, activeColor)
        {
            Screen = screen;

            var skin = SkinManager.Skin.Keys[screen.Map.KeyCount];
            if (ConfigManager.DisplaySongTimeProgressNumbers.Value)
            {
                CurrentTime = new GameplayNumberDisplay(NumberDisplayType.SongTime, "00:00",
                    new Vector2(skin.SongTimeProgressScale / 100f, skin.SongTimeProgressScale / 100f))
                {
                    Parent = this,
                    Alignment = skin.SongTimeProgressPositionAtTop ? Alignment.TopLeft : Alignment.BotLeft,
                    X = 10,
                    Y = skin.SongTimeProgressPositionAtTop ? Height + 5 : -Height - 5
                };

                var startText = (new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds((int)Bindable.MaxValue)).ToString("mm:ss");

                TimeLeft = new GameplayNumberDisplay(NumberDisplayType.SongTime, "-" + startText,
                    new Vector2(skin.SongTimeProgressScale / 100f, skin.SongTimeProgressScale / 100f))
                {
                    Parent = this,
                    Alignment = skin.SongTimeProgressPositionAtTop ? Alignment.TopRight : Alignment.BotRight,
                    X = -10,
                    Y = CurrentTime.Y
                };
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            Visible = ConfigManager.DisplayGameplayOverlay?.Value ?? true;

            Bindable.Value = Screen.Timing.Time / ModHelper.GetRateFromMods(ModManager.Mods);

            // Only update time each second.
            if (Math.Abs(Screen.Timing.Time - TimeLastProgressChange) < 1000)
            {
                base.Update(gameTime);
                return;
            }

            TimeLastProgressChange = Screen.Timing.Time;

            // Set the time of the current time
            if (ConfigManager.DisplaySongTimeProgressNumbers.Value)
            {
                if (Bindable.Value > 0)
                {
                    var currTime = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds((int) Bindable.Value);
                    CurrentTime.Value = currTime.ToString("mm:ss");
                }

                // Set the time of the time left.
                if (Bindable.MaxValue - Bindable.Value >= 0)
                {
                    var timeLeft = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds((int)Bindable.MaxValue - Bindable.Value);

                    // Set the new value.
                    TimeLeft.Value = "-" + timeLeft.ToString("mm:ss");
                }
            }

            base.Update(gameTime);
        }
    }
}

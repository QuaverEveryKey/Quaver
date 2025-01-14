/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quaver.API.Enums;
using Quaver.Shared.Assets;
using Quaver.Shared.Config;
using Quaver.Shared.Skinning;
using Wobble.Graphics;
using Wobble.Graphics.Sprites;

namespace Quaver.Shared.Screens.Gameplay.UI.Counter
{
    /// <inheritdoc />
    /// <summary>
    ///     Displays all the current judgements + KPS
    /// </summary>
    public class JudgementCounter : Container
    {
        /// <summary>
        ///     Reference to the ruleset.
        /// </summary>
        public GameplayScreen Screen { get; }

        /// <summary>
        ///     The list of judgement displays.
        /// </summary>
        private Dictionary<Judgement, JudgementCounterItem> JudgementDisplays { get; }

        /// <summary>
        ///     The background of judgement displays.
        /// </summary>
        private Dictionary<Judgement, Sprite> JudgementDisplaysBackground { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Ctor -
        /// </summary>
        public JudgementCounter(GameplayScreen screen)
        {
            Screen = screen;

            // Create the judgement displays.
            JudgementDisplays = new Dictionary<Judgement, JudgementCounterItem>();
            JudgementDisplaysBackground = new Dictionary<Judgement, Sprite>();

            var skin = SkinManager.Skin.Keys[Screen.Map.KeyCount];
            for (var i = 0; i < Screen.Ruleset.ScoreProcessor.CurrentJudgements.Count; i++)
            {
                var key = (Judgement)i;
                var color = SkinManager.Skin.Keys[Screen.Map.KeyCount].JudgeColors[key];

                if (SkinManager.Skin.JudgementOverlayBackground[key] != UserInterface.BlankBox)
                {
                    JudgementDisplaysBackground[key] = new Sprite()
                    {
                        Alignment = Alignment.MidRight,
                        Parent = this,
                        Image = SkinManager.Skin.JudgementOverlayBackground[key],
                        Alpha = skin.JudgementCounterAlpha,
                        X = skin.JudgementCounterPosX,
                        Y = skin.JudgementCounterPosY,
                        Size = new ScalableVector2(skin.JudgementCounterSize, skin.JudgementCounterSize)
                    };
                }

                // Default it to an inactive color.
                JudgementDisplays[key] = new JudgementCounterItem(this, key, new Color(color.R / 2, color.G / 2, color.B / 2),
                    new Vector2(skin.JudgementCounterSize, skin.JudgementCounterSize))
                {
                    Alignment = Alignment.MidRight,
                    Parent = this,
                    Image = SkinManager.Skin.JudgementOverlay[key],
                    Alpha = skin.JudgementCounterAlpha,
                    X = skin.JudgementCounterPosX,
                    Y = skin.JudgementCounterPosY
                };

                // Normalize the position of the first one so that all the rest will be completely in the middle.
                if (i == 0)
                {
                    Y = Screen.Ruleset.ScoreProcessor.CurrentJudgements.Count * -JudgementDisplays[key].Height / 2f;
                    continue;
                }

                if (skin.JudgementCounterHorizontal)
                    JudgementDisplays[key].X = JudgementDisplays[(Judgement)(i - 1)].X + JudgementDisplays[key].Width + 5 + skin.JudgementCounterPadding;
                else
                    JudgementDisplays[key].Y = JudgementDisplays[(Judgement)(i - 1)].Y + JudgementDisplays[key].Height + 5 + skin.JudgementCounterPadding;

                if (SkinManager.Skin.JudgementOverlayBackground[key] != UserInterface.BlankBox)
                    JudgementDisplaysBackground[key].Position = JudgementDisplays[key].Position;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (ConfigManager.DisplayGameplayOverlay.Value)
                Visible = ConfigManager.DisplayJudgementCounter.Value;
            else
                Visible = false;

            var dt = gameTime.ElapsedGameTime.TotalMilliseconds;

            // Update the judgement counts of each one.
            foreach (var item in JudgementDisplays)
            {
                if (Screen.Ruleset.ScoreProcessor.CurrentJudgements[item.Key] != JudgementDisplays[item.Key].JudgementCount)
                    JudgementDisplays[item.Key].JudgementCount = Screen.Ruleset.ScoreProcessor.CurrentJudgements[item.Key];
            }

            base.Update(gameTime);
        }
    }
}

using Microsoft.Xna.Framework.Media;
using Quaver.API.Helpers;
using Quaver.Shared.Assets;
using Quaver.Shared.Database.Maps;
using Quaver.Shared.Graphics;
using Quaver.Shared.Helpers;
using Quaver.Shared.Modifiers;
using Quaver.Shared.Screens.Selection.UI.FilterPanel.MapInformation.Metadata;
using Wobble.Bindables;
using Wobble.Graphics;
using Wobble.Graphics.Sprites;
using Wobble.Graphics.Sprites.Text;
using Wobble.Managers;

namespace Quaver.Shared.Screens.Selection.UI.FilterPanel.MapInformation
{
    public class FilterPanelMapInfo : ScrollContainer
    {
        /// <summary>
        ///     Displays the title of the song
        /// </summary>
        private SpriteTextPlus ArtistTitle { get; set; }

        /// <summary>
        ///     Displays the difficulty and mods
        /// </summary>
        private SpriteTextPlus DifficultyMods { get; set; }

        /// <summary>
        ///     Displays the game mode of the current map
        /// </summary>
        private FilterMetadataGameMode GameMode { get; set; }

        /// <summary>
        ///     Displays the length of the map
        /// </summary>
        private FilterMetadataLength Length { get; set; }

        /// <summary>
        ///     Displays the BPM of the map
        /// </summary>
        private FilterMetadataBpm Bpm { get; set; }

        /// <summary>
        ///     Displays the LN% of the map
        /// </summary>
        private FilterMetadataLongNotePercentage LongNotePercentage { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public FilterPanelMapInfo() : base(new ScalableVector2(800, 72), new ScalableVector2(800, 72))
        {
            Alpha = 0f;
            InputEnabled = false;

            CreateArtistTitleText();
            CreateDifficultyModsText();
            CreateMetadata();

            UpdateText(MapManager.Selected.Value);

            MapManager.Selected.ValueChanged += OnMapChanged;
            ModManager.ModsChanged += OnModsChanged;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override void Destroy()
        {
            // ReSharper disable once DelegateSubtraction
            MapManager.Selected.ValueChanged -= OnMapChanged;
            ModManager.ModsChanged -= OnModsChanged;

            base.Destroy();
        }

        /// <summary>
        ///     Creates the text that displays the title of the song
        /// </summary>
        private void CreateArtistTitleText()
        {
            ArtistTitle = new SpriteTextPlus(FontManager.GetWobbleFont(Fonts.LatoBlack), "Artist - Title", 20);

            AddContainedDrawable(ArtistTitle);
        }

        /// <summary>
        ///     Creates the text that displays both the difficulty and activated mods
        /// </summary>
        private void CreateDifficultyModsText()
        {
            DifficultyMods = new SpriteTextPlus(FontManager.GetWobbleFont(Fonts.LatoBlack), "[Difficulty] + Mods", 20)
            {
                Y = ArtistTitle.Height + 6,
            };

            AddContainedDrawable(DifficultyMods);
        }

        private void CreateMetadata()
        {
            GameMode = new FilterMetadataGameMode
            {
                Parent = this,
                Y = DifficultyMods.Y + DifficultyMods.Height + 6,
                X = ArtistTitle.X
            };

            Length = new FilterMetadataLength
            {
                Parent = this,
                Y = GameMode.Y,
                X = GameMode.X + GameMode.Width + 35
            };

            Bpm = new FilterMetadataBpm
            {
                Parent = this,
                Y = GameMode.Y,
                X = Length.X + Length.Width + 35
            };

            LongNotePercentage = new FilterMetadataLongNotePercentage()
            {
                Parent = this,
                Y = GameMode.Y,
                X = Bpm.X + Bpm.Width + 35
            };
        }

        /// <summary>
        ///     Updates the data with a new map
        /// </summary>
        /// <param name="map"></param>
        private void UpdateText(Map map)
        {
            if (map == null)
                return;

            ArtistTitle.Text = map.Artist + " - " + map.Title;

            var mods = ModManager.CurrentModifiersList.Count > 0 ? $" + {ModHelper.GetModsString(ModManager.Mods)}": "";
            DifficultyMods.Text = $"[{map.DifficultyName}]{mods}";

            //  Reset positions of the text
            ArtistTitle.ClearAnimations();
            ArtistTitle.X = 0;

            DifficultyMods.ClearAnimations();
            DifficultyMods.X = 0;
        }

        /// <summary>
        ///     Called when the map has changed. Updates the text with the new map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMapChanged(object sender, BindableValueChangedEventArgs<Map> e) => UpdateText(e.Value);

        /// <summary>
        ///     Called when the player's activated mods change.
        ///     Updates the text of the mods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnModsChanged(object sender, ModsChangedEventArgs e) => UpdateText(MapManager.Selected.Value);
    }
}
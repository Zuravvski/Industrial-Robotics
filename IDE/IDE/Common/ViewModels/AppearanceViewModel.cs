using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using FirstFloor.ModernUI.Presentation;
using IDE.Common.ViewModels.Commands;

namespace IDE.Common.ViewModels
{
    /// <summary>
    /// A simple view model for configuring theme, font and accent colors.
    /// </summary>
    /// <seealso cref="IDE.Common.ViewModels.Commands.ObservableObject" />
    public class AppearanceViewModel : ObservableObject
    {
        /// <summary>
        /// The font small
        /// </summary>
        private const string FontSmall = "small";
        /// <summary>
        /// The font large
        /// </summary>
        private const string FontLarge = "large";

        /// <summary>
        /// The palette metro
        /// </summary>
        private const string PaletteMetro = "metro";
        /// <summary>
        /// The palette wp
        /// </summary>
        private const string PaletteWP = "windows phone";

        /// <summary>
        /// The metro accent colors
        /// </summary>
        private Color[] metroAccentColors = {
            Color.FromRgb(0x33, 0x99, 0xff),   // blue
            Color.FromRgb(0x00, 0xab, 0xa9),   // teal
            Color.FromRgb(0x33, 0x99, 0x33),   // green
            Color.FromRgb(0x8c, 0xbf, 0x26),   // lime
            Color.FromRgb(0xf0, 0x96, 0x09),   // orange
            Color.FromRgb(0xff, 0x45, 0x00),   // orange red
            Color.FromRgb(0xe5, 0x14, 0x00),   // red
            Color.FromRgb(0xff, 0x00, 0x97),   // magenta
            Color.FromRgb(0xa2, 0x00, 0xff),   // purple            
        };

        // 20 accent colors from Windows Phone 8
        /// <summary>
        /// The wp accent colors
        /// </summary>
        private readonly Color[] wpAccentColors =
        {
            Color.FromRgb(0xa4, 0xc4, 0x00), // lime
            Color.FromRgb(0x60, 0xa9, 0x17), // green
            Color.FromRgb(0x00, 0x8a, 0x00), // emerald
            Color.FromRgb(0x00, 0xab, 0xa9), // teal
            Color.FromRgb(0x1b, 0xa1, 0xe2), // cyan
            Color.FromRgb(0x00, 0x50, 0xef), // cobalt
            Color.FromRgb(0x6a, 0x00, 0xff), // indigo
            Color.FromRgb(0xaa, 0x00, 0xff), // violet
            Color.FromRgb(0xf4, 0x72, 0xd0), // pink
            Color.FromRgb(0xd8, 0x00, 0x73), // magenta
            Color.FromRgb(0xa2, 0x00, 0x25), // crimson
            Color.FromRgb(0xe5, 0x14, 0x00), // red
            Color.FromRgb(0xfa, 0x68, 0x00), // orange
            Color.FromRgb(0xf0, 0xa3, 0x0a), // amber
            Color.FromRgb(0xe3, 0xc8, 0x00), // yellow
            Color.FromRgb(0x82, 0x5a, 0x2c), // brown
            Color.FromRgb(0x6d, 0x87, 0x64), // olive
            Color.FromRgb(0x64, 0x76, 0x87), // steel
            Color.FromRgb(0x76, 0x60, 0x8a), // mauve
            Color.FromRgb(0x87, 0x79, 0x4e) // taupe
        };

        /// <summary>
        /// The selected palette
        /// </summary>
        private string selectedPalette = PaletteWP;

        /// <summary>
        /// The selected accent color
        /// </summary>
        private Color selectedAccentColor;
        /// <summary>
        /// The selected theme
        /// </summary>
        private Link selectedTheme;
        /// <summary>
        /// The selected font size
        /// </summary>
        private string selectedFontSize;
        /// <summary>
        /// The theme color
        /// </summary>
        private Brush themeColor;

        // Sigleton
        /// <summary>
        /// The instance
        /// </summary>
        private static readonly Lazy<AppearanceViewModel> instance =
            new Lazy<AppearanceViewModel>(() => new AppearanceViewModel());

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static AppearanceViewModel Instance => instance.Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppearanceViewModel"/> class.
        /// </summary>
        protected AppearanceViewModel()
        {
            // add the default themes
            Themes.Add(new Link
            {
                DisplayName = "dark",
                Source = AppearanceManager.DarkThemeSource
            });

            Themes.Add(new Link
            {
                DisplayName = "light",
                Source = AppearanceManager.LightThemeSource
            });

            // add additional themes
            Themes.Add(new Link
            {
                DisplayName = "hello kitty",
                Source = new Uri("/IDE;component/Common/Views/CustomThemes/ModernUI.HelloKitty.xaml", UriKind.Relative)
            });
            Themes.Add(new Link
            {
                DisplayName = "WE ZUT",
                Source = new Uri("/IDE;component/Common/Views/CustomThemes/ModernUI.WE.xaml", UriKind.Relative)
            });

            SelectedFontSize = AppearanceManager.Current.FontSize == FontSize.Small ? FontLarge : FontSmall;
            SyncThemeAndColor();

            AppearanceManager.Current.PropertyChanged += OnAppearanceManagerPropertyChanged;
        }


        /// <summary>
        /// Synchronizes the color of the theme and.
        /// </summary>
        private void SyncThemeAndColor()
        {
            // synchronizes the selected viewmodel theme with the actual theme used by the appearance manager.
            SelectedTheme = Themes.FirstOrDefault(l => l.Source.Equals(AppearanceManager.Current.ThemeSource));

            // and make sure accent color is up-to-date
            SelectedAccentColor = AppearanceManager.Current.AccentColor;
        }

        /// <summary>
        /// Called when [appearance manager property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnAppearanceManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ThemeSource" || e.PropertyName == "AccentColor")
                SyncThemeAndColor();
        }

        /// <summary>
        /// Gets the themes.
        /// </summary>
        /// <value>
        /// The themes.
        /// </value>
        public LinkCollection Themes { get; } = new LinkCollection();

        /// <summary>
        /// Gets the font sizes.
        /// </summary>
        /// <value>
        /// The font sizes.
        /// </value>
        public string[] FontSizes
        {
            get { return new[] {FontSmall, FontLarge}; }
        }

        /// <summary>
        /// Gets the palettes.
        /// </summary>
        /// <value>
        /// The palettes.
        /// </value>
        public string[] Palettes
        {
            get { return new string[] {PaletteMetro, PaletteWP}; }
        }

        /// <summary>
        /// Gets the accent colors.
        /// </summary>
        /// <value>
        /// The accent colors.
        /// </value>
        public Color[] AccentColors
        {
            get { return selectedPalette == PaletteMetro ? metroAccentColors : wpAccentColors; }
        }

        /// <summary>
        /// Gets or sets the selected palette.
        /// </summary>
        /// <value>
        /// The selected palette.
        /// </value>
        public string SelectedPalette
        {
            get { return selectedPalette; }
            set
            {
                if (selectedPalette != value)
                {
                    selectedPalette = value;
                    NotifyPropertyChanged("AccentColors");

                    SelectedAccentColor = AccentColors.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected theme.
        /// </summary>
        /// <value>
        /// The selected theme.
        /// </value>
        public Link SelectedTheme
        {
            get { return selectedTheme; }
            set
            {
                if (selectedTheme != value)
                {
                    selectedTheme = value;
                    NotifyPropertyChanged("SelectedTheme");

                    // and update the actual theme
                    AppearanceManager.Current.ThemeSource = value.Source;
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the selected font.
        /// </summary>
        /// <value>
        /// The size of the selected font.
        /// </value>
        public string SelectedFontSize
        {
            get { return selectedFontSize; }
            set
            {
                if (selectedFontSize != value)
                {
                    selectedFontSize = value;
                    NotifyPropertyChanged("SelectedFontSize");

                    AppearanceManager.Current.FontSize = value == FontLarge ? FontSize.Large : FontSize.Small;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the selected accent.
        /// </summary>
        /// <value>
        /// The color of the selected accent.
        /// </value>
        public Color SelectedAccentColor
        {
            get { return selectedAccentColor; }
            set
            {
                if (selectedAccentColor != value)
                {
                    selectedAccentColor = value;
                    NotifyPropertyChanged("SelectedAccentColor");
                    AppearanceManager.Current.AccentColor = value;
                    ThemeColor = new SolidColorBrush(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the theme.
        /// </summary>
        /// <value>
        /// The color of the theme.
        /// </value>
        public Brush ThemeColor
        {
            get { return themeColor; }
            set
            {
                themeColor = value;
                NotifyPropertyChanged("ThemeColor");
            }
        }
    }
}

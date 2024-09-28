using osu.Framework.Graphics;
using NUnit.Framework;
using osu.Framework.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Extensions.Color4Extensions;

namespace ThemeSwitch.Game.Tests.Visual
{
    [TestFixture]
    public partial class TestSceneThemeToggle : ThemeSwitchTestScene
    {
        // Add visual tests to ensure correct behaviour of your game: https://github.com/ppy/osu-framework/wiki/Development-and-Testing
        // You can make changes to classes associated with the tests and they will recompile and update immediately.


        private static readonly Color4 dark_background = Color4Extensions.FromHex("#535C72");
        private static readonly Color4 light_background = Color4Extensions.FromHex("#CDE7FF");
        private Box background;

        private ThemeToggle themeToggle;

        public TestSceneThemeToggle()
        {
            AddToggleStep("Toggle background", b => background.FadeColour(b ? dark_background : light_background, 100));

            AddToggleStep("Toggle the toggle", b => themeToggle.Toggle());

            AddSliderStep("Set toggle scale", 0.5f, 5f, 2.50f, s => themeToggle.Scale = new osuTK.Vector2(s));
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                },
                themeToggle = new ThemeToggle
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            themeToggle.Current.BindValueChanged(v =>
            {
                background.FadeColour(v.NewValue == Theme.Light ? light_background : dark_background, 500, Easing.InOutCubic);
            }, true);
        }
    }
}

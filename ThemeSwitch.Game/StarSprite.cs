using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK.Graphics;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osu.Framework.Bindables;
using osu.Framework.Utils;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Textures;

namespace ThemeSwitch.Game;

public partial class StarSprite : Sprite
{
    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Size = new(25.6f);

        Texture = textures.Get(@"star.png");
    }
}
using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace ThemeSwitch.Game;

public partial class CloudGroup : Container
{
    public CloudGroup()
    {
        Origin = Anchor.Centre;
        RelativeSizeAxes = Axes.Both;
        Size = Vector2.One;
        Rotation = -3.84f;

        InternalChildren = new Drawable[]
        {
            new WhiteCircle
            {
                Position = new(0f, 6.4f),
            },
            new WhiteCircle
            {
                Position = new(16.0f, 0f),
            },
            new WhiteCircle
            {
                Position = new(16.0f, 12.79f),
            },
            new WhiteCircle
            {
                Position = new(29.85f, 3.2f),
            },
            new WhiteCircle
            {
                Position = new(35.18f, 13.86f),
            },
            new WhiteCircle
            {
                Position = new(46.9f, 6.4f),
            }
        };
    }

    private partial class WhiteCircle : FastCircle
    {
        public WhiteCircle()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            Colour = Colour4.White;
            Size = new(23f);
        }
    }
}
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
using osuTK.Graphics.OpenGL;

namespace ThemeSwitch.Game;

public partial class SunMoon : CircularContainer
{
    private readonly Bindable<float> animationProgress = new Bindable<float>();
    private readonly ThemeToggle themeToggle;
    private Drawable moonDark;
    private MaskingCircle planetCircle;
    private Drawable planet;

    public static readonly Color4 SUN_COLOR = Color4Extensions.FromHex("#FFBF43").Opacity(0.96f);
    public static readonly Color4 MOON_COLOR = Color4Extensions.FromHex("#DEE5F3");

    private static readonly EdgeEffectParameters light_glow_effect = new EdgeEffectParameters
    {
        Type = EdgeEffectType.Glow,
        Radius = 5.0f,
        Colour = Color4Extensions.FromHex("#B7B7B7").Opacity(0.2f),
        Offset = new Vector2(0.5f, 0.25f),
    };

    private static readonly EdgeEffectParameters dark_glow_effect = new EdgeEffectParameters
    {
        Type = EdgeEffectType.Shadow,
        Radius = 10f,
        Colour = Color4Extensions.FromHex("#B7B7B7").Opacity(0.125f),
        Offset = new Vector2(-1f, 0.25f),
    };

    private MaskingCircle glowEffect;
    private Container haloContainer;

    public SunMoon(ThemeToggle themeToggle)
    {
        this.themeToggle = themeToggle;

        Children = new Drawable[]
        {
            new MaskingCircle
            {
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Radius = 5.2f,
                    Colour = Color4.Black.Opacity(0.3f),
                    Offset = new Vector2(0f, 0.75f),
                },
            },
            glowEffect = new MaskingCircle
            {
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Glow,
                    Radius = 5.0f,
                    Colour = Color4Extensions.FromHex("#B7B7B7").Opacity(0.3f),
                    Offset = new Vector2(0f, 0.5f),
                },
            },
            haloContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new HaloCircle
                    {
                        Scale = new Vector2(3.25f),
                    },
                    new HaloCircle
                    {
                        Scale = new Vector2(2.5f),
                    },
                    new HaloCircle
                    {
                        Scale = new Vector2(1.75f),
                        Alpha = 0.75f
                    },
                }
            },
            planet = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    planetCircle = new MaskingCircle
                    {
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                        },
                    },
                    new MaskingCircle
                    {
                        Child = moonDark = new FastCircle
                        {
                            Colour = Color4Extensions.FromHex("#1F2533").Opacity(0.79f),
                        }
                    },
                }
            },
        };
    }

    private Vector2 moonDarkHiddenSize => planetCircle.DrawSize;
    private Vector2 moonDarkTargetSize => planetCircle.DrawSize * 0.8f;

    protected override void LoadComplete()
    {
        base.LoadComplete();

        animationProgress.BindTo(themeToggle.AnimationProgress);
        animationProgress.BindValueChanged(v =>
        {
            float pathLength = themeToggle.DragPathLength / 2;
            float moveToX = Interpolation.ValueAt(v.NewValue, -pathLength, pathLength, -1f, 1f, IsDragged ? Easing.None : Easing.InOutQuint);
            Position = new Vector2(moveToX, 0);

            var glowEffectAlpha = Interpolation.ValueAt(v.NewValue, light_glow_effect, dark_glow_effect, -1f, 1f, Easing.InOutQuint);
            glowEffect.EdgeEffect = glowEffectAlpha;

            var planetColor = Interpolation.ValueAt(v.NewValue, SUN_COLOR, MOON_COLOR, -1f, 1f, Easing.InOutQuint);
            planetCircle.Colour = planetColor;

            var moonDarkAlpha = Interpolation.ValueAt(v.NewValue, 0.0f, 1.0f, -1f, 1f, Easing.InOutQuint);
            moonDark.Alpha = moonDarkAlpha;

            Vector2 moonDarkSize = Interpolation.ValueAt(v.NewValue, moonDarkHiddenSize, moonDarkTargetSize, -1f, 1f, Easing.InOutQuint);
            moonDark.Size = moonDarkSize;

            float haloAlpha = Interpolation.ValueAt(v.NewValue, 1.0f, 0.1f, -1f, 1f, Easing.InOutQuint);
            haloContainer.Alpha = haloAlpha;
        }, true);

        planet.RotateTo(5, 1000, Easing.InOutSine)
                .Then()
                .RotateTo(-15, 1000, Easing.InOutSine)
                .Loop();
    }

    private partial class HaloCircle : FastCircle
    {
        public HaloCircle()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            Colour = Color4.White.Opacity(0.25f);
        }
    }

    private partial class MaskingCircle : CircularContainer
    {
        public MaskingCircle()
        {
            Masking = true;
            RelativePositionAxes = Axes.Both;
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        base.OnMouseUp(e);
    }

    protected override void OnDragEnd(DragEndEvent e)
    {
        base.OnDragEnd(e);

        themeToggle.DragEnd();
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        themeToggle.DragStart();

        return true;
    }

    protected override void OnDrag(DragEvent e)
    {
        base.OnDrag(e);

        float localX = ToLocalSpace(e.ScreenSpaceMousePosition).X;
        float localStartX = ToLocalSpace(e.ScreenSpaceMouseDownPosition).X;

        themeToggle.Dragging(localX - localStartX);
    }

    private const float scale_duration = 100;
    protected override bool OnHover(HoverEvent e)
    {
        planet.ScaleTo(1.075f, scale_duration);

        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        planet.ScaleTo(1.0f, scale_duration);

        base.OnHoverLost(e);
    }
}

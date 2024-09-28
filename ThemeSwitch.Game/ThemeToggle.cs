#nullable enable

using System;
using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;

namespace ThemeSwitch.Game;

public partial class ThemeToggle : Container, IHasCurrentValue<Theme>
{
    public readonly Bindable<float> AnimationProgress = new Bindable<float>();
    private readonly BindableWithCurrent<Theme> current = new BindableWithCurrent<Theme>(Theme.Dark);

    public Theme DefaultValue
    {
        get => current.Default;
        set => current.Default = value;
    }

    public Bindable<Theme> Current
    {
        get => current.Current;
        set => current.Current = value;
    }

    public void Toggle()
    {
        current.Value = current.Value is Theme.Light ? Theme.Dark : Theme.Light;

        Theme theme = current.Value;

        cancelCloudAnimation();

        this.TransformBindableTo(AnimationProgress, theme is Theme.Light ? -1 : 1, fade_duration)
            .OnComplete(_ => beginCloudAnimation());
    }

    private static readonly Color4 light_background = Color4Extensions.FromHex("#A2D1FD");
    private static readonly Color4 dark_background = Color4Extensions.FromHex("#1F2533");

    private CircularContainer borderMask;
    private Box background;

    private Container starContainer;
    private Container leftLeaveParts;
    private Container rightLeaveParts;

    public ThemeToggle()
    {
        AutoSizeAxes = Axes.Both;

        Child = borderMask = new CircularContainer
        {
            Masking = true,
            Size = new Vector2(160, 64),
            Child = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    background = new ClickableBox(this)
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                    starContainer = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                    leftLeaveParts = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Origin = Anchor.TopLeft,
                        Children = new Drawable[]
                        {
                            new CloudGroup
                            {
                                Position = new Vector2(10f, 55f),
                                Scale = new Vector2(1.5f),
                            },
                        }
                    },
                    rightLeaveParts = new Container
                    {
                        Origin = Anchor.TopLeft,
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            new CloudGroup
                            {
                                Position = new Vector2(87f, 50f),
                                Scale = new Vector2(1.5f),
                                Rotation = -25,
                            },
                            new CloudGroup
                            {
                                Position = new Vector2(85f, 70f),
                                Scale = new Vector2(1.5f),
                                Rotation = -25,
                            },
                        }
                    },
                    new SunMoon(this)
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(44),
                    }
                }
            },
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Shadow,
                Colour = Color4.Black.Opacity(0.1f),
                Radius = 10,
                Offset = new Vector2(0, 0.5f),
            },
            BorderThickness = 2f,
        };
    }

    private static readonly ColourInfo light_border = new ColourInfo
    {
        TopLeft = Color4Extensions.FromHex("#6CB8FF"),
        TopRight = Color4Extensions.FromHex("#6CB8FF"),
        BottomLeft = Color4.White.Opacity(0),
        BottomRight = Color4.White.Opacity(0),
    };

    private static readonly ColourInfo dark_border = new ColourInfo
    {
        TopLeft = Color4.Black.Opacity(0.87f),
        TopRight = Color4.Black.Opacity(0.87f),
        BottomLeft = Color4.White.Opacity(0),
        BottomRight = Color4.White.Opacity(0),
    };

    public float DragPathLength => borderMask.Width - borderMask.Height;

    private float? relativeValueAtDragStart = null;

    public void Dragging(float localDeltaX)
    {
        if (relativeValueAtDragStart is null)
            return;

        float dragProgress = relativeValueAtDragStart.Value + (localDeltaX / DragPathLength) * 2;
        dragProgress = Math.Clamp(dragProgress, -1, 1);

        AnimationProgress.Value = dragProgress;
    }

    public void DragStart()
    {
        Debug.Assert(relativeValueAtDragStart is null);

        relativeValueAtDragStart = current.Value is Theme.Light ? -1 : 1;

        cancelCloudAnimation();
    }

    private void beginCloudAnimation()
    {
        leftLeaveParts.MoveToOffset(new Vector2(2, -6), 1000, Easing.InOutSine)
            .Then()
            .MoveToOffset(new Vector2(-2, 6), 1000, Easing.InOutSine)
            .Loop();

        rightLeaveParts.MoveToOffset(new Vector2(-2, -6), 1000, Easing.InOutSine)
            .Then()
            .MoveToOffset(new Vector2(2, 6), 1000, Easing.InOutSine)
            .Loop();
    }

    private void cancelCloudAnimation()
    {
        leftLeaveParts.MoveToOffset(Vector2.Zero);
        rightLeaveParts.MoveToOffset(Vector2.Zero);
    }

    public void DragEnd()
    {
        if (relativeValueAtDragStart is null)
            return;

        var dragProgress = AnimationProgress.Value;

        if (dragProgress > 0)
        {
            current.Value = Theme.Dark;
        }
        else if (dragProgress < 0)
        {
            current.Value = Theme.Light;
        }

        this.TransformBindableTo(AnimationProgress, current.Value is Theme.Light ? -1 : 1, fade_duration)
            .OnComplete(_ => beginCloudAnimation());

        relativeValueAtDragStart = null;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        for (int i = 0; i < 15; i++)
        {
            starContainer.Add(new StarSprite
            {
                Position = new Vector2(RNG.NextSingle(-borderMask.Width / 2 * 0.85f, 0), RNG.NextSingle(-borderMask.Height / 2 * 0.9f, borderMask.Height / 2)),
                Scale = new Vector2(RNG.NextSingle(0.25f, 0.5f)),
            });
        }
    }

    private Vector2 leftLeavePartStartPosition;
    private Vector2 rightLeavePartStartPosition;

    private const float fade_duration = 550;
    protected override void LoadComplete()
    {
        base.LoadComplete();

        leftLeavePartStartPosition = leftLeaveParts.Position;
        rightLeavePartStartPosition = rightLeaveParts.Position;

        AnimationProgress.Value = current.Value is Theme.Light ? -1 : 1;

        AnimationProgress.BindValueChanged(v =>
        {
            var backgroundColor = Interpolation.ValueAt(v.NewValue, light_background, dark_background, -1, 1, Easing.InOutQuint);
            background.Colour = backgroundColor;

            var borderColor = Interpolation.ValueAt(v.NewValue, light_border, dark_border, -1, 1, Easing.InOutQuint);
            borderMask.BorderColour = borderColor;

            float leftLerpFactor = v.NewValue;

            if (leftLerpFactor > 0.5)
                leftLerpFactor = 1;
            else
                leftLerpFactor = (leftLerpFactor + 1) / 3 * 2;

            leftLeaveParts.Position = Interpolation.ValueAt(leftLerpFactor, leftLeavePartStartPosition, new Vector2(-20, 55), 0, 1, Easing.InBack);
            rightLeaveParts.Position = Interpolation.ValueAt(leftLerpFactor, rightLeavePartStartPosition, new Vector2(100, 52), 0, 1, Easing.InBack);

            float starAlpha = Interpolation.ValueAt(v.NewValue, 0, 1, -1, 1, Easing.InOutQuint);
            starContainer.Alpha = starAlpha;
        }, true);

        foreach (var star in starContainer.Children)
        {
            float randomStart = RNG.NextSingle() * (fade_duration * 2);

            using (BeginDelayedSequence(randomStart))
            {
                star.FadeInFromZero(fade_duration)
                    .Then()
                    .FadeOutFromOne(fade_duration)
                    .Loop(fade_duration);
            }
        }
    }

    private partial class ClickableBox : Box
    {
        private readonly ThemeToggle themeToggle;

        public ClickableBox(ThemeToggle themeToggle)
        {
            this.themeToggle = themeToggle;
        }

        protected override bool OnClick(ClickEvent e)
        {
            themeToggle.Toggle();

            return true;
        }
    }
}
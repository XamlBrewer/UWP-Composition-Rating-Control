using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;

namespace XamlBrewer.Uwp.Controls
{
    [TemplatePart(Name = ItemsPartName, Type = typeof(StackPanel))]
    [TemplatePart(Name = InteractionPartName, Type = typeof(UIElement))]
    public class Rating : Control
    {
        private const string ItemsPartName = "PART_Items";
        private const string InteractionPartName = "PART_Interaction";

        #region Dependency Property Registrations

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            nameof(Maximum),
            typeof(int),
            typeof(Rating),
            new PropertyMetadata(5, OnStructureChanged));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(Rating),
            new PropertyMetadata(0d, OnValueChanged));

        public static readonly DependencyProperty StepFrequencyProperty = DependencyProperty.Register(
            nameof(StepFrequency),
            typeof(double),
            typeof(Rating),
            new PropertyMetadata(.5, OnStructureChanged));

        public static readonly DependencyProperty EmptyImageProperty = DependencyProperty.Register(
            nameof(EmptyImage),
            typeof(Uri),
            typeof(Rating),
            new PropertyMetadata(new Uri("ms-appx:///XamlBrewer.Uwp.Rating/Assets/defaultStar_empty.png"), OnStructureChanged));

        public static readonly DependencyProperty FilledImageProperty = DependencyProperty.Register(
            nameof(FilledImage),
            typeof(Uri),
            typeof(Rating),
            new PropertyMetadata(new Uri("ms-appx:///XamlBrewer.Uwp.Rating/Assets/defaultStar_full.png"), OnStructureChanged));

        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            nameof(ItemHeight),
            typeof(int),
            typeof(Rating),
            new PropertyMetadata(12, OnStructureChanged));

        public static readonly DependencyProperty ItemPaddingProperty = DependencyProperty.Register(
            nameof(ItemPadding),
            typeof(int),
            typeof(Rating),
            new PropertyMetadata(2, OnStructureChanged));

        public static readonly DependencyProperty IsInteractiveProperty = DependencyProperty.Register(
            nameof(IsInteractive),
            typeof(bool),
            typeof(Rating),
            new PropertyMetadata(true));

        #endregion

        public Rating()
        {
            DefaultStyleKey = typeof(Rating);
        }

        private List<InsetClip> Clips { get; set; } = new List<InsetClip>();

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double StepFrequency
        {
            get { return (double)GetValue(StepFrequencyProperty); }
            set { SetValue(StepFrequencyProperty, value); }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public Uri EmptyImage
        {
            get { return (Uri)GetValue(EmptyImageProperty); }
            set { SetValue(EmptyImageProperty, value); }
        }

        public Uri FilledImage
        {
            get { return (Uri)GetValue(FilledImageProperty); }
            set { SetValue(FilledImageProperty, value); }
        }

        public int ItemHeight
        {
            get { return (int)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public int ItemPadding
        {
            get { return (int)GetValue(ItemPaddingProperty); }
            set { SetValue(ItemPaddingProperty, value); }
        }

        public bool IsInteractive
        {
            get { return (bool)GetValue(IsInteractiveProperty); }
            set { SetValue(IsInteractiveProperty, value); }
        }

        /// <summary>
        /// Update the visual state of the control when its template is changed.
        /// </summary>
        protected override async void OnApplyTemplate()
        {
            // Ensures that ActualWidth is actually the actual width.
            HorizontalAlignment = HorizontalAlignment.Left;

            await OnStructureChanged(this);

            var surface = GetTemplateChild(InteractionPartName) as UIElement;
            if (surface != null)
            {
                surface.Tapped += Surface_Tapped;
                surface.ManipulationDelta += Surface_ManipulationDelta;
            }

            base.OnApplyTemplate();
        }

        private static async void OnStructureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            await OnStructureChanged(d);
        }

        private static async Task OnStructureChanged(DependencyObject d)
        {
            var c = (Rating)d;

            if (c.EmptyImage == null)
            {
                c.EmptyImage = new Uri("ms-appx:///XamlBrewer.Uwp.Rating/Assets/defaultStar_empty.png");
            }

            if (c.FilledImage == null)
            {
                c.FilledImage = new Uri("ms-appx:///XamlBrewer.Uwp.Rating/Assets/defaultStar_full.png");
            }

            if ((c.StepFrequency <= 0) || (c.StepFrequency > 1))
            {
                c.StepFrequency = 1;
            }

            var panel = c.GetTemplateChild(ItemsPartName) as StackPanel;
            if (panel != null)
            {
                // Load images.
                var root = panel.GetVisual();
                var compositor = root.Compositor;
                var canvasDevice = new CanvasDevice();
                var compositionDevice = CanvasComposition.CreateCompositionGraphicsDevice(compositor, canvasDevice);

                var rightPadding = c.ItemPadding;
                c.Clips.Clear();

                for (int i = 0; i < c.Maximum; i++)
                {
                    if (i == c.Maximum - 1)
                    {
                        rightPadding = 0;
                    }

                    // Create grid.
                    var grid = new Grid
                    {
                        Height = c.ItemHeight,
                        Width = c.ItemHeight,
                        Margin = new Thickness(0, 0, rightPadding, 0)
                    };
                    panel.Children.Add(grid);
                    var gridRoot = grid.GetVisual();

                    // Empty image.
                    var surface = await LoadFromUri(canvasDevice, compositionDevice, c.EmptyImage, new Size(c.ItemHeight, c.ItemHeight));
                    var emptyBrush = compositor.CreateSurfaceBrush(surface);
                    var spriteVisual = compositor.CreateSpriteVisual();
                    spriteVisual.Size = new Vector2(c.ItemHeight, c.ItemHeight);
                    gridRoot.Children.InsertAtTop(spriteVisual);
                    spriteVisual.Brush = emptyBrush;

                    // Filled image.
                    surface = await LoadFromUri(canvasDevice, compositionDevice, c.FilledImage, new Size(c.ItemHeight, c.ItemHeight));
                    var fullBrush = compositor.CreateSurfaceBrush(surface);
                    spriteVisual = compositor.CreateSpriteVisual();
                    spriteVisual.Size = new Vector2(c.ItemHeight, c.ItemHeight);
                    var clip = compositor.CreateInsetClip();
                    c.Clips.Add(clip);
                    spriteVisual.Clip = clip;
                    gridRoot.Children.InsertAtTop(spriteVisual);
                    spriteVisual.Brush = fullBrush;
                }

                compositionDevice.Dispose();
                canvasDevice.Dispose();
            }

            OnValueChanged(c);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnValueChanged(d);
        }

        private static void OnValueChanged(DependencyObject d)
        {
            Rating c = (Rating)d;

            var panel = c.GetTemplateChild(ItemsPartName) as StackPanel;
            if (panel != null)
            {
                for (int i = 0; i < c.Maximum; i++)
                {
                    if (i <= Math.Floor(c.Value - 1))
                    {
                        // Filled image.
                        c.Clips[i].RightInset = 0;
                    }
                    else if (i > Math.Ceiling(c.Value - 1))
                    {
                        // Empty image.
                        c.Clips[i].RightInset = c.ItemHeight;
                    }
                    else
                    {
                        // Curtain.
                        c.Clips[i].RightInset = (float)(c.ItemHeight * (1 + Math.Floor(c.Value) - c.Value));
                    }
                }
            }
        }

        private void Surface_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!IsInteractive)
            {
                return;
            }

            Value = (int)(e.GetPosition(this).X / (ActualWidth + ItemPadding) * Maximum) + 1;
        }

        private void Surface_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (!IsInteractive)
            {
                return;
            }

            // Floor.
            var value = Math.Floor(e.Position.X / (ActualWidth + ItemPadding) * Maximum);

            // Step.
            value += Math.Min(RoundToFraction(((e.Position.X - (ItemHeight + ItemPadding) * (value)) / (ItemHeight)), StepFrequency), 1);

            // Keep within range.
            if (value < 0)
            {
                value = 0;
            }
            else if (value > Maximum)
            {
                value = Maximum;
            }

            Value = value;
        }

        public static double RoundToFraction(double number, double fraction)
        {
            // We assume that fraction is a value between 0 and 1.
            if (fraction <= 0) { return 0; }
            if (fraction > 1) { return number; }

            double modulo = number % fraction;
            if ((fraction - modulo) <= modulo)
                modulo = (fraction - modulo);
            else
                modulo *= -1;

            return number + modulo;
        }

        private static async Task<CompositionDrawingSurface> LoadFromUri(CanvasDevice canvasDevice, CompositionGraphicsDevice compositionDevice, Uri uri, Size sizeTarget)
        {
            CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(canvasDevice, uri);
            Size sizeSource = bitmap.Size;

            if (sizeTarget.IsEmpty)
            {
                sizeTarget = sizeSource;
            }

            var surface = compositionDevice.CreateDrawingSurface(
                sizeTarget,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                DirectXAlphaMode.Premultiplied);

            using (var ds = CanvasComposition.CreateDrawingSession(surface))
            {
                ds.Clear(Color.FromArgb(0, 0, 0, 0));
                ds.DrawImage(
                    bitmap,
                    new Rect(0, 0, sizeTarget.Width, sizeTarget.Height),
                    new Rect(0, 0, sizeSource.Width, sizeSource.Height),
                    1,
                    CanvasImageInterpolation.HighQualityCubic);
            }

            return surface;
        }
    }

    public static class UIElementExtensions
    {
        public static ContainerVisual GetVisual(this UIElement element)
        {
            var hostVisual = ElementCompositionPreview.GetElementVisual(element);
            var root = hostVisual.Compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(element, root);
            return root;
        }
    }
}

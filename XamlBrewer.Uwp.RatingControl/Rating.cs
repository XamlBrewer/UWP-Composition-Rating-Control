using Microsoft.UI.Composition.Toolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace XamlBrewer.Uwp.Controls
{
    [TemplatePart(Name = ItemsPartName, Type = typeof(StackPanel))]
    [TemplatePart(Name = InteractionPartName, Type = typeof(Grid))]
    public class Rating : Control
    {
        private const string ItemsPartName = "PART_Items";
        private const string InteractionPartName = "PART_Surface";

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
            new PropertyMetadata(1d, OnStructureChanged));

        public static readonly DependencyProperty EmptyImageProperty = DependencyProperty.Register(
            nameof(EmptyImage),
            typeof(Uri),
            typeof(Rating),
            new PropertyMetadata(null, OnStructureChanged));

        public static readonly DependencyProperty FilledImageProperty = DependencyProperty.Register(
            nameof(FilledImage),
            typeof(Uri),
            typeof(Rating),
            new PropertyMetadata(null, OnStructureChanged));

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

        #endregion

        public Rating()
        {
            this.DefaultStyleKey = typeof(Rating);
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

        /// <summary>
        /// Update the visual state of the control when its template is changed.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            OnStructureChanged(this);

            var surface = this.GetTemplateChild(InteractionPartName) as Grid;
            if (surface != null)
            {
                surface.Tapped += Surface_Tapped;
                surface.ManipulationDelta += Surface_ManipulationDelta;
            }

            base.OnApplyTemplate();
        }

        private static void OnStructureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnStructureChanged(d);
        }

        private static void OnStructureChanged(DependencyObject d)
        {
            Rating c = (Rating)d;

            var panel = c.GetTemplateChild(ItemsPartName) as StackPanel;
            if (panel != null)
            {
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

                    // Load images.
                    var root = grid.GetVisual();
                    var compositor = root.Compositor;
                    var imageFactory = CompositionImageFactory.CreateCompositionImageFactory(compositor);

                    // Empty image.
                    var spriteVisual = compositor.CreateSpriteVisual();
                    spriteVisual.Size = new Vector2(c.ItemHeight, c.ItemHeight);
                    root.Children.InsertAtTop(spriteVisual);
                    var options = new CompositionImageOptions()
                    {
                        DecodeWidth = c.ItemHeight,
                        DecodeHeight = c.ItemHeight
                    };
                    var image = imageFactory.CreateImageFromUri(c.EmptyImage, options);
                    var brush = compositor.CreateSurfaceBrush(image.Surface);
                    spriteVisual.Brush = brush;

                    // Filled image.
                    spriteVisual = compositor.CreateSpriteVisual();
                    spriteVisual.Size = new Vector2(c.ItemHeight, c.ItemHeight);
                    var clip = compositor.CreateInsetClip();
                    c.Clips.Add(clip);
                    spriteVisual.Clip = clip;
                    root.Children.InsertAtTop(spriteVisual);
                    image = imageFactory.CreateImageFromUri(c.FilledImage, options);
                    brush = compositor.CreateSurfaceBrush(image.Surface);
                    spriteVisual.Brush = brush;
                }
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
                        c.Clips[i].RightInset = 0;
                    }
                    else if (i > Math.Ceiling(c.Value - 1))
                    {
                        c.Clips[i].RightInset = c.ItemHeight;
                    }
                    else
                    {
                        c.Clips[i].RightInset = (float)(c.ItemHeight - c.ItemHeight * RoundToInterval(c.Value - Math.Floor(c.Value) , c.StepFrequency));
                    }
                }
            }
        }

        private void Surface_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            // TODO: consider the paddings.

            var value = RoundToInterval((e.Position.X / ActualWidth * Maximum) + 1, StepFrequency);
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

        private void Surface_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Value = (int)(e.GetPosition(this).X / ActualWidth * Maximum) + 1;
        }

        public static double RoundToInterval(double number, double roundingInterval)
        {
            if (roundingInterval == 0) { return 0; }

            double intv = Math.Abs(roundingInterval);
            double modulo = number % intv;
            if ((intv - modulo) == modulo)
            {
                var temp = (number - modulo).ToString("#.##################");
                if (temp.Length != 0 && temp[temp.Length - 1] % 2 == 0) modulo *= -1;
            }
            else if ((intv - modulo) < modulo)
                modulo = (intv - modulo);
            else
                modulo *= -1;

            return number + modulo;
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            }

            base.OnApplyTemplate();
        }

        private void Surface_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Value = (int)(e.GetPosition(this).X / ActualWidth * Maximum) + 1;
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
                for (int i = 0; i < c.Maximum; i++)
                {
                    panel.Children.Add(
                        new Grid
                        {
                            Background = new SolidColorBrush(Colors.BlueViolet),
                            Height = c.ItemHeight,
                            Width = c.ItemHeight,
                            Margin = new Thickness(0, 0, c.ItemPadding, 0)
                        });
                }
            }

            // ...

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

            }
        }
    }
}

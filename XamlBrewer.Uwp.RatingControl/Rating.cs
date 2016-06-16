using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace XamlBrewer.Uwp.Controls
{
    public class Rating : Control
    {
        #region Dependency Property Registrations

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            nameof(Maximum),
            typeof(int),
            typeof(Rating),
            new PropertyMetadata(5));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(Rating),
            new PropertyMetadata(0d));

        public static readonly DependencyProperty EmptyImageProperty = DependencyProperty.Register(
            nameof(EmptyImage),
            typeof(Uri),
            typeof(Rating),
            new PropertyMetadata(null));

        public static readonly DependencyProperty FilledImageProperty = DependencyProperty.Register(
            nameof(FilledImage),
            typeof(Uri),
            typeof(Rating),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            nameof(ItemHeight),
            typeof(int),
            typeof(Rating),
            new PropertyMetadata(12));

        public static readonly DependencyProperty ItemPaddingProperty = DependencyProperty.Register(
            nameof(ItemPadding),
            typeof(int),
            typeof(Rating),
            new PropertyMetadata(2));

        #endregion

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
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
            base.OnApplyTemplate();
        }

        private static void OnStructureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnStructureChanged(d);
        }

        private static void OnStructureChanged(DependencyObject d)
        {
            Rating c = (Rating)d;

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
        }
    }
}

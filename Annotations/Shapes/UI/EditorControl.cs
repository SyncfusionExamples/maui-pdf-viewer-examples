using Microsoft.Maui.Controls.Shapes;
using Syncfusion.Maui.Sliders;

namespace Shapes
{
    internal class EditorControl : Border
    {
        internal double cornerRadius = 4;
        internal double borderThickness = 0.15;
        double opacity = 1;
        string colorCode = null;
        double thickness = 5;

        Button SelectedButton;
        VerticalStackLayout ControlLayout = new VerticalStackLayout();
        SfSlider OpacitySlider = new SfSlider() { Minimum = 0, Maximum = 1, Value = 1 };
        SfSlider ThicknessSlider = new SfSlider() { Minimum = 0, Maximum = 12, Value = 5 };
        internal VerticalStackLayout ColorPaletteLayOut { get; set; }
        internal VerticalStackLayout OpacitySliderLayOut { get; set; }
        internal VerticalStackLayout ThicknessSliderLayOut { get; set; }
        internal Line OpacityThicknessSeperator { get; set; }
        internal Line ColorOpacitySeperator { get; set; }
        internal event EventHandler<ColorChangedEventArgs> ColorChanged;
        internal event EventHandler<EventArgs> OpacityChangedEnd;
        internal event EventHandler<EventArgs> ThicknessChangedEnd;

        /// <summary>
        /// Gets or sets the modified opacity value.
        /// </summary>
        internal double EditorOpacity
        {
            get
            {
                return opacity;
            }
            set 
            { 
                opacity = value; 
                OpacitySlider.Value = value;
            }
        }
        /// <summary>
        /// Gets or sets the modified thickness value.
        /// </summary>
        internal double EditorThickness
        {
            get
            {
                return thickness;
            }
            set
            {
                thickness = value;
                ThicknessSlider.Value = value;
            }
        }

        /// <summary>
        /// Returns the modified color code.
        /// </summary>
        internal string ColorCode
        {
            get
            {
                return colorCode;
            }
        }

        internal double CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = value;
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(cornerRadius)
                };
            }
        }
        internal double BorderThickness
        {
            get => borderThickness;
            set
            {
                borderThickness = value;
                this.StrokeThickness = borderThickness;
            }
        }

        /// <summary>
        /// Configures and adds a color picker to the control layout using the provided color codes.
        /// </summary>
        /// <param name="colorCodes"></param>
        public void ConfigureColorPicker(string[,] colorCodes)
        {
            ControlLayout.Children.Add(ColorPicker(colorCodes));
        }

        public EditorControl()
        {
            this.Content = ControlLayout;
            this.Stroke = Brush.Black;
            ControlLayout.Spacing = 6;
            this.WidthRequest = 188;
            this.HeightRequest = 200;
            this.Padding = 8;
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(cornerRadius)
            };
            OpacitySlider.ValueChangeEnd += OpacitySlider_ValueChangeEnd;
            ThicknessSlider.ValueChangeEnd += ThicknessSlider_ValueChangeEnd;
            this.StrokeThickness = borderThickness;
        }

        private void OpacitySlider_ValueChangeEnd(object sender, EventArgs e)
        {
            EditorOpacity = OpacitySlider.Value;
            OpacityChangedEnd?.Invoke(this, e);
        }
        private void ThicknessSlider_ValueChangeEnd(object sender, EventArgs e)
        {
            EditorThickness = ThicknessSlider.Value;
            ThicknessChangedEnd?.Invoke(this, e);
        }

        internal void ConfigureOpacity(int value)
        {
            OpacitySlider.Value = value;
            ColorOpacitySeperator = Separator();
            ControlLayout.Children.Add(ColorOpacitySeperator);
            OpacitySlider.ValueChangeEnd += OpacitySlider_ValueChangeEnd;
            ControlLayout.Children.Add(OpacityController("Opacity"));
        }

        internal void ConfigureThickness()
        {
            
            OpacityThicknessSeperator = Separator();
            ControlLayout.Children.Add(OpacityThicknessSeperator);
            ThicknessSlider.ValueChangeEnd += ThicknessSlider_ValueChangeEnd;
            ControlLayout.Children.Add(ThicknessController("Thickness"));
        }

        VerticalStackLayout ColorPicker(string[,] colorCodes)
        {
            VerticalStackLayout verticalStackLayout = new VerticalStackLayout();
            verticalStackLayout.Spacing = 2;
            for (int i = 0; i < colorCodes.GetLength(0); i++)
            {
                HorizontalStackLayout horizontalStackLayout = new HorizontalStackLayout();
                horizontalStackLayout.Spacing = 2;
                for(int j = 0; j < colorCodes.GetLength(1);j++)
                {
                    Button button = new Button();
                    button.BorderWidth = 1;
                    button.BorderColor = Colors.Transparent;
                    button.Padding = 1;
                    button.HeightRequest = button.WidthRequest = 32;
                    button.CornerRadius = 16;
                    button.Clicked += Button_Clicked;
                    button.BackgroundColor = Color.FromArgb(colorCodes[i, j]);
                    horizontalStackLayout.Children.Add(button);
                }
                verticalStackLayout.Children.Add(horizontalStackLayout);
            }
            ColorPaletteLayOut = verticalStackLayout;
            return ColorPaletteLayOut;
        }

        internal void Clear()
        {
            if (SelectedButton != null)
                SelectedButton.BorderColor = Colors.Transparent;
            colorCode = null;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Clear();
            Button button = sender as Button;
            button.BorderColor = Colors.Black;
            colorCode = button.BackgroundColor.ToArgbHex();
            ColorChangedEventArgs colorChangedEventArgs = new ColorChangedEventArgs(colorCode);
            ColorChanged?.Invoke(this, colorChangedEventArgs);
            SelectedButton = button;
        }

        VerticalStackLayout OpacityController(string type)
        {
            VerticalStackLayout verticalStackLayout = new VerticalStackLayout();
            Label label = new Label();
            label.Text = type;
            label.FontSize = 10;
            label.TextColor = Colors.Black;
            verticalStackLayout.Children.Add(label);
            verticalStackLayout.Children.Add(OpacitySlider);
            OpacitySliderLayOut = verticalStackLayout;
            return OpacitySliderLayOut;
        }

        VerticalStackLayout ThicknessController(string type)
        {
            VerticalStackLayout verticalStackLayout = new VerticalStackLayout();
            Label label = new Label();
            label.Text = type;
            label.FontSize = 10;
            label.TextColor = Colors.Black;
            verticalStackLayout.Children.Add(label);
            verticalStackLayout.Children.Add(ThicknessSlider);
            ThicknessSliderLayOut = verticalStackLayout;
            return ThicknessSliderLayOut;
        }

        Line Separator()
        {
            Line line = new Line();
            line.BackgroundColor = Color.FromArgb("#FFCAC4D0");
            line.HeightRequest = 1;
            return line;
        }
    }

    internal class ColorChangedEventArgs: EventArgs
    {
        internal string ColorCode { get; set; }
        internal ColorChangedEventArgs(string colorCode)
        {
            ColorCode = colorCode;
        }
    }
}

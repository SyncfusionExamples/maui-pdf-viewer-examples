using Microsoft.Maui.Controls.Shapes;
using Syncfusion.Maui.TabView;

namespace Shapes;

public partial class ColorPalatte : ContentView
{
    Ellipse? selectedColorButtonHighlight;
    Ellipse? selectedColorButtonHighlightStroke;

    internal float SelectedOpacity { get; set; }
    internal float SelectedFontSize { get; set; } 
    internal float SelectedThickness { get; set; }

    internal float SelectedFillColorOpacity {  get; set; }
    internal event EventHandler<Microsoft.Maui.Graphics.Color>? ShapeStrokeColorChanged;
    internal event EventHandler<Microsoft.Maui.Graphics.Color>? ShapeFillColorChanged;
    internal event EventHandler<float>? BorderThicknessChanged;
    internal event EventHandler<float>? StrokeOpacityChanged;
    internal event EventHandler<float>? FillOpacityChanged;

    Button? PreButton = null;
    public ColorPalatte()
	{
		InitializeComponent();
        Colorpaletteborder.Content = MyGrid;
        this.Content = Colorpaletteborder;
        tabView.SelectionChanged += OnSelectionChanged;
        this.PropertyChanged += ColorPalatte_PropertyChanged;
    }

    private void ColorPalatte_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsVisible))
        {
            if (IsVisible == false)
            {
                if (PreButton != null && selectedColorButtonHighlight != null)
                {
#if !WINDOWS
                    PreButton.HeightRequest = 35;
                    PreButton.WidthRequest = 35;
#if MACCATALYST
                    PreButton.CornerRadius = 17;
#elif IOS
                    PreButton.CornerRadius = 20;
#else
                    PreButton.CornerRadius = 35;
#endif
#else
                    PreButton.HeightRequest = 40;
                    PreButton.WidthRequest = 40;
                    PreButton.CornerRadius = 40;
#endif
                    selectedColorButtonHighlight.Stroke = Brush.Transparent;
                    if (selectedColorButtonHighlight.Parent != null)

                    {
                        ColorFill.Children.Remove(selectedColorButtonHighlight);
                    }
                    selectedColorButtonHighlight = null;
                }
                if (PreButton != null && selectedColorButtonHighlightStroke != null)
                {
#if !WINDOWS
                    PreButton.HeightRequest = 35;
                    PreButton.WidthRequest = 35;
#if MACCATALYST
                    PreButton.CornerRadius = 17;
#elif IOS
                    PreButton.CornerRadius = 20;
#else
                    PreButton.CornerRadius = 35;
#endif
#else
                    PreButton.HeightRequest = 40;
                    PreButton.WidthRequest = 40;
                    PreButton.CornerRadius = 40;
#endif
                    selectedColorButtonHighlightStroke.Stroke = Brush.Transparent;
                    if (selectedColorButtonHighlightStroke.Parent != null)
                    {
                        ColorStroke.Children.Remove(selectedColorButtonHighlightStroke);
                    }
                    selectedColorButtonHighlightStroke = null;
                }
            }
        }
    }


#if MACCATALYST
    Frame Colorpaletteborder = new Frame()
    {
        BackgroundColor = Color.FromArgb("#EEE8F4"),
        BorderColor = Color.FromArgb("#26000000"),
        Padding = new Thickness(0),
        VerticalOptions = LayoutOptions.Start,
        HorizontalOptions = LayoutOptions.Start,
        CornerRadius = 12,
        Shadow = new Shadow
        {
            Offset = new Point(-1, 0),
            Brush = Color.FromRgba("#000000"),
            Radius = 8,
            Opacity = 0.5f
        },
        WidthRequest = 230,
    };
#else
    Border Colorpaletteborder = new Border()
    {
        BackgroundColor = Color.FromArgb("#EEE8F4"),
        Stroke = Color.FromArgb("#26000000"),
        StrokeThickness = 1,
        VerticalOptions = LayoutOptions.Start,
        HorizontalOptions = LayoutOptions.Start,
        StrokeShape = new RoundRectangle
        {
            CornerRadius = new CornerRadius(12)
        },
        Shadow = new Shadow
        {
            Offset = new Point(-1, 0),
            Brush = Color.FromRgba("#000000"),
            Radius = 8,
            Opacity = 0.5f
        },
#if WINDOWS ||IOS
        WidthRequest = 280,
#else
        WidthRequest = 230,
#endif
    };
#endif

    private void OnSelectionChanged(object? sender, TabSelectionChangedEventArgs e)
    {
        if (e.OldIndex != e.NewIndex)
        {
            if (sender is View view && view.Parent is Grid grid)
            {
                if (Fill.IsSelected)
                {
                    view.HeightRequest = 200;
                    Fill.TextColor = Color.FromArgb("#6750A4");
                    Strokes.TextColor = Color.FromArgb("#49454F");
                }
                else if (Strokes.IsSelected)
                {
                    view.HeightRequest = 280;
                    Strokes.TextColor = Color.FromArgb("#6750A4");
                    Fill.TextColor = Color.FromArgb("#49454F");
                }
                int row = Grid.GetRow(grid);
                Grid.SetRow(view, row);
            }
        }
    }

    private void ColorFillButton_Clicked(object sender, EventArgs e)
    {
        if (PreButton != null)
        {
#if !WINDOWS
            PreButton.HeightRequest = 35;
            PreButton.WidthRequest = 35;
#if MACCATALYST
            PreButton.CornerRadius = 17;
#elif IOS
            PreButton.CornerRadius = 20;
#else
            PreButton.CornerRadius = 35;
#endif
#else
                    PreButton.HeightRequest = 40;
                    PreButton.WidthRequest = 40;
                    PreButton.CornerRadius = 40;
#endif
        }
        PreButton = sender as Button;
        if (selectedColorButtonHighlight == null)
        {
            selectedColorButtonHighlight = new Ellipse();
#if !WINDOWS
            selectedColorButtonHighlight.WidthRequest = 35;
            selectedColorButtonHighlight.HeightRequest = 35;
#else
            selectedColorButtonHighlight.WidthRequest = 40;
            selectedColorButtonHighlight.HeightRequest = 40;
#endif
            selectedColorButtonHighlight.VerticalOptions = LayoutOptions.Center;
            selectedColorButtonHighlight.HorizontalOptions = LayoutOptions.Center;
            selectedColorButtonHighlight.Stroke = Brush.Black;
            selectedColorButtonHighlight.StrokeThickness = 2;
        }
        if (sender is Button button)
        {
#if !WINDOWS
            button.HeightRequest = 35;
            button.WidthRequest = 35;
#if MACCATALYST
            button.CornerRadius = 17;
#elif IOS
            button.CornerRadius = 20;
#else
            button.CornerRadius = 35;
#endif
#else
                    button.HeightRequest = 40;
                    button.WidthRequest = 40;
                    button.CornerRadius = 40;
#endif
            button.HorizontalOptions = LayoutOptions.Center;
            button.VerticalOptions = LayoutOptions.Center;
            int column = Grid.GetColumn(button);
            int row = Grid.GetRow(button);
            Grid.SetColumn(selectedColorButtonHighlight, column);
            Grid.SetRow(selectedColorButtonHighlight, row);
#if WINDOWS 
            selectedColorButtonHighlight.Margin=new Thickness(0,0,0,0);
#else
            selectedColorButtonHighlight.Margin = new Thickness(0, 0, 2, 0);
#endif
            ShapeFillColorChanged?.Invoke(this, GetColor(button));
        }
        if (selectedColorButtonHighlight.Parent == null)
        {
            ColorFill.Children.Add(selectedColorButtonHighlight);
        }
    }

    private void ColorStrokeButton_Clicked(object sender, EventArgs e)
    {
        if (PreButton != null)
        {
#if !WINDOWS
            PreButton.HeightRequest = 35;
            PreButton.WidthRequest = 35;
#if MACCATALYST
            PreButton.CornerRadius = 17;
#elif IOS
            PreButton.CornerRadius = 20;
#else
            PreButton.CornerRadius = 35;
#endif
#else
            PreButton.HeightRequest = 40;
                    PreButton.WidthRequest = 40;
                    PreButton.CornerRadius = 40;
#endif
        }
        PreButton = sender as Button;
        if (selectedColorButtonHighlightStroke == null)
        {
            selectedColorButtonHighlightStroke = new Ellipse();
#if !WINDOWS
            selectedColorButtonHighlightStroke.WidthRequest = 35;
            selectedColorButtonHighlightStroke.HeightRequest = 35;
#else
            selectedColorButtonHighlightStroke.WidthRequest = 40;
            selectedColorButtonHighlightStroke.HeightRequest = 40;
#endif
            selectedColorButtonHighlightStroke.VerticalOptions = LayoutOptions.Center;
            selectedColorButtonHighlightStroke.HorizontalOptions = LayoutOptions.Center;
            selectedColorButtonHighlightStroke.Stroke = Brush.Black;
            selectedColorButtonHighlightStroke.StrokeThickness = 2;
        }
        if (sender is Button button)
        {
#if !WINDOWS
            button.HeightRequest = 35;
            button.WidthRequest = 35;
#if MACCATALYST
            button.CornerRadius = 17;
#elif IOS
            button.CornerRadius = 20;
#else
            button.CornerRadius = 35;
#endif
#else
                    button.HeightRequest = 40;
                    button.WidthRequest = 40;
                    button.CornerRadius = 40;
#endif
            button.HorizontalOptions = LayoutOptions.Center;
            button.VerticalOptions = LayoutOptions.Center;
            int column = Grid.GetColumn(button);
            int row = Grid.GetRow(button);
            Grid.SetColumn(selectedColorButtonHighlightStroke, column);
            Grid.SetRow(selectedColorButtonHighlightStroke, row);
#if WINDOWS 
            selectedColorButtonHighlightStroke.Margin=new Thickness(0,0,0,0);
#else
            selectedColorButtonHighlightStroke.Margin = new Thickness(0, 0, 2, 0);
#endif
            
            ShapeStrokeColorChanged?.Invoke(this, GetColor(button));
        }
        if (selectedColorButtonHighlightStroke.Parent == null)
        {
            ColorStroke.Children.Add(selectedColorButtonHighlightStroke);
        }
    }
    private Color GetColor(Button button)
    {
        Color buttonColor = Colors.Red;
            if(button==yellow || button==fillYellow)
            return Color.FromArgb("#F3F500");
            else if(button==green || button==fillGreen)
            return Color.FromArgb("#03FF0F");
            else if (button==aqua || button==fillAqua)
            return Color.FromArgb("#00FFEF");
            else if (button==blue || button==fillBlue)
            return Color.FromArgb("#1108FF");
            else if (button==lightPink || button==fillLightPink)
            return Color.FromArgb("#B900FF");
            else if (button==darkPink || button==fillDarkPink)
            return Color.FromArgb("#F500F3");

        return buttonColor;
    }
    private void ShapeStrokeOpacitySlidervalue_Chnaged(object sender, EventArgs e)
    {
        float opacity = (float)shapeStrokeOpacitySlider.Value;
        SelectedOpacity = opacity;
        StrokeOpacityChanged?.Invoke(this,opacity);
    }

    private void ShapeFillColorOpacitySlidervalue_Chnaged(object sender, EventArgs e)
    {
        float opacity = (float)shapeFillColorOpacitySlider.Value;
        SelectedFillColorOpacity = opacity;
        FillOpacityChanged?.Invoke(this, opacity);
    }

    private void SfSlider_ValueChangeEnd(object sender, EventArgs e)
    {
        float thickness = (float)ShapeStroke.Value;
        SelectedThickness = thickness;
        BorderThicknessChanged?.Invoke(this, thickness);
    }

}
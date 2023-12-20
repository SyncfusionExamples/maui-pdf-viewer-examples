using Microsoft.Maui.Controls.Shapes;
using Syncfusion.Maui.TabView;

namespace TextMarkups;

public partial class TextMarkupEditor : ContentView
{
    Ellipse? selectedColorButtonHighlight;
    Ellipse? selectedColorButtonHighlightStroke;
    Ellipse? selectedFontColorHighlight;

    internal float SelectedOpacity { get; set; } = 1;
    
    internal event EventHandler<Microsoft.Maui.Graphics.Color> ColorChanged;
    internal event EventHandler<float> OpacityChanged;

    Button? PreButton = null;
    public TextMarkupEditor()
	{
		InitializeComponent();
        Colorpaletteborder.Content = MyGrid;
        this.Content = Colorpaletteborder;
        tabView.SelectionChanged += OnSelectionChanged;
        this.PropertyChanged += FreeTextFillColorPalatte_PropertyChanged;
    }

    private void FreeTextFillColorPalatte_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsVisible))
        {
            if (IsVisible == false)
            {
                if (PreButton != null && selectedColorButtonHighlight != null)
                {
                    PreButton.HeightRequest = 40;
                    PreButton.WidthRequest = 40;
                    PreButton.CornerRadius = 40;
#if MACCATALYST || IOS
                    PreButton.CornerRadius = 17;
#endif
                    selectedColorButtonHighlight.Stroke = Brush.Transparent;
                    
                    selectedColorButtonHighlight = null;
                }
                if (PreButton != null && selectedColorButtonHighlightStroke != null)
                {
                    PreButton.HeightRequest = 40;
                    PreButton.WidthRequest = 40;
                    PreButton.CornerRadius = 40;
                    selectedColorButtonHighlightStroke.Stroke = Brush.Transparent;
                    if (selectedColorButtonHighlightStroke.Parent != null)
                    {
                        ColorStroke.Children.Remove(selectedColorButtonHighlightStroke);
                    }
                    selectedColorButtonHighlightStroke = null;
                }
                if (PreButton != null && selectedFontColorHighlight != null)
                {
                    PreButton.HeightRequest = 40;
                    PreButton.WidthRequest = 40;
                    PreButton.CornerRadius = 40;
#if MACCATALYST || IOS
                    PreButton.CornerRadius = 17;
#endif
                    selectedFontColorHighlight.Stroke = Brush.Transparent;
                   
                    selectedFontColorHighlight = null;
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
        WidthRequest = 280,
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
        WidthRequest = 280,
    };
#endif

    private void OnSelectionChanged(object? sender, TabSelectionChangedEventArgs e)
    {
        if (e.OldIndex != e.NewIndex)
        {
            if (sender is View view && view.Parent is Grid grid)
            {
                if (Stroke.IsSelected)
                {
                    view.HeightRequest = 280;
                    Stroke.TextColor = Color.FromArgb("#6750A4");
                }
                int row = Grid.GetRow(grid);
                Grid.SetRow(view, row);
            }
        }
    }

    private Color GetColor(Button button)
    {
        Color buttonColor = Colors.Red;
        if (button == yellow )
            return Color.FromArgb("#F3F500");
        else if (button == green )
            return Color.FromArgb("#03FF0F");
        else if (button == aqua )
            return Color.FromArgb("#00FFEF");
        else if (button == blue )
            return Color.FromArgb("#1108FF");
        else if (button == lightPink )
            return Color.FromArgb("#B900FF");
        else if (button == darkPink )
            return Color.FromArgb("#F500F3");

        return buttonColor;
    }

    private void ColorStrokeButton_Clicked(object sender, EventArgs e)
    {
        if (PreButton != null)
        {
            PreButton.HeightRequest = 40;
            PreButton.WidthRequest = 40;
            PreButton.CornerRadius = 40;
#if MACCATALYST || IOS
            PreButton.CornerRadius = 17;
#endif
        }
        PreButton = sender as Button;
        if (selectedColorButtonHighlightStroke == null)
        {
            selectedColorButtonHighlightStroke = new Ellipse();
            selectedColorButtonHighlightStroke.WidthRequest = 40;
            selectedColorButtonHighlightStroke.HeightRequest = 40;
            selectedColorButtonHighlightStroke.VerticalOptions = LayoutOptions.Center;
            selectedColorButtonHighlightStroke.HorizontalOptions = LayoutOptions.Center;
            selectedColorButtonHighlightStroke.Stroke = Brush.Black;
            selectedColorButtonHighlightStroke.StrokeThickness = 2;
        }
        if (sender is Button button)
        {
            button.HeightRequest = 40;
            button.WidthRequest = 40;
            button.CornerRadius = 40;
#if MACCATALYST || IOS
            PreButton.CornerRadius = 17;
#endif
            button.HorizontalOptions = LayoutOptions.Center;
            button.VerticalOptions = LayoutOptions.Center;
            int column = Grid.GetColumn(button);
            int row = Grid.GetRow(button);
            Grid.SetColumn(selectedColorButtonHighlightStroke, column);
            Grid.SetRow(selectedColorButtonHighlightStroke, row);
            ColorChanged.Invoke(this, GetColor(button));
        }
        if (selectedColorButtonHighlightStroke.Parent == null)
        {
            ColorStroke.Children.Add(selectedColorButtonHighlightStroke);
        }
    }
    private void ShapeStrokeOpacitySlidervalue_Chnaged(object sender, EventArgs e)
    {
        float opacity = (float)shapeStrokeOpacitySlider.Value;
        SelectedOpacity = opacity;
        OpacityChanged.Invoke(this,opacity);
    }
}
using Microsoft.Maui.Controls.Shapes;
using Syncfusion.Maui.TabView;

namespace FreeText;

public partial class FreeTextEditor : ContentView
{
    Ellipse? selectedColorButtonHighlight;
    Ellipse? selectedColorButtonHighlightStroke;
    Ellipse? selectedFontColorHighlight;

    internal float SelectedOpacity { get; set; } = 1;
    internal float SelectedFontSize { get; set; } = 12;
    internal float SelectedThickness { get; set; } = 1;

    internal event EventHandler<Microsoft.Maui.Graphics.Color> FillColorChanged;
    internal event EventHandler<Microsoft.Maui.Graphics.Color> FontColorChanged;
    internal event EventHandler<Microsoft.Maui.Graphics.Color> BorderColorChanged;
    internal event EventHandler<float> BorderThicknessChanged;
    internal event EventHandler<float> OpacityChanged;
    internal event EventHandler<double> FontSizeChanged;

    Button? PreButton = null;
    public FreeTextEditor()
	{
		InitializeComponent();
        Colorpaletteborder.Content = MyGrid;
        this.Content = Colorpaletteborder;
        tabView.SelectionChanged += OnSelectionChanged;
        tabView.LayoutChanged += TabView_LayoutChanged;
        this.PropertyChanged += FreeTextFillColorPalatte_PropertyChanged;
    }

    private void TabView_LayoutChanged(object? sender, EventArgs e)
    {
        if (sender is View view && view.Parent is Grid grid)
        {
            if (Text.IsSelected)
            {
                Fill.TextColor = Color.FromArgb("#49454F");
                Stroke.TextColor = Color.FromArgb("#49454F");
                Text.TextColor = Color.FromArgb("#6750A4");
            }
        }
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
                    if (selectedColorButtonHighlight.Parent != null)
                    {
                        ColorFill.Children.Remove(selectedColorButtonHighlight);
                    }
                    selectedColorButtonHighlight = null;
                }
                if (PreButton != null && selectedColorButtonHighlightStroke != null)
                {
                    PreButton.HeightRequest = 40;
                    PreButton.WidthRequest = 40;
                    PreButton.CornerRadius = 40;
#if MACCATALYST || IOS
                    PreButton.CornerRadius = 17;
#endif
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
                    if (selectedFontColorHighlight.Parent != null)
                    {
                        FontColor.Children.Remove(selectedFontColorHighlight);
                    }
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
                if (Fill.IsSelected)
                {
                    view.HeightRequest = 200;
                    Fill.TextColor = Color.FromArgb("#6750A4");
                    Stroke.TextColor = Color.FromArgb("#49454F");
                    Text.TextColor = Color.FromArgb("#49454F");
                }
                else if (Stroke.IsSelected)
                {
                    view.HeightRequest = 280;
                    Stroke.TextColor = Color.FromArgb("#6750A4");
                    Fill.TextColor = Color.FromArgb("#49454F");
                    Text.TextColor = Color.FromArgb("#49454F");
                }
                else if (Text.IsSelected)
                {
                    view.HeightRequest = 200;
                    Fill.TextColor = Color.FromArgb("#49454F");
                    Stroke.TextColor = Color.FromArgb("#49454F");
                    Text.TextColor = Color.FromArgb("#6750A4");
                }
                int row = Grid.GetRow(grid);
                Grid.SetRow(view, row);
            }
        }
    }

    private Color GetColor(Button button)
    {
        Color buttonColor = Colors.Red;
        if (button == yellow || button == fillYellow || button == borYellow)
            return Color.FromArgb("#F3F500");
        else if (button == green || button == fillGreen || button == borGreen)
            return Color.FromArgb("#03FF0F");
        else if (button == aqua || button == fillAqua || button == borLightBlue)
            return Color.FromArgb("#00FFEF");
        else if (button == blue || button == fillBlue || button == borBlue)
            return Color.FromArgb("#1108FF");
        else if (button == lightPink || button == fillLightPink || button == borViolet)
            return Color.FromArgb("#B900FF");
        else if (button == darkPink || button == fillDarkPink || button == borPink)
            return Color.FromArgb("#F500F3");

        return buttonColor;
    }
    private void ColorFillButton_Clicked(object sender, EventArgs e)
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
        if (selectedColorButtonHighlight == null)
        {
            selectedColorButtonHighlight = new Ellipse();
            selectedColorButtonHighlight.WidthRequest = 40;
            selectedColorButtonHighlight.HeightRequest = 40;
            selectedColorButtonHighlight.VerticalOptions = LayoutOptions.Center;
            selectedColorButtonHighlight.HorizontalOptions = LayoutOptions.Center;
            selectedColorButtonHighlight.Stroke = Brush.Black;
            selectedColorButtonHighlight.StrokeThickness = 2;
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
            Grid.SetColumn(selectedColorButtonHighlight, column);
            Grid.SetRow(selectedColorButtonHighlight, row);
            FillColorChanged.Invoke(this, GetColor(button));
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
            BorderColorChanged.Invoke(this, GetColor(button));
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

    private void ShapeFillColorOpacitySlidervalue_Chnaged(object sender, EventArgs e)
    {
        float opacity = (float)shapeFillColorOpacitySlider.Value;
        SelectedOpacity = opacity;
        OpacityChanged.Invoke(this, opacity);
    }

    private void SfSlider_ValueChangeEnd(object sender, EventArgs e)
    {
        float thickness = (float)FreetextStroke.Value;
        SelectedThickness = thickness;
        BorderThicknessChanged.Invoke(this, thickness);
    }

    private void FontSizeSliderValueChanged(object sender, EventArgs e)
    {
        SelectedFontSize = (float)textSize.Value;
        FontSizeChanged.Invoke(this, textSize.Value);
    }

    private void FontColorButton_Clicked(object sender, EventArgs e)
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
        if (selectedFontColorHighlight == null)
        {
            selectedFontColorHighlight = new Ellipse();
            selectedFontColorHighlight.WidthRequest = 40;
            selectedFontColorHighlight.HeightRequest = 40;
            selectedFontColorHighlight.VerticalOptions = LayoutOptions.Center;
            selectedFontColorHighlight.HorizontalOptions = LayoutOptions.Center;
            selectedFontColorHighlight.Stroke = Brush.Black;
            selectedFontColorHighlight.StrokeThickness = 2;
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
            Grid.SetColumn(selectedFontColorHighlight, column);
            Grid.SetRow(selectedFontColorHighlight, row);
            FontColorChanged.Invoke(this, GetColor(button));
        }
        if (selectedFontColorHighlight.Parent == null)
        {
            FontColor.Children.Add(selectedFontColorHighlight);
        }
    }
    }
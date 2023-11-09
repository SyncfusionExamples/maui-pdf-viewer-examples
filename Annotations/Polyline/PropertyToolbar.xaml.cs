namespace PolylineAnnotationDemo
{
    public partial class PropertyToolbar : ContentView
    {
        List<string> colorStrings;
        public PropertyToolbar()
        {
            InitializeComponent();
            colorStrings = new List<string>()
        {
                "----",
            "Red",
            "Green",
            "Blue",
            "Pink",
            "Beize",
            "Yellow"
        };
            colorPicker.ItemsSource = colorStrings;
            colorPicker.SelectedIndex = 0;
            colorPicker.WidthRequest = 150 ;
           
        }

        private void ValueChanged(object sender, ValueChangedEventArgs e)
        {

        }
    }
}


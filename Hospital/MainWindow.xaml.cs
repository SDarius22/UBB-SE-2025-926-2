namespace Hospital
{
    using Microsoft.UI.Xaml;
    using Hospital.Views;

    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            var loginWindow = new LoginPage();
            loginWindow.Activate();

            this.Close();
        }
    }
}
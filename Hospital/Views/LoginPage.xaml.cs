namespace Hospital.Views
{
    using Hospital.ApiClients;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.UI.Xaml;

    /// <summary>
    /// LoginPage class.
    /// </summary>
    public sealed partial class LoginPage : global::Microsoft.UI.Xaml.Window
    {
        private readonly UserApiService userApiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPage"/> class.
        /// </summary>
        public LoginPage()
        {
            this.userApiService = App.Services.GetRequiredService<UserApiService>();
            this.InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string token = await this.userApiService.Login(this.Username.Text, this.Password.Password);

            if (token == null)
            {
                this.errorMessage.Text = "Invalid username or password.";
                this.errorMessage.Visibility = Visibility.Visible;
                return;
            }


            this.errorMessage.Visibility = Visibility.Collapsed;

            App.Token = token;
            // Navigate back to the main page
            var adminMainPage = new AdminMainPage();
            adminMainPage.Activate();
            this.Close();
        }
    }
}
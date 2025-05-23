namespace Hospital.Views
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Navigation;
    using Hospital.Models;
    using Hospital.ViewModels;
    

    /// <summary>
    /// DoctorInfoPage class.
    /// </summary>
    public sealed partial class DoctorInfoPage : Page
    {
        private DoctorInformationViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorInfoPage"/> class.
        /// </summary>
        public DoctorInfoPage()
        {
            this.InitializeComponent();
            this.viewModel = new DoctorInformationViewModel();
            this.DataContext = this.viewModel;
        }

        /// <summary>
        /// Handles the navigatedTo event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is DoctorJointModel doctor)
            {
                int doctorID = doctor.DoctorId;
                this.viewModel.LoadDoctorInformation(doctorID);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
    }
}
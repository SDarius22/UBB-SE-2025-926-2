using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hospital.ViewModels.AddViewModels;


// To learn more about WinUI, the WinUI Hospital structure,
// and more about our Hospital templates, see: http://aka.ms/winui-Hospital-info.

namespace Hospital.Views.AddViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddDoctorView : Page
    {
        private DoctorAddViewModel _viewModel;
        public AddDoctorView()
        {
            this.InitializeComponent();
            _viewModel = new DoctorAddViewModel();
            this.DataContext = _viewModel;
        }
    }
}

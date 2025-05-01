using Microsoft.UI.Xaml.Controls;
using Hospital.ViewModels.AddViewModels;

// To learn more about WinUI, the WinUI Hospital structure,
// and more about our Hospital templates, see: http://aka.ms/winui-Hospital-info.

namespace Hospital.Views.AddViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddDrugView : Page
    {
        private DrugAddViewModel _viewModel;
        public AddDrugView()
        {
            this.InitializeComponent();
            _viewModel = new DrugAddViewModel();
            this.DataContext = _viewModel;
        }
    }
}

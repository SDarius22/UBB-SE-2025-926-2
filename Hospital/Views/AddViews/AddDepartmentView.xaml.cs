namespace Hospital.Views.AddViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.ViewModels.AddViewModels;
    using Hospital.DatabaseServices;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddDepartmentView : Page
    {
        private DepartmentAddViewModel viewModel;
        private IDepartmentsDatabaseService _departmentModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddDepartmentView"/> class.
        /// </summary>
        public AddDepartmentView()
        {
            this.InitializeComponent();
            this.viewModel = new DepartmentAddViewModel(_departmentModel);
            this.DataContext = this.viewModel;
        }
    }
}

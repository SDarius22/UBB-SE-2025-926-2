namespace Hospital.Views.DeleteViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.ViewModels.DeleteViewModels;
    using Hospital.DatabaseServices;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeleteDepartmentView : Page
    {
        private DepartmentDeleteViewModel viewModel;
        private IDepartmentsDatabaseService _departmentModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteDepartmentView"/> class.
        /// </summary>
        public DeleteDepartmentView()
        {
            this.InitializeComponent();
            this.viewModel = new DepartmentDeleteViewModel(_departmentModel);
            this.DataContext = this.viewModel;
        }
    }
}

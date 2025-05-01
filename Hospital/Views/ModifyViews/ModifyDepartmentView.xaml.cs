namespace Hospital.Views.ModifyViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.ViewModels.UpdateViewModels;
    using Hospital.DatabaseServices;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModifyDepartmentView : Page
    {
        private DepartmentUpdateViewModel viewModel;
        private IDepartmentsDatabaseService _departmentModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyDepartmentView"/> class.
        /// </summary>
        public ModifyDepartmentView()
        {
            this.InitializeComponent();
            this.viewModel = new DepartmentUpdateViewModel(_departmentModel);
            this.DataContext = this.viewModel;
        }
    }
}
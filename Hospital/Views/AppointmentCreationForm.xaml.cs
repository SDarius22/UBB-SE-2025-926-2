using System;
using Hospital.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using System.Linq;
using Hospital.DatabaseServices;
using Hospital.Managers;
using System.Diagnostics;

namespace Hospital.Views
{
    public sealed partial class AppointmentCreationForm : Page
    {
        private AppointmentCreationFormViewModel _viewModel;

        public AppointmentCreationForm()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is not AppointmentCreationFormViewModel viewModel)
                throw new InvalidOperationException("AppointmentCreationForm requires a ViewModel.");

            _viewModel = viewModel;

            AppointmentForm.DataContext = _viewModel;
            _viewModel.Root = this.Content.XamlRoot;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _viewModel.BookAppointment();
                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            }
            catch (Exception ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        private async Task ShowErrorDialog(string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await errorDialog.ShowAsync();
        }

        private async void ProcedureComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                await _viewModel.LoadAvailableTimeSlots();
            }
            catch (Exception ex)
            {
                await ShowErrorDialog(ex.Message);
            }
        }

        private async void DoctorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                await _viewModel.LoadDoctorSchedule();
                await _viewModel.LoadAvailableTimeSlots();

                CalendarDatePicker.Date = null;
                CalendarDatePicker.MinDate = _viewModel.MinimumDate;
                CalendarDatePicker.MaxDate = _viewModel.MaximumDate;
            }
            catch (Exception ex)
            {
                await ShowErrorDialog(ex.Message);
            }
        }

        private async void DepartmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                await _viewModel.LoadProceduresAndDoctorsOfSelectedDepartment();
            }
            catch (Exception ex)
            {
                await ShowErrorDialog(ex.Message);
            }
        }

        private void CalendarView_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            if (_viewModel.HighlightedDates == null)
                return;

            DateTimeOffset date = args.Item.Date.Date;
            if (_viewModel.HighlightedDates.Any(d => d.Date == date))
            {
                args.Item.Background = new SolidColorBrush(Microsoft.UI.Colors.LightGreen);
                args.Item.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Black);
            }
        }
    }
}
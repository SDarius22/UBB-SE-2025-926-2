using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Hospital.Managers;
using Hospital.ViewModels;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Popups;
using System.Security.AccessControl;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hospital.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentCreationForm : Window
    {
        private AppointmentCreationFormViewModel _viewModel;

        private AppointmentCreationForm(AppointmentCreationFormViewModel viewModel)
        {
            this.InitializeComponent();
            this.StyleTitleBar();
            _viewModel = viewModel;
            AppointmentForm.DataContext = _viewModel;
            _viewModel.Root = this.Content.XamlRoot;
            this.AppWindow.Resize(new(1000, 1400));
        }



        public static Task<AppointmentCreationForm> CreateAppointmentCreationForm(
            AppointmentCreationFormViewModel viewModel)
        {
            return Task.FromResult(new AppointmentCreationForm(viewModel));
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _viewModel.BookAppointment();
                this.Close();
            }
            catch (Exception ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        //this method is used to style the title bar of the window
        private void StyleTitleBar()
        {
            // Get the title bar of the app window.
            AppWindow m_Window = this.AppWindow;
            AppWindowTitleBar m_TitleBar = m_Window.TitleBar;

            // Set title bar colors.
            m_TitleBar.ForegroundColor = Colors.White;
            m_TitleBar.BackgroundColor = Colors.Green;

            // Set button colors.
            m_TitleBar.ButtonForegroundColor = Colors.White;
            m_TitleBar.ButtonBackgroundColor = Colors.SeaGreen;

            // Set button hover colors.
            m_TitleBar.ButtonHoverForegroundColor = Colors.Gainsboro;
            m_TitleBar.ButtonHoverBackgroundColor = Colors.DarkSeaGreen;
            m_TitleBar.ButtonPressedForegroundColor = Colors.Gray;
            m_TitleBar.ButtonPressedBackgroundColor = Colors.LightGreen;

            // Set inactive window colors.
            // Note: No effect when app is running on Windows 10
            // because color customization is not supported.
            m_TitleBar.InactiveForegroundColor = Colors.Gainsboro;
            m_TitleBar.InactiveBackgroundColor = Colors.SeaGreen;
            m_TitleBar.ButtonInactiveForegroundColor = Colors.Gainsboro;
            m_TitleBar.ButtonInactiveBackgroundColor = Colors.SeaGreen;
        }

        private async void DepartmentComboBox_SelectionChanged(object sender, object e)
        {
            try
            {
                await _viewModel.LoadProceduresAndDoctorsOfSelectedDepartment();
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                };
                errorDialog.XamlRoot = this.Content.XamlRoot;
                await errorDialog.ShowAsync();
            }
        }

        private async void ProcedureComboBox_SelectionChanged(object sender, object e)
        {
            try
            {
                await _viewModel.LoadAvailableTimeSlots();
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                };
                errorDialog.XamlRoot = this.Content.XamlRoot;
                await errorDialog.ShowAsync();
            }
        }

        private async void DoctorComboBox_SelectionChanged(object sender, object e)
        {
            try
            {
                await _viewModel.LoadDoctorSchedule();
                await _viewModel.LoadAvailableTimeSlots();

                // Reset the calendar date picker
                CalendarDatePicker.Date = null;
                CalendarDatePicker.MinDate = _viewModel.MinimumDate;
                CalendarDatePicker.MaxDate = _viewModel.MaximumDate;
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                };
                errorDialog.XamlRoot = this.Content.XamlRoot;
                await errorDialog.ShowAsync();
            }
        }

        private void CalendarView_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            DateTimeOffset date = args.Item.Date.Date;
            if (_viewModel.HighlightedDates.Any(d => d.Date == date))
            {
                args.Item.Background = new SolidColorBrush(Microsoft.UI.Colors.LightGreen); // Highlight date
                args.Item.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Black);      // Ensure text is readable
            }
        }

        private async void ShowErrorDialog(string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK"
            };
            errorDialog.XamlRoot = this.Content.XamlRoot;
            await errorDialog.ShowAsync();
        }
    }
}

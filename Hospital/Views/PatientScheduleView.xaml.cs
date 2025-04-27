using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Hospital.Managers;
using Hospital.Models;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using System.Collections.Generic;
using Microsoft.UI;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Hospital.ViewModels;

namespace Hospital.Views
{
    public sealed partial class PatientScheduleView : Window
    {
        private readonly PatientScheduleViewModel _viewModel;
        private readonly DispatcherQueue _dispatcherQueue;

        public PatientScheduleView()
        {
            this.ExtendsContentIntoTitleBar = false;
            this.InitializeComponent();

            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            _viewModel = new PatientScheduleViewModel(new AppointmentManager(new DatabaseServices.AppointmentsDatabaseService()));

            DailyScheduleList.ItemsSource = _viewModel.DailyAppointments;
            AppointmentsCalendar.CalendarViewDayItemChanging += CalendarView_DayItemChanging;

            AppointmentsCalendar.MinDate = _viewModel.MinDate;
            AppointmentsCalendar.MaxDate = _viewModel.MaxDate;

            LoadAppointmentsAndUpdateUI();
        }

        private async void LoadAppointmentsAndUpdateUI()
        {
            await _viewModel.LoadAppointmentsForPatient(1); // can be changed to the current patient
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshAppointments();
        }

        private void AppointmentsCalendar_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (args.AddedDates.Count > 0)
            {
                DateTime selectedDate = args.AddedDates[0].DateTime.Date;
                _viewModel.UpdateDailySchedule(selectedDate);
                NoAppointmentsText.Visibility = _viewModel.HasAppointmentsOnDate(selectedDate) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void CalendarView_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            var date = args.Item.Date.Date;
            if (_viewModel.HighlightedDates.Any(a => a.Date == date))
            {
                args.Item.Background = new SolidColorBrush(Colors.LightGreen);
            }
        }

        private async void DailyScheduleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedSlot = (TimeSlotModel)e.AddedItems[0];
                DailyScheduleList.SelectedItem = null;

                var selectedDate = AppointmentsCalendar.SelectedDates.FirstOrDefault().DateTime.Date;
                var selectedAppointment = _viewModel.GetAppointmentForTimeSlot(selectedSlot, selectedDate);

                if (selectedAppointment != null)
                {
                    await ShowAppointmentDetailsDialog(selectedAppointment);
                }
            }
        }

        private async Task ShowAppointmentDetailsDialog(AppointmentJointModel appointment)
        {
            string message = $"Date and Time: {appointment.DateAndTime:f}\n" +
                           $"Doctor: {appointment.DoctorName}\n" +
                           $"Department: {appointment.DepartmentName}\n" +
                           $"Procedure: {appointment.ProcedureName}\n" +
                           $"Procedure Duration: {appointment.ProcedureDuration.TotalMinutes} minutes";

            ContentDialog dialog = new ContentDialog
            {
                Title = "Appointment Details",
                CloseButtonText = "Close",
                XamlRoot = this.Content.XamlRoot,
                RequestedTheme = ElementTheme.Default
            };

            StackPanel dialogContent = new StackPanel { Spacing = 10 };
            dialogContent.Children.Add(new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap
            });

            bool canCancel = _viewModel.CanCancelAppointment(appointment);
            StackPanel buttonRow = CreateAppointmentButtonPanel(dialog, appointment, canCancel);
            dialogContent.Children.Add(buttonRow);
            dialog.Content = dialogContent;

            await dialog.ShowAsync();
        }

        private StackPanel CreateAppointmentButtonPanel(ContentDialog dialog, AppointmentJointModel appointment, bool canCancel)
        {
            var buttonRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Spacing = 10,
                Margin = new Thickness(0, 10, 0, 0)
            };

            if (canCancel)
            {
                Button cancelBtn = new Button
                {
                    Content = "Cancel Appointment",
                    Width = 160,
                    Height = 40,
                    Background = new SolidColorBrush(Colors.Red),
                    Foreground = new SolidColorBrush(Colors.White)
                };
                cancelBtn.Click += async (s, e) => await HandleAppointmentCancellation(dialog, appointment);
                buttonRow.Children.Add(cancelBtn);
            }
            else
            {
                Button disabledBtn = new Button
                {
                    Content = "Cancel Appointment",
                    Width = 160,
                    Height = 40,
                    IsEnabled = false,
                    Background = new SolidColorBrush(Color.FromArgb(255, 255, 102, 102)),
                    Foreground = new SolidColorBrush(Colors.White)
                };
                ToolTipService.SetToolTip(disabledBtn, "You can only cancel appointments more than 24 hours in advance.");
                buttonRow.Children.Add(disabledBtn);
            }

            return buttonRow;
        }

        private async Task HandleAppointmentCancellation(ContentDialog dialog, AppointmentJointModel appointment)
        {
            dialog.Hide();
            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Confirm Cancellation",
                Content = "Are you sure you want to cancel this appointment?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot
            };

            var result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    await _viewModel.CancelAppointment(appointment);
                    if (AppointmentsCalendar.SelectedDates.Any())
                    {
                        var selectedDate = AppointmentsCalendar.SelectedDates.First().DateTime.Date;
                        _viewModel.UpdateDailySchedule(selectedDate);
                        NoAppointmentsText.Visibility = _viewModel.HasAppointmentsOnDate(selectedDate) ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Cancellation Failed",
                        Content = ex.Message,
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        private async void RefreshAppointments()
        {
            try
            {
                AppointmentsCalendar.CalendarViewDayItemChanging -= CalendarView_DayItemChanging;
                AppointmentsCalendar.SelectedDatesChanged -= AppointmentsCalendar_SelectedDatesChanged;

                AppointmentsCalendar.MinDate = _viewModel.MinDate;
                AppointmentsCalendar.MaxDate = _viewModel.MaxDate;
                AppointmentsCalendar.SelectionMode = CalendarViewSelectionMode.Single;
                AppointmentsCalendar.BorderBrush = new SolidColorBrush(Colors.Green);
                AppointmentsCalendar.BorderThickness = new Thickness(2);

                AppointmentsCalendar.CalendarViewDayItemChanging += CalendarView_DayItemChanging;
                AppointmentsCalendar.SelectedDatesChanged += AppointmentsCalendar_SelectedDatesChanged;

                await _viewModel.LoadAppointmentsForPatient(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error refreshing calendar: " + ex.Message);
            }
        }
    }
}

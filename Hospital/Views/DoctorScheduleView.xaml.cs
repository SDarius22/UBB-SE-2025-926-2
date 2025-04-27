using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hospital.Managers;
using Hospital.ViewModels;
using System;
using Microsoft.UI;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Hospital.Models;
using Microsoft.UI.Xaml.Input;

namespace Hospital.Views
{
    public sealed partial class DoctorScheduleView : Window
    {
        public DoctorScheduleViewModel ViewModel => _viewModel;
        private readonly DoctorScheduleViewModel _viewModel;
        public ObservableCollection<TimeSlotModel> _dailySchedule { get; private set; }

        private IMedicalRecordManager _medicalRecordManager;
        private IDocumentManager _documentManager;

        public DoctorScheduleView(
            IAppointmentManager appointmentManagerModel,
            IShiftManager shiftManagerModel,
            IMedicalRecordManager medicalRecordManager,
            IDocumentManager documentManager)
        {
            _viewModel = new DoctorScheduleViewModel(appointmentManagerModel, shiftManagerModel);
            _medicalRecordManager = medicalRecordManager;
            _documentManager = documentManager;
            _dailySchedule = new ObservableCollection<TimeSlotModel>();

            this.InitializeComponent();
            ((FrameworkElement)this.Content).DataContext = _viewModel;
            DailyScheduleList.ItemsSource = _dailySchedule;
            DoctorSchedule.CalendarViewDayItemChanging += CalendarView_DayItemChanging;
            LoadShiftsAndRefreshCalendar();
        }

        private async void LoadShiftsAndRefreshCalendar()
        {
            try
            {
                await _viewModel.LoadShiftsForDoctor();

                if (_viewModel.Shifts == null || !_viewModel.Shifts.Any()) return;

                DoctorSchedule.SelectedDates.Clear();
                DoctorSchedule.InvalidateArrange();
                DoctorSchedule.InvalidateMeasure();
                DoctorSchedule.UpdateLayout();

                await RecreateCalendarView();
            }
            catch (Exception e)
            {
                await ShowErrorDialog($"Failed to load doctor shifts.\n\n{e.Message}");
            }
        }

        private async Task RecreateCalendarView()
        {
            try
            {
                CalendarView newCalendar = new CalendarView
                {
                    MinDate = _viewModel.MinimumDateForSelectingAppointment.DateTime,
                    MaxDate = _viewModel.MaximumDateForSelectingAppointment.DateTime,
                    SelectionMode = CalendarViewSelectionMode.Single,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top,
                    BorderBrush = new SolidColorBrush(Colors.Green),
                    BorderThickness = new Thickness(2)
                };

                newCalendar.CalendarViewDayItemChanging += CalendarView_DayItemChanging;
                newCalendar.SelectedDatesChanged += CalendarView_SelectedDatesChanged;

                CalendarContainer.Children.Remove(DoctorSchedule);
                DoctorSchedule = newCalendar;
                CalendarContainer.Children.Insert(0, DoctorSchedule);
            }
            catch (Exception ex)
            {
                await ShowErrorDialog("Calendar failed to reload.");
            }
        }

        private async void CalendarView_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            try
            {
                if (args.AddedDates.Count > 0)
                {
                    DateTime selectedDate = args.AddedDates[0].DateTime.Date;
                    await _viewModel.OnDateSelected(selectedDate);
                }
            }
            catch (Exception e)
            {
                await ShowErrorDialog($"Error selecting date: {e.Message}");
            }
        }

        private void CalendarView_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            if (_viewModel.ShiftDates == null || !_viewModel.ShiftDates.Any()) return;
            var date = args.Item.Date.Date;

            if (_viewModel.ShiftDates.Any(d => d.Date == date.Date))
            {
                args.Item.Background = new SolidColorBrush(Colors.LightGreen);
            }
        }

        private async void TimeSlot_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is StackPanel panel && panel.DataContext is TimeSlotModel slot)
            {
                if (_viewModel.OpenDetailsCommand?.CanExecute(slot) == true)
                {
                    _viewModel.OpenDetailsCommand.Execute(slot);
                    var selected = _viewModel.SelectedSlot;
                    if (selected != null)
                    {
                        await ShowAppointmentDetails(selected);
                    }
                }
            }
        }

        private async Task ShowAppointmentDetails(TimeSlotModel slot)
        {
            if (!string.IsNullOrEmpty(slot.Appointment))
            {
                var appointment = _viewModel.Appointments.FirstOrDefault(a => a.ProcedureName == slot.Appointment);
                if (appointment != null)
                {
                    await ShowAppointmentDialog(appointment);
                }
            }
            else if (slot.HighlightStatus == "Available")
            {
                await ShowEmptySlotDialog(slot);
            }
        }

        private async Task ShowAppointmentDialog(AppointmentJointModel appointment)
        {
            var message = $"Appointment: {appointment.ProcedureName}\n" +
                         $"Date: {appointment.DateAndTime}\n" +
                         $"Doctor: {appointment.DoctorName}\n" +
                         $"Patient: {appointment.PatientName}\n";

            var dialog = new ContentDialog
            {
                Title = "Appointment Info",
                XamlRoot = this.Content.XamlRoot,
                RequestedTheme = ElementTheme.Default
            };

            var dialogContent = new StackPanel();
            dialogContent.Children.Add(new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20)
            });

            var buttonPanel = CreateAppointmentButtonPanel(appointment, dialog);
            dialogContent.Children.Add(buttonPanel);
            dialog.Content = dialogContent;
            dialog.CloseButtonText = "Close";

            await dialog.ShowAsync();
        }

        private StackPanel CreateAppointmentButtonPanel(AppointmentJointModel appointment, ContentDialog dialog)
        {
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Spacing = 10
            };

            var createRecordBtn = new Button { Content = "Create Medical Record" };
            createRecordBtn.Click += (s, args) =>
            {
                dialog.Hide();
                ShowMedicalRecordForm(appointment);
            };

            var viewHistoryBtn = new Button { Content = "Medical Records History" };
            viewHistoryBtn.Click += (s, args) =>
            {
                dialog.Hide();
                ShowMedicalRecordsHistory(appointment.PatientId);
            };

            buttonPanel.Children.Add(createRecordBtn);
            buttonPanel.Children.Add(viewHistoryBtn);

            return buttonPanel;
        }

        private void ShowMedicalRecordForm(AppointmentJointModel appointment)
        {
            var viewModel = new MedicalRecordCreationFormViewModel(_medicalRecordManager, _documentManager);
            var medicalRecordCreateView = new CreateMedicalRecordForm(viewModel, appointment);
            medicalRecordCreateView.Activate();
        }

        private void ShowMedicalRecordsHistory(int patientId)
        {
            var recordsHistoryView = new MedicalRecordsHistoryView(patientId, _medicalRecordManager, _documentManager);
            recordsHistoryView.Activate();
        }

        private async Task ShowEmptySlotDialog(TimeSlotModel slot)
        {
            var dialog = new ContentDialog
            {
                Title = "Appointment Info",
                Content = $"No appointments scheduled in this shift slot.\nTime: {slot.Time}",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot,
                RequestedTheme = ElementTheme.Default
            };

            await dialog.ShowAsync();
        }

        private async Task ShowErrorDialog(string message)
        {
            try
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = message,
                    CloseButtonText = "OK",
                    RequestedTheme = ElementTheme.Default
                };

                if (this.Content is FrameworkElement rootElement)
                {
                    errorDialog.XamlRoot = rootElement.XamlRoot;
                }
                else
                {
                    Console.WriteLine("Error: Unable to find a valid XamlRoot.");
                    return;
                }

                await errorDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical error while showing error dialog: {ex.Message}");
            }
        }

        private void DailyScheduleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // This method is required by the XAML but we don't need any functionality here
            return;
        }
    }
}
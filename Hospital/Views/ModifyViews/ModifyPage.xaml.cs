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

// To learn more about WinUI, the WinUI Hospital structure,
// and more about our Hospital templates, see: http://aka.ms/winui-Hospital-info.

namespace Hospital.Views.ModifyViews
{
    /// <summary>
    /// Code-behind for the ModifyPage, handling navigation between different modification views.
    /// </summary>
    public sealed partial class ModifyPage : Page
    {
        public ModifyPage()
        {
            this.InitializeComponent();
            ModifyPageFrame.Navigate(typeof(ModifyDoctorView)); // Default view
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                string invokedItemName = args.SelectedItemContainer.Tag.ToString();
                switch (invokedItemName)
                {
                    case "Doctors":
                        ModifyPageFrame.Navigate(typeof(ModifyDoctorView));
                        break;
                    case "Equipment":
                        ModifyPageFrame.Navigate(typeof(ModifyEquipmentView));
                        break;
                    case "Drugs":
                        ModifyPageFrame.Navigate(typeof(ModifyDrugView));
                        break;
                    case "Schedule":
                        ModifyPageFrame.Navigate(typeof(ModifyScheduleAndShifts));
                        break;
                    case "Departments":
                        ModifyPageFrame.Navigate(typeof(ModifyRoomAndDepartments));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

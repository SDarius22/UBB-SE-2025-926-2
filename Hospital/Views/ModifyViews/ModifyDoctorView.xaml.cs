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
using Hospital.ViewModel;
using Hospital.Models;




// To learn more about WinUI, the WinUI Hospital structure,
// and more about our Hospital templates, see: http://aka.ms/winui-Hospital-info.

namespace Hospital.Views.ModifyViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModifyDoctorView : Page
    {
        private DoctorUpdateViewModel _viewModel;
        public ModifyDoctorView()
        {
            this.InitializeComponent();
            _viewModel = new DoctorUpdateViewModel();
            this.DataContext = _viewModel;
        }
    }
}

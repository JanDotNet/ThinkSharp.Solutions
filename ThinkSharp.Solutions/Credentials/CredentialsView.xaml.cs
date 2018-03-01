﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ThinkSharp.Solutions.Credentials
{
    /// <summary>
    /// Interaction logic for CredentialsView.xaml
    /// </summary>
    public partial class CredentialsView : UserControl
    {
        public CredentialsView()
        {
            InitializeComponent();
        }

        private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CredentialsViewModel;
            if (vm != null)
                vm.Password = myPasswordBox.Password;
        }
    }
}

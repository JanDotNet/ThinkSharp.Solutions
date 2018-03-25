using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ThinkSharp.Solutions.Infrastructure
{
    public static class UIHelper
    {
        public static void ShowView<TView>(ViewModelBase viewModel) where TView : FrameworkElement, new()
        {
            ShowView<TView>(viewModel, Size.Empty);
        }
        public static void ShowView<TView>(ViewModelBase viewModel, Size windowSize) where TView : FrameworkElement, new()
        {
            var window = new DialogWindow();
            
            if (windowSize != Size.Empty)
            {
                window.SizeToContent = SizeToContent.Manual;
                window.Width = windowSize.Width;
                window.Height = windowSize.Height;
            }
            else
            {
                window.SizeToContent = SizeToContent.WidthAndHeight;
            }

            var view = Activator.CreateInstance<TView>() as TView;
            var vm = viewModel as IWindowContoller;

            if (vm != null)
            {
                vm.CmdCloseWindow = new DelegateCommand(() => window.Close());
                window.Title = vm.Title;
            }
            view.DataContext = vm;
            window.Content = view;

            window.WindowStyle = WindowStyle.ToolWindow;
            window.ShowInTaskbar = true;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            window.ShowDialog();
        }
    }
}

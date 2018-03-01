using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ThinkSharp.Solutions.Infrastructure
{
    public interface IWindowContoller
    {
        ICommand CmdCloseWindow { get; set; }
        string Title { get; }
    }

    public class DialogViewModel : ViewModelBase, IWindowContoller
    {
        public DialogViewModel(string title)
        {
            Title = title;
        }

        protected void CloseDialog()
        {
            (this as IWindowContoller)?.CmdCloseWindow.Execute(null);
        }

        protected virtual bool OnSubmit()
        {
            return true;
        }

        protected virtual bool OnCancel()
        {
            return true;
        }

        public string Title { get; }
        public bool? Canceled { get; private set; }
        ICommand IWindowContoller.CmdCloseWindow { get; set; }

        public ICommand CmdSubmit => new DelegateCommand(() =>
        {            
            if (OnSubmit())
            {
                Canceled = false;
                CloseDialog();
            }
        });

        public ICommand CmdCancel => new DelegateCommand(() =>
        {
            if (OnCancel())
            {
                Canceled = true;
                CloseDialog();
            }
        });
    }
}

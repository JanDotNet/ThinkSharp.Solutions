using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkSharp.Solutions.Infrastructure;

namespace ThinkSharp.Solutions.ServiceFromTemplate
{
    public abstract class StepViewModel : ViewModelBase
    {
        public StepViewModel(string actionText)
        {
            ActionText = actionText;
        }

        private bool myIsWorking = false;
        public bool IsWorking
        {
            get { return myIsWorking; }
            set
            {
                myIsWorking = value;
                OnPropertyChanged();
            }
        }

        public string ActionText { get; }
        public abstract Task<bool> ExecuteAsync(IProgress<string> progress, StepContext ctx);
        public virtual bool CanExecute() => true;
        public virtual void SaveSettings() { }
        public virtual Task<bool> OnNavigatedToAsync(IProgress<string> progress, StepContext ctx) => Task.FromResult(true);
        public virtual void UpdateStepContext(StepContext context) { }
    }
}

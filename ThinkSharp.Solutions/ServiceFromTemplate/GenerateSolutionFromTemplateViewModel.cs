using EnvDTE80;
using LibGit2Sharp;
using Microsoft.VisualStudio.Shell.Interop;
using ThinkSharp.Solutions.Credentials;
using ThinkSharp.Solutions.Infrastructure;
using ThinkSharp.Solutions.ItemSelection;
using ThinkSharp.Solutions.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using ThinkSharp.Solutions.ServiceFromTemplate.CloneRepository;
using ThinkSharp.Solutions.ServiceFromTemplate.PlaceholderReplacement;

namespace ThinkSharp.Solutions.ServiceFromTemplate
{
    public class GenerateSolutionFromTemplateViewModel : DialogViewModel
    {
        private static readonly ILog theLogger = LogManager.GetLogger(typeof(GenerateSolutionFromTemplateViewModel));

        private readonly StepViewModel[] myStepViewModels = new StepViewModel[]
        {
            new CloneRepositoryViewModel(),
            new ReplacePlaceholderViewModel()
        };

        private int myCurrentStepIndex = 0;
        private readonly IProgress<string>  myProgress;
        private readonly StepContext myStepContext = new StepContext();

        public GenerateSolutionFromTemplateViewModel() : base("Create form Template")
        {
            OpenSolutionAfterCreation = Settings.Default.OpenSolutionAfterCreation;

            myProgress = new Progress<string>(s => this.StatusText = s);
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

        private string myStatusText = "Ready.";
        public string StatusText
        {
            get { return myStatusText; }
            set
            {
                myStatusText = value;
                OnPropertyChanged();
            }
        }

        public bool OpenSolutionAfterCreation { get; set; }

        public ICommand CmdGenerateMicroService => new DelegateCommand(
            async () => await RunCurrentStep(),
            () => CurrentStep.CanExecute());
        
        private async Task RunCurrentStep()
        {
            try
            {
                CurrentStep.SaveSettings();
                Settings.Default.OpenSolutionAfterCreation = OpenSolutionAfterCreation;
                Settings.Default.Save();

                IsWorking = true;

                var succeeded = await CurrentStep.ExecuteAsync(myProgress, myStepContext);
                
                IsWorking = false;

				if (!succeeded)
                {
                    StatusText = "Failed.";
                    return;
                }

                StatusText = "Ready.";

				if (NextStep())
                {
                    await CurrentStep.OnNavigatedToAsync(myProgress, myStepContext);
                    return;
                }

				// last step                
                if (OpenSolutionAfterCreation)
                {
                    OpenSolution();
                }

                // close dialog after the last step.
                CloseDialog();               
            }
            catch (Exception ex)
            {
                var msg = $"An error occured while running step '{CurrentStep?.ActionText ?? "<null>"}'.";
                theLogger.Error(msg, ex);
                MessageBox.Show(msg + Environment.NewLine + "See log file for details: '%AppData%\\ThinkSharp\\Solutions\\error.log''", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenSolution()
        {
            var solutionFiles = Directory.GetFiles(myStepContext.TargetDirectory, "*.sln", SearchOption.AllDirectories);

            if (solutionFiles.Length == 0)
            {
                MessageBox.Show("MicroService created - unable to open solution because no solution file found.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string solutionFile = solutionFiles[0];
                if (solutionFiles.Length > 1)
                {
                    var itemSelectionViewModel = new ItemSelectionViewModel<string>();
                    foreach (var file in solutionFiles)
                        itemSelectionViewModel.Add(file, new FileInfo(file).Name, file);
                    UIHelper.ShowView<ItemSelectionView>(itemSelectionViewModel);
                    if (itemSelectionViewModel.Canceled == false)
                    {
                        solutionFile = itemSelectionViewModel.SelectedItem;
                    }
                    else
                    {
                        solutionFile = null;
                    }
                }

                if (solutionFile != null)
                {
                    var application = ServiceFromTemplateCommand.Instance.ServiceProvider.GetService(typeof(SDTE)) as DTE2;
                    if (!application.Solution.IsOpen)
                        application.Solution.Open(solutionFile);
                    else
                        Process.Start(solutionFile);
                }
            }
        }

        public StepViewModel CurrentStep => myStepViewModels[myCurrentStepIndex];

        private bool NextStep()
        {
            if (myCurrentStepIndex < myStepViewModels.Length - 1)
            {
                myCurrentStepIndex++;
                OnPropertyChanged("CurrentStep");
                return true;
            }

            return false;
        }

        public string Version => typeof(ServiceFromTemplateCommand).Assembly.GetName().Version.ToString(2);
    }
}

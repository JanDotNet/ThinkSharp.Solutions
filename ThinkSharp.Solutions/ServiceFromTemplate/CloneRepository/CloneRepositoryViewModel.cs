using EnvDTE80;
using LibGit2Sharp;
using Microsoft.VisualStudio.Shell.Interop;
using ThinkSharp.Solutions.Credentials;
using ThinkSharp.Solutions.Infrastructure;
using ThinkSharp.Solutions.ItemSelection;
using ThinkSharp.Solutions.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;

namespace ThinkSharp.Solutions.ServiceFromTemplate.CloneRepository
{
    public class CloneRepositoryViewModel : StepViewModel
    {
        private static ILog theLogger = LogManager.GetLogger(typeof(CloneRepositoryViewModel));
        public CloneRepositoryViewModel() : base("Clone Template")
        {
            GitRepository = Settings.Default.GitRepository;
            TargetDirectory = Settings.Default.TargetDirectory;
        }

        public string GitRepository { get; set; }        

        private string myTargetDirectory = string.Empty;
        public string TargetDirectory
        {
            get { return myTargetDirectory; }
            set
            {
                myTargetDirectory = value;
                OnPropertyChanged();
            }
        }

        public ICommand CmdSelectFolder => new DelegateCommand(
            () =>
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK)
                    return;
                TargetDirectory = dialog.SelectedPath;
            }
        }, () => !IsWorking);

        public override bool CanExecute()
            => !string.IsNullOrWhiteSpace(TargetDirectory) 
            && !string.IsNullOrWhiteSpace(GitRepository) 
            && !IsWorking;

        public override async Task<bool> ExecuteAsync(IProgress<string> progress, StepContext ctx)
        {
            try
            {
                if (!Directory.Exists(TargetDirectory))
                {
                    Directory.CreateDirectory(TargetDirectory);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Unable to create directory '{TargetDirectory}'.";
                theLogger.Error(msg, ex);
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!IOHelper.IsEmpty(TargetDirectory))
            {
                MessageBox.Show("Target directory must be empty!", "Info", MessageBoxButton.OK, MessageBoxImage.Hand);
                return false;
            }            

            progress.Report("Cloning Repository...");

            string gitRepoDir = null;
            try
            {
                // try to clone without authentication
                await Task.Run(() =>
                {
                    gitRepoDir = Repository.Clone(GitRepository, TargetDirectory);
                    IOHelper.NormalizeAttributes(gitRepoDir);
                    Directory.Delete(gitRepoDir, true);
                });
            }
            // probably authentication exception
            catch(Exception ex)
            {
                theLogger.Info($"Unable to clone repository '{GitRepository}' without credentials.", ex);
                var userName = string.IsNullOrWhiteSpace(Settings.Default.UserName) ? Environment.UserName : Settings.Default.UserName;
                var vm = new CredentialsViewModel(userName, "Enter Git Credentials");
                UIHelper.ShowView<CredentialsView>(vm);
                if (vm.Canceled == true) return false;

                await Task.Run(() =>
                {
                    var co = new CloneOptions();
                    co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = vm.UserName, Password = vm.Password };
                    gitRepoDir = Repository.Clone(GitRepository, TargetDirectory, co);
                });
            }

            IOHelper.NormalizeAttributes(gitRepoDir);
            Directory.Delete(gitRepoDir, true);

            ctx.TargetDirectory = TargetDirectory;

            return true;
        }

        public override void SaveSettings()
        {
            base.SaveSettings();

            Settings.Default.GitRepository = GitRepository;
            Settings.Default.TargetDirectory = TargetDirectory;
        }
    }
}

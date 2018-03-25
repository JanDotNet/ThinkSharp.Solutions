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
using System.Text.RegularExpressions;

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

        public string GitBranch { get; set; }

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

            if (!await CloneRepositoryAsync(requestCredentials: false))
                return false;

            ctx.TargetDirectory = TargetDirectory;

            return true;
        }

        private async Task<bool> CloneRepositoryAsync(bool requestCredentials)
        {
            string gitRepoDir = null;
            try
            {
                string username = null;
                string password = null;

                if (requestCredentials)
                {
                    theLogger.Info("Requesting Credentials.");
                    if (!RequestCredentials(out username, out password))
                    {
                        return false;
                    }
                }

                theLogger.Info("Cloning Repository.");
                gitRepoDir = await CloneRepositoryAsync(username, password);

                theLogger.Info("Normalizing Attributes.");
                IOHelper.NormalizeAttributes(gitRepoDir);

                theLogger.Info("Deleting GIT reporitory from working copy.");
                Directory.Delete(gitRepoDir, true);

                return true;
            }
            catch (Exception ex)
            {
                // not authenticated
                if (Regex.IsMatch(ex.Message, "request failed with status code: 401"))
                {
                    theLogger.Error("Unable to clone repository: Invalid Credentials. Retry requesting credentials.", ex);
                    return await CloneRepositoryAsync(requestCredentials: true);
                }
                // not found
                if (Regex.IsMatch(ex.Message, "request failed with status code: 404"))
                {
                    var msg = $"Repository '{GitRepository}' does not exist.";
                    theLogger.Error("Unable to clone repository: " + msg, ex);
                    MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                // invalid branch name
                if (Regex.IsMatch(ex.Message, "reference '[^';]*' not found"))
                {
                    var msg = $"Branch '{GitBranch}' does not exist.";
                    theLogger.Error("Unable to clone repository: " + msg, ex);
                    MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                // invalid protocol
                if (Regex.IsMatch(ex.Message, "unsupported URL protocol"))
                {
                    var msg = $"Unsupported URL protocol. Note that the only supported protocol is HTTP(S).";
                    theLogger.Error("Unable to clone repository: " + msg, ex);
                    MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                // unknonw issue
                else
                {
                    MessageBox.Show($"Unable to clone repository: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    theLogger.Error("Unable to clone repository: Unknown Error.", ex);
                    return false;
                }
            }
        }

        private bool RequestCredentials(out string username, out string password)
        {
            username = string.IsNullOrWhiteSpace(Settings.Default.UserName) ? Environment.UserName : Settings.Default.UserName;
            password = string.Empty;

            var vm = new CredentialsViewModel(username, "Enter Git Credentials");
            UIHelper.ShowView<CredentialsView>(vm);

            if (vm.Canceled == true) return false;

            username = vm.UserName;
            password = vm.Password;

            Settings.Default.UserName = username;
            Settings.Default.Save();

            return true;
        }

        private Task<string> CloneRepositoryAsync(string username = null, string password = null)
        {
            return Task.Run(() =>
            {
                var co = new CloneOptions();
                if (!string.IsNullOrEmpty(GitBranch)) co.BranchName = GitBranch;
                if (username != null) co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = username, Password = password };
                return Repository.Clone(GitRepository, TargetDirectory, co);                    
            });
        }

        public override void SaveSettings()
        {
            base.SaveSettings();

            Settings.Default.GitRepository = GitRepository;
            Settings.Default.TargetDirectory = TargetDirectory;
        }
    }
}

using ThinkSharp.Solutions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using ThinkSharp.Solutions.TemplateDefinition;

namespace ThinkSharp.Solutions.ServiceFromTemplate.PlaceholderReplacement
{
    public class ReplacePlaceholderViewModel : StepViewModel
    {
        private static readonly ILog theLogger = LogManager.GetLogger(typeof(ReplacePlaceholderViewModel));

        public ReplacePlaceholderViewModel() : base("Replace Placeholders")
        { }

        public ObservableCollection<PlaceholderViewModel> Placeholders { get; } = new ObservableCollection<PlaceholderViewModel>();

        public override bool CanExecute()
        {
            return Placeholders.All(p => !string.IsNullOrWhiteSpace(p.Replacement));
        }

        public override async Task<bool> OnNavigatedToAsync(IProgress<string> progress, StepContext ctx)
        {
            if (!IsTargetDirectoryValid(ctx))
                return false;

            var templateDefinitionFile = Path.Combine(ctx.TargetDirectory, "TemplateDefinition.xml");
            if (!File.Exists(templateDefinitionFile))
                return false;

            var templateDefinition = await Task.Run(() => TemplateDefinitionHelper.Parse(templateDefinitionFile));
            if (templateDefinition == null)
                return false;

            try
            {
                File.Delete(templateDefinitionFile);
            }
            catch(Exception ex)
            {
                theLogger.Error($"Unable to delete template definition file: '{templateDefinitionFile}'.", ex);
            }

            foreach (var placeholderDefinition in templateDefinition.Placeholders)
            {
                var placeholder = new PlaceholderViewModel()
                {
                    Name = placeholderDefinition.Name,
                    TextToReplace = string.IsNullOrEmpty(placeholderDefinition.TextToReplace) 
                        ? placeholderDefinition.Name
                        : placeholderDefinition.TextToReplace,
                    Replacement = placeholderDefinition.DefaultValue,
                    Suggestions = TemplateDefinitionHelper.SplitEscapedString(placeholderDefinition.SuggestionList),
                    Description = placeholderDefinition.Description
                };

                Placeholders.Add(placeholder);
            }

            return true;
        }

        public override async Task<bool> ExecuteAsync(IProgress<string> progress, StepContext ctx)
        {
            if (!IsTargetDirectoryValid(ctx))
                return false;

            var placeholders = Placeholders.Select(p => new KeyValuePair<string, string>(p.TextToReplace, p.Replacement)).ToArray();

            progress.Report("Replacing placeholders in file names and directories...");

            await Task.Run(() => IOHelper.ReplacePlaceholdersInFileNamesAndDirectories(ctx.TargetDirectory, placeholders));

            progress.Report("Replacing placeholders in file content...");

            await Task.Run(() => IOHelper.ReplacePlaceholdersInFilesContent(ctx.TargetDirectory, placeholders));

            progress.Report("Replacing guids in file content...");

            await Task.Run(() => IOHelper.ReplaceGuids(ctx.TargetDirectory));

            return true;
        }

        private bool IsTargetDirectoryValid(StepContext ctx)
        {
            if (string.IsNullOrEmpty(ctx.TargetDirectory))
            {
                var msg = "'StepContext.TargetDirectory' is null or empty.";
                theLogger.Error(msg);
                ctx.Errors.Add(msg);
                return false;
            }

            if (!Directory.Exists(ctx.TargetDirectory))
            {
                var msg = $"'StepContext.TargetDirectory' ({ctx.TargetDirectory}) does not exist.";
                theLogger.Error(msg);
                ctx.Errors.Add(msg);
                return false;
            }
            return true;
        }
    }
}

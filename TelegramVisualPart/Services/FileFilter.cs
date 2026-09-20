using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramVisualPart.Services
{
    public static class FileFilter
    {
        public const string AllFilesEntry = "All files (*.*)|*.*";

        public static string Create(string description, params string[] extensions)
        {
            return Create(description, true, extensions);
        }

        public static string Create(string description, bool showExtensionsInDescription, params string[] extensions)
        {
            string patterns = string.Join(";", extensions.Select(NormalizeExtension));
            string label = showExtensionsInDescription ? $"{description} ({patterns})" : description;
            return $"{label}|{patterns}";
        }

        public static string Build(IEnumerable<(string Description, string[] Extensions)> groups, bool includeAllFiles = true)
        {
            var parts = groups.Select(g => Create(g.Description, g.Extensions)).ToList();

            if (includeAllFiles)
                parts.Add(AllFilesEntry);

            return string.Join("|", parts);
        }

        public static string Build(bool includeAllFiles, params (string Description, string[] Extensions)[] groups)
        {
            return Build(groups, includeAllFiles);
        }
        private static string NormalizeExtension(string ext)
        {
            ext = ext.Trim().TrimStart('.', '*');
            return $"*.{ext}";
        }
    }

    public sealed class FileDialogFilterBuilder
    {
        private readonly List<(string Description, string[] Extensions)> _groups = new();
        private bool _includeAllFiles;

        public static FileDialogFilterBuilder New() => new();

        public FileDialogFilterBuilder Add(string description, params string[] extensions)
        {
            _groups.Add((description, extensions));
            return this;
        }

        public FileDialogFilterBuilder AddAllFiles()
        {
            _includeAllFiles = true;
            return this;
        }

        public string Build()
        {
            return FileFilter.Build(_groups, _includeAllFiles);
        }
        public override string ToString()
        {
            return Build();
        }
    }
}

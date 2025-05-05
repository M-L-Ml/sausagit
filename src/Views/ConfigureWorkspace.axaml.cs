using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace SourceGit.Views
{
    public partial class ConfigureWorkspace : ChromelessWindow
    {
        public ConfigureWorkspace()
        {
            InitializeComponent();
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            base.OnClosing(e);

            if (!Design.IsDesignMode)
                ViewModels.Preferences.Instance.Save();
        }

        private async void SelectDefaultCloneDir(object _, RoutedEventArgs e)
        {
            var workspace = DataContext as ViewModels.ConfigureWorkspace;
            if (workspace?.Selected == null)
                return;

            try
            {
                var selected = await ViewModels.Preferences.GetSelectDefaultCloneDirAsync( StorageProvider);
                if (selected != null)
                {
                    workspace.Selected.DefaultCloneDir = selected;
                }
            }
            catch (Exception ex)
            {
                App.RaiseException(string.Empty, $"Failed to select default clone directory: {0}", ex);
            }

            e.Handled = true;
        }
    }
}

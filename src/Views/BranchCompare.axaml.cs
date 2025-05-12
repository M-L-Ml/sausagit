using Avalonia.Controls;
using Avalonia.Input;

namespace SourceGit.Views
{
    public partial class BranchCompare : ChromelessWindow
    {
        public BranchCompare()
        {
            InitializeComponent();
        }

        private void OnChangeContextRequested(object sender, ContextRequestedEventArgs e)
        {
            if (DataContext is ViewModels.BranchCompare vm && sender is ChangeCollectionView view)
            {
                var menuModel = vm.CreateChangeContextMenu();
                var menu = menuModel?.CreateContextMenuFromModel();
                menu?.Open(view);
            }

            e.Handled = true;
        }

        private void OnPressedSHA(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is ViewModels.BranchCompare vm && sender is TextBlock block)
                vm.NavigateTo(block.Text);

            e.Handled = true;
        }
    }
}

using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Interactivity;
using System.Linq;
using CompanyFrontend.ViewModels;

namespace CompanyFrontend.Views
{
    public partial class CompanyListView : UserControl
    {
        private ScrollViewer? _scrollViewer;

        public CompanyListView()
        {
            InitializeComponent();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            // On cherche le ScrollViewer à l'intérieur du DataGrid
            var dataGrid = this.FindControl<DataGrid>("CompaniesGrid");
            _scrollViewer = dataGrid.GetVisualDescendants()
                .OfType<ScrollViewer>()
                .FirstOrDefault();

            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged += OnScrollChanged;
            }
        }

        private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if (_scrollViewer == null) return;
            
            var isAtBottom = _scrollViewer.Offset.Y >= (_scrollViewer.Extent.Height - _scrollViewer.Viewport.Height - 50);

            if (isAtBottom)
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    _ = vm.LoadNextPage();
                }
            }
        }
    }
}
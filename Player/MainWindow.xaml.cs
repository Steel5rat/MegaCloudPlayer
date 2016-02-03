using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Player.ViewModels;

namespace Player
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        private bool _isLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        public override async void EndInit()
        {
            base.EndInit();
            if (_isLoaded) return;
            _isLoaded = true;

            NotPlayingList.Items.SortDescriptions.Add(new SortDescription(string.Empty, ListSortDirection.Ascending));
            PlayingList.Items.SortDescriptions.Add(new SortDescription(string.Empty, ListSortDirection.Ascending));
            NotPlayingList.ItemsSource = await Task.Run(() =>
            {
                _viewModel = new MainWindowViewModel();
                return _viewModel.NotPlayingList;
            });
            PlayingList.ItemsSource = _viewModel.PlayingList;
        }

        private void NotPlayingList_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _viewModel.MoveToPlayList(((sender as ListBox).SelectedItem as ListItem).Id);
        }

        private void PlayingList_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _viewModel.MoveFromPlayList(((sender as ListBox).SelectedItem as ListItem).Id);
        }
    }
}
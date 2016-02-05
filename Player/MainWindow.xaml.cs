using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Player.Utils;
using Player.ViewModels;

namespace Player
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HotkeyHook _hotkeyHook;
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override async void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            NotPlayingList.Items.SortDescriptions.Add(new SortDescription(string.Empty, ListSortDirection.Ascending));
            PlayingList.Items.SortDescriptions.Add(new SortDescription(string.Empty, ListSortDirection.Ascending));
            NotPlayingList.ItemsSource = await Task.Run(() =>
            {
                _viewModel = new MainWindowViewModel();
                return _viewModel.NotPlayingList;
            });
            PlayingList.ItemsSource = _viewModel.PlayingList;
            _hotkeyHook = new HotkeyHook(this, _viewModel);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _hotkeyHook.Dispose();
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
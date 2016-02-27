using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NAudio.Wave;
using Player.Models;
using Player.Utils;

namespace Player.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly Node _model;
        private readonly AudioPlayer _player;

        public MainWindowViewModel()
        {
            _player = new AudioPlayer();
            _model = new Node();
            NotPlayingList = new ObservableCollection<ListItem>(
                _model.GetRootDirs().Select(s => new ListItem
                {
                    Name = s.Name,
                    Id = s.Id
                }));
            PlayingList = new ObservableCollection<ListItem>();
            PlayingList.CollectionChanged += PlayingList_CollectionChanged;
        }

        public ObservableCollection<ListItem> NotPlayingList { get; set; }
        public ObservableCollection<ListItem> PlayingList { get; set; }

        private void PlayingList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var files = _model.GetAllMp3(PlayingList.Select(s => s.Id).ToList());
            _player.Init(files);
            _player.Play();
        }

        public void MoveToPlayList(string id)
        {
            SwapItem(NotPlayingList, PlayingList, id);
        }

        public void MoveFromPlayList(string id)
        {
            SwapItem(PlayingList, NotPlayingList, id);
        }

        public void PauseHotkeyPressed()
        {
            _player.Pause();//TODO: more intellect
        }

        public void NextHotkeyPressed()
        {
            _player.Forward();
        }

        public void PreviousHotkeyPressed()
        {
            _player.Back();
        }
        public void ShowHotkeyPressed() { }

        private void SwapItem(ICollection<ListItem> source, ICollection<ListItem> destination, string id)
        {
            var item = source.First(f => f.Id == id);
            source.Remove(item);
            destination.Add(item);
        }
    }

    public class ListItem : IComparable
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public int CompareTo(object obj)
        {
            if (!(obj is ListItem))
                return -1;
            return string.Compare(Name, ((ListItem)obj).Name, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
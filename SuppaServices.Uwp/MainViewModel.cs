using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Web.Syndication;
using ReactiveUI;
using SuppaServices.Interfaces;
using SuppaServices.Interfaces.Personnel;
using Zafiro.Core;

namespace SuppaServices.Uwp
{
    public class MainViewModel : ReactiveObject
    {
        private string _searchString;
        private readonly IPersonnelService _personnelService;

        private ObservableAsPropertyHelper<IEnumerable<PersonnelListEntry>> _personnelList;

        public MainViewModel(IPersonnelService personnelService, IFilePicker filePicker)
        {
            _personnelService = personnelService;
            BrowseFile = ReactiveCommand
                .CreateFromObservable(() => filePicker.Pick("Select an image", new[] {".png", ".jpg"})
                    .Where(file => file != null)
                    .SelectMany(x => Observable.FromAsync(() => ToBytes(x))));
            
            _personnelList = this
                .WhenAnyValue(x => x.SearchString)
                .Throttle(TimeSpan.FromMilliseconds(800))
                .Select(term => term?.Trim())
                .DistinctUntilChanged()
                .Where(term => !string.IsNullOrWhiteSpace(term))
                .SelectMany(SearchPersonnel)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.PersonnelList);


        }

        private async Task<IEnumerable<PersonnelListEntry>> SearchPersonnel(string searchString)
        {
            return await _personnelService.GetPersonnelListEntries(searchString);
        }

        public bool IsLoading => false;

        public ReactiveCommand<Unit, byte[]> BrowseFile { get; }
        
        private async Task<byte[]> ToBytes(ZafiroFile zafiroFile)
        {
            using (var stream = await zafiroFile.OpenForRead())
            {
                return await stream.ReadBytes();
            }
        }
        
        public string SearchString
        {
            get => _searchString;
            set => this.RaiseAndSetIfChanged(ref _searchString, value);
        }

        public IEnumerable<PersonnelListEntry> PersonnelList => _personnelList.Value;

    }
}
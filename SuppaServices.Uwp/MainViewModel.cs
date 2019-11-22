using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using SuppaServices.Interfaces;
using Zafiro.Core;

namespace SuppaServices.Uwp
{
    public class MainViewModel : ReactiveObject
    {
        private float angle;
        private readonly ObservableAsPropertyHelper<byte[]> destination;
        private readonly ObservableAsPropertyHelper<byte[]> source;
        private readonly ObservableAsPropertyHelper<bool> isLoading;

        public MainViewModel(IBitmapService bitmapService, IFilePicker filePicker)
        {
            BrowseFile = ReactiveCommand
                .CreateFromObservable(() => filePicker.Pick("Select an image", new[] {".png", ".jpg"})
                    .Where(file => file != null)
                    .SelectMany(x => Observable.FromAsync(() => ToBytes(x))));
            Rotate = ReactiveCommand.CreateFromTask(() => bitmapService.Create(Source, Angle), BrowseFile.Any());

            source = BrowseFile.ToProperty(this, x => x.Source);
            destination = Rotate.ToProperty(this, x => x.Destination);
            Angle = 90f;

            isLoading = Rotate.IsExecuting.ToProperty(this, x => x.IsLoading);
        }

        public bool IsLoading => isLoading.Value;

        public ReactiveCommand<Unit, byte[]> BrowseFile { get; }
        
        private async Task<byte[]> ToBytes(ZafiroFile zafiroFile)
        {
            using (var stream = await zafiroFile.OpenForRead())
            {
                return await stream.ReadBytes();
            }
        }


        public ReactiveCommand<Unit, byte[]> Rotate { get; }

        public float Angle
        {
            get => angle;
            set => this.RaiseAndSetIfChanged(ref angle, value);
        }

        public byte[] Source => source.Value;

        public byte[] Destination => destination.Value;
    }
}
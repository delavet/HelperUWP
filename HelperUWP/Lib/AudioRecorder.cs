using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;

namespace HelperUWP.Lib
{
    class AudioRecorder
    {
        private MediaCapture _mediaCapture;
        private InMemoryRandomAccessStream _memoryBuffer;
        private StorageFile fileBuffer;
        public bool IsRecording { get; set; }
        public AudioRecorder()
        {
            IsRecording = false;
        }
        public async Task Record()
        {
            if (IsRecording)
            {
                throw new InvalidOperationException("Recording already in progress!");
            }
            _memoryBuffer = new InMemoryRandomAccessStream();
            _mediaCapture = new MediaCapture();
            MediaCaptureInitializationSettings settings =
             new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Audio
                };
            
            await _mediaCapture.InitializeAsync(settings);
            MediaEncodingProfile recordProfile = new MediaEncodingProfile();
            
            AudioEncodingProperties audioProp = new AudioEncodingProperties();
            audioProp.ChannelCount = 1;
            audioProp.BitsPerSample = 16;
            audioProp.SampleRate = 8000;
            audioProp.Bitrate = 16;
            audioProp.Subtype = MediaEncodingSubtypes.AmrNb;
            recordProfile.Audio = audioProp;
            ContainerEncodingProperties containProp = new ContainerEncodingProperties();
            containProp.Subtype = MediaEncodingSubtypes.Mpeg4;
            recordProfile.Container = containProp;
            
            recordProfile.Video = null;
            //await _mediaCapture.StartRecordToStorageFileAsync(recordProfile, fileBuffer);
            StorageFile amr = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/a.amr"));
            var pro = await MediaEncodingProfile.CreateFromFileAsync(amr);
            pro.Container.Subtype = MediaEncodingSubtypes.Mpeg4;
            var pro2 = MediaEncodingProfile.CreateWma(AudioEncodingQuality.Low);
            await _mediaCapture.StartRecordToStreamAsync(pro2, _memoryBuffer);
            IsRecording = true;
        }
        public async Task<IRandomAccessStream> StopRecording()
        {
            await _mediaCapture.StopRecordAsync();
            IsRecording = false;
            return _memoryBuffer.CloneStream();
        }
    }
}

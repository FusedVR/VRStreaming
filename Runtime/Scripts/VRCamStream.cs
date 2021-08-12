using System.Collections;
using UnityEngine;
using Unity.RenderStreaming;
using Unity.WebRTC;

namespace FusedVR.VRStreaming {
    public class VRCamStream : CameraStreamer {
        private string mainConnection = "";

        [SerializeField]
        private ulong BITRATE = 12582912 ; //bitrate in Mbs

        [SerializeField]
        private uint FRAMERATE = 90; //framerate target

        // Start is called before the first frame update
        void Start() {
            OnStartedStream += StartStream;
            StartCoroutine(TestBitrate());
        }

        private void StartStream(string connectionId) {
            mainConnection = connectionId;
        }

        // Update is called once per frame
        IEnumerator TestBitrate() {
            while (true) {
                if (Senders.TryGetValue(mainConnection, out var sender)) {
                    RTCRtpSendParameters parameters = sender.GetParameters();
                    foreach (var encoding in parameters.encodings) {
                        encoding.minBitrate = BITRATE;
                        encoding.maxBitrate = BITRATE;
                        encoding.maxFramerate = FRAMERATE;
                    }

                    sender.SetParameters(parameters);
                }

                yield return new WaitForSeconds(2f);
            }


        }
    }
}


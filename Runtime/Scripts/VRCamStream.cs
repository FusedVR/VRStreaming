using System.Collections;
using UnityEngine;
using Unity.RenderStreaming;
using Unity.WebRTC;

namespace FusedVR.VRStreaming {
    public class VRCamStream : CameraStreamer {
        private string mainConnection = "";

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
                        Debug.Log("Min Bitrate : " + encoding.minBitrate);
                        Debug.Log("Max Bitrate : " + encoding.maxBitrate);
                    }
                }

                yield return new WaitForSeconds(2f);
            }


        }
    }
}


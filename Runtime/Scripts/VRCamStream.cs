using System.Collections;
using UnityEngine;
using Unity.RenderStreaming;
using Unity.WebRTC;

namespace FusedVR.VRStreaming {
    public class VRCamStream : VideoStreamBase {

        [SerializeField] private int depth = 0;
        [SerializeField] private int antiAliasing = 1;

        [SerializeField]
        private Camera leftEye;

        [SerializeField]
        private Camera rightEye;

        private string mainConnection = "";

        [SerializeField]
        private ulong BITRATE = 12582912 ; //bitrate in Mbs

        [SerializeField]
        private uint FRAMERATE = 90; //framerate target

        public override Texture SendTexture => leftEye.targetTexture; //should be the same as right eye

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

        protected override MediaStreamTrack CreateTrack() {

            RenderTextureFormat format = WebRTC.GetSupportedRenderTextureFormat(SystemInfo.graphicsDeviceType);
            RenderTexture rt = new RenderTexture(streamingSize.x, streamingSize.y, depth, format) {
                antiAliasing = antiAliasing
            };
            rt.Create();

            leftEye.targetTexture = rt;
            leftEye.rect = new Rect(Vector2.zero, new Vector2(0.5f, 1f));

            rightEye.targetTexture = rt;
            rightEye.rect = new Rect(new Vector2(0.5f, 0f), new Vector2(0.5f, 1f));

            return new VideoStreamTrack("VR Camera", rt);
        }
    }
}


using UnityEngine;
using Unity.RenderStreaming;
using Unity.WebRTC;

namespace FusedVR.VRStreaming {
    public class VRCamStream : VideoStreamSender {

        #region Variables
        [SerializeField]
        [Tooltip("The Cameras to combine together for the render texture")]
        private Camera[] cameras;

        [SerializeField]
        [Tooltip("Defines the depth buffer used for render streaming (0, 16, 24, 32)")]
        private int depth = 0;

        [SerializeField]
        [Tooltip("Defines the number of samples for anti-aliasing (1, 2, 4, 8)")]
        private int antiAliasing = 1;

        [SerializeField]
        [Tooltip("Defines the default bitrate to be used by the encoders in bits per second")]
        private ulong BIT_RATE = 9000000; //default bitrate of 9 Mbps

        /// <summary>
        /// The Main Connection ID that this instance is connected with
        /// </summary>
        private string mainConnection = "";

        public const uint MAX_FRAMERATE = 90; //default max framerate target
        #endregion

        #region Events
        // Start is called before the first frame update
        void Start() {
            OnStartedStream += StartStream;
        }

        private void StartStream(string connectionId) {
            mainConnection = connectionId;
            ChangeSendParameters(BIT_RATE, MAX_FRAMERATE);
        }

        /// <summary>
        /// Assigns Render Textures to our cameras and then 
        /// Creates a Video Streaming Track that references the render textures
        /// </summary>
        protected override MediaStreamTrack CreateTrack() {

            RenderTextureFormat format = WebRTC.GetSupportedRenderTextureFormat(SystemInfo.graphicsDeviceType);
            RenderTexture rt = new RenderTexture(streamingSize.x, streamingSize.y, depth, format) {
                antiAliasing = antiAliasing
            };
            rt.Create();

            // divide cameras into n sections over the canvas
            for ( int i = 0; i < cameras.Length; i++) {
                cameras[i].targetTexture = rt;
                cameras[i].rect = new Rect(new Vector2(i / cameras.Length, 0f), new Vector2(1 / cameras.Length, 1f)); 
            }

            return new VideoStreamTrack(rt);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Change Parameters associated with encoders for sending data to browser
        /// </summary>
        public void ChangeSendParameters(ulong? bitrate , uint? framerate) {
            if (Senders.TryGetValue(mainConnection, out var sender)) {
                RTCRtpSendParameters parameters = sender.GetParameters();
                foreach (var encoding in parameters.encodings) {
                    if (bitrate != null) {
                        encoding.minBitrate = bitrate;
                        encoding.maxBitrate = bitrate;
                    }

                    if (framerate != null) {
                        encoding.maxFramerate = framerate;
                    }
                }

                sender.SetParameters(parameters);
            }
        }
        #endregion

    }
}


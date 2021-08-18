using UnityEngine;
using Unity.RenderStreaming;
using Unity.WebRTC;

namespace FusedVR.VRStreaming {
    public class VRCamStream : VideoStreamBase {

        #region Variables
        [SerializeField]
        [Tooltip("The Left Eye of the VR Camera")]
        private Camera leftEye;

        [SerializeField]
        [Tooltip("The Right Eye of the VR Camera")]
        private Camera rightEye;

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

        public override Texture SendTexture => leftEye.targetTexture; //should be the same as right eye
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

            leftEye.targetTexture = rt;
            leftEye.rect = new Rect(Vector2.zero, new Vector2(0.5f, 1f));

            rightEye.targetTexture = rt;
            rightEye.rect = new Rect(new Vector2(0.5f, 0f), new Vector2(0.5f, 1f));

            return new VideoStreamTrack("VR Camera", rt);
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


using System.Collections.Generic;
using Unity.RenderStreaming;
using UnityEngine;

namespace FusedVR.VRStreaming {
    /// <summary>
    /// The Stream Manager is responsible for managing the various streams that we want to output to the client
    /// </summary>
    public class ClientStreams : MonoBehaviour
    {
        /// <summary>
        /// Whether to Delete this Instance if the connection is dropped
        /// </summary>
        [Tooltip("Whether to Delete this Instance if the connection is dropped")]
        public bool isDeletable = false;

        /// <summary>
        /// Data Channel for the client
        /// </summary>
        [SerializeField]
        private InputChannelReceiverBase dataChannel;

        /// <summary>
        /// Streams (video, audio) that need to be sent to the client
        /// </summary>
        [SerializeField]
        private List<StreamSourceBase> streams = new List<StreamSourceBase>();

        /// <summary>
        /// Connection ID for client
        /// </summary>
        private string myConnection;

        /// <summary>
        /// Set the connection based on the signalling data from the client on an Offer
        /// </summary>
        public void SetFullConnection(string connectionID, SignalingHandlerBase broadcast) {
            myConnection = connectionID; //save ID

            foreach (StreamSourceBase source in streams) {
                var transceiver = broadcast.AddTrack(myConnection, source.Track);
                source.SetSender(myConnection, transceiver.Sender);
            }

            var _channel = broadcast.CreateChannel(myConnection, dataChannel.Label);
            dataChannel.SetChannel(myConnection, _channel);
        }

        /// <summary>
        /// Enable & Disable View Streams
        /// TODO: this is currently broken in WebRTC 
        /// Filed Issue : https://github.com/Unity-Technologies/com.unity.webrtc/issues/523
        /// </summary>
        public void ViewStreams(bool view) {
            foreach (StreamSourceBase source in streams) {
                source.Track.Enabled = view;
            }
        }

        /// <summary>
        /// Set the Data Channel for the Client manually
        /// </summary>
        public void SetDataChannel(SignalingEventData data) {
            dataChannel.SetChannel(data.connectionId, data.channel);
        }

        /// <summary>
        /// Clean up streams for a disconnected client
        /// </summary>
        public void DeleteConnection(string connectionID) {

            if (myConnection == connectionID) {
                foreach (StreamSourceBase source in streams) {
                    source.SetSender(myConnection, null);
                }

                dataChannel.SetChannel(myConnection, null);

                myConnection = null; //remove ID

                if (isDeletable) {
                    Destroy(gameObject); //destroy self 
                }
            }
        }
    }
}
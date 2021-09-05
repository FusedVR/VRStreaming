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
        [HideInInspector]
        public string myConnection;

        public void SetFullConnection(string connectionID, SignalingHandlerBase broadcast) {

            myConnection = connectionID; //save ID

            foreach (StreamSourceBase source in streams) {
                var transceiver = broadcast.AddTrack(myConnection, source.Track);
                source.SetSender(myConnection, transceiver.Sender);
            }

            var _channel = broadcast.CreateChannel(myConnection, dataChannel.Label);
            dataChannel.SetChannel(myConnection, _channel);

        }

        public void SetDataChannel(SignalingEventData data) {
            dataChannel.SetChannel(data.connectionId, data.channel);
        }

        public void DeleteConnection(string connectionID) {

            if (myConnection == connectionID) {
                foreach (StreamSourceBase source in streams) {
                    source.SetSender(myConnection, null);
                }

                dataChannel.SetChannel(myConnection, null);

                myConnection = null; //save ID
            }
        }
    }
}
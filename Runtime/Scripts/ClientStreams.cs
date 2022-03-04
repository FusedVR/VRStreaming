using Newtonsoft.Json;
/**
 * Copyright 2021 Vasanth Mohan. All rights and licenses reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 */

using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using Unity.WebRTC;
using UnityEngine;

namespace FusedVR.VRStreaming {
    /// <summary>
    /// The Stream Manager is responsible for managing the various streams that we want to output to the client
    /// </summary>
    public class ClientStreams : MonoBehaviour
    {
        #region Variables
        /// <summary>
        /// Data Channel for the client
        /// </summary>
        [SerializeField]
        private InputChannelReceiverBase dataChannel;

        /// <summary>
        /// Microphone Audio Stream 
        /// </summary>
        [SerializeField]
        private StreamReceiverBase micStream;

        /// <summary>
        /// Streams (video, audio) that need to be sent to the client
        /// </summary>
        [SerializeField]
        private List<StreamSenderBase> streams = new List<StreamSenderBase>();

        /// <summary>
        /// Connection ID for client
        /// </summary>
        private string myConnection;
        #endregion

        #region Events
        /// <summary>
        /// Static events for when a client joins or leaves
        /// </summary>
        public delegate void OnClientStream(ClientStreams player);
        public static OnClientStream OnClientAdded;
        public static OnClientStream OnClientLeft;

        private void OnEnable() {
            OnClientAdded?.Invoke(this);
        }

        private void OnDisable() {
            OnClientLeft?.Invoke(this);
        }
        #endregion

        /// <summary>
        /// Set the connection based on the signalling data from the client on an Offer
        /// </summary>
        public void SetFullConnection(string connectionID, SignalingHandlerBase broadcast) {
            myConnection = connectionID; //save ID

            foreach (StreamSenderBase source in streams) {
                broadcast.AddSender(myConnection, source);
            }

            //broadcast.AddReceiver(myConnection, micStream);

            broadcast.AddChannel(myConnection, dataChannel);
        }

        IEnumerator TestSender (SignalingHandlerBase broadcast , StreamSenderBase sender) {

            yield return new WaitForSeconds(0.4f);
            Debug.LogError("WAITING MY TURN");
            broadcast.AddSender(myConnection, sender);
        } 

        /// <summary>
        /// Set the Microphone which will be recieve after answer
        /// </summary>
        public void AddMicrophone(RTCRtpReceiver rec) {
            micStream?.SetReceiver(myConnection, rec);
        }

        /// <summary>
        /// Enable & Disable View Streams
        /// TODO: disabling track causes the project to crash. disabling / enabling the stream seems fine
        /// Filed Issue : https://github.com/Unity-Technologies/com.unity.webrtc/issues/523
        /// </summary>
        public void ViewStreams(bool view) {
            foreach (StreamSenderBase source in streams) {
                source.gameObject.SetActive(view); //disables sending data
                //source.Track.Enabled = view;
            }
        }

        /// <summary>
        /// Send Event Data to this Client to be processed on the data channel
        /// </summary>
        public void SendDataMessage(string evt, string data) {
            Dictionary<string, string> payload = new Dictionary<string, string> 
            {
                { "event", evt },
                { "payload" , data}
            };
            string json = JsonConvert.SerializeObject(payload, Formatting.Indented);
            dataChannel.Channel.Send( json ); //send over the data channel
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
                foreach (StreamSenderBase source in streams) {
                    source.SetSender(myConnection, null);
                }

                dataChannel.SetChannel(myConnection, null);

                myConnection = null; //remove ID
            }
        }
    }
}
/**
 * Copyright 2021 Vasanth Mohan. All rights and licenses reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.RenderStreaming;
using Newtonsoft.Json;

namespace FusedVR.VRStreaming {
    /// <summary>
    /// VRBroadcast is very similar to the Broadcast script included with Render Streaming.
    /// This script is responsible for listening for offers from the client and choosing when to respond. 
    /// </summary>
    public class VRBroadcast : SignalingHandlerBase,
        IOfferHandler, IAddChannelHandler, IDisconnectHandler, IDeletedConnectionHandler {

        #region Variables

        public static VRBroadcast Instance { get; private set; } //Singleton Broadcast Manager

        /// <summary>
        /// Check if client is listening for the same GameID
        /// </summary>
        [SerializeField]
        [Tooltip("This ID should match the input from the Web Browser. Leave blank to match against anything")]
        private string gameID = "";

        [SerializeField]
        [Tooltip("The maximum number of connections you would like to connect to this server.")]
        private int maxConnections = 1;

        [SerializeField]
        [Tooltip("The player prefabs to be spawned upon a connection.")]
        public ClientStreams[] playerPrefabs;

        /// <summary>
        /// List of all connectionIds that are connected
        /// </summary>
        private Dictionary<string , ClientStreams> connections = new Dictionary<string, ClientStreams>();
        #endregion

        private void Awake() {
            //Singleton
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            } else {
                Instance = this;
            }
        }

        #region Disconnect
        public void OnDeletedConnection(SignalingEventData eventData) {
            Disconnect(eventData.connectionId);
        }

        public void OnDisconnect(SignalingEventData eventData) {
            Disconnect(eventData.connectionId);
        }

        private void Disconnect(string connectionId) {
            if (!connections.ContainsKey(connectionId))
                return;

            connections[connectionId].DeleteConnection(connectionId); //remove streams
            connections.Remove(connectionId); //remove dictionary
        }
        #endregion

        /// <summary>
        /// Event that is called when an Offer is made by a client
        /// Determines whether to accept offer and if so that to apply sources and submit answer
        /// </summary>
        public void OnOffer(SignalingEventData data) {

            string inputGID = GetGameID(data.sdp);
            if ( gameID.Length != 0 && inputGID != gameID) {
                Debug.Log($"Offer Doesn't Match My GameID : {inputGID}");
                return;
            }

            if (connections.Count >= maxConnections) { //if there is more than 1 connection, let's skip this offer
                Debug.LogWarning($"Reached Maxed Connections : {connections.Count}");
                return;
            }

            if (connections.ContainsKey(data.connectionId)) { //if connection is already connected, skip offer
                Debug.LogWarning($"Already answered this connectionId : {data.connectionId}");
                return;
            }

            //only accept answer if there is a viable set of connections
            if (playerPrefabs.Length > 0) {
                int playerID = connections.Count % playerPrefabs.Length;

                ClientStreams player = playerPrefabs[playerID];
                if ( player.gameObject.scene.rootCount == 0 // if player is a prefab or more connections than prefabs
                    || connections.Count >= playerPrefabs.Length) {
                    player = Instantiate(playerPrefabs[playerID]);
                    player.isDeletable = true; //we should clean up prefabs
                }

                player.SetFullConnection(data.connectionId, this);
                connections.Add(data.connectionId, player); //confirm we will use this connection

                SendAnswer(data.connectionId); //accept offer with an answer
            }

        }

        /// <summary>
        /// Apply Data Channel
        /// </summary>
        public void OnAddChannel(SignalingEventData data) {
            if ( connections.ContainsKey(data.connectionId) ) {
                connections[data.connectionId].SetDataChannel(data);
            }
        }

        private string GetGameID(string sdp) {
            string[] parameters = sdp.Split('\n'); //split on new line
            string customParam = parameters[parameters.Length - 2].Split('=')[1]; //get line before last new line and split on =

            Dictionary<string, string> jsonMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(customParam);
            if (jsonMap != null) {
                string value = ""; //get dictionary value
                jsonMap.TryGetValue("user", out value);
                return value;
            } else {
                return ""; //TODO: check if this is a valid fail case as in do we want to connect to anything?
            }

        }
    }
}

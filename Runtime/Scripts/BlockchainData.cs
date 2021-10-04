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
using Newtonsoft.Json;

namespace FusedVR.VRStreaming {
    public static class BlockchainData
    {
        #region Constants
        /// <summary>
        /// Enums for the Block Chain Data Events. Use DataEvent.ToString() to convert to event name
        /// </summary>
        public enum DataEvents {
            account, sign, sendTx, setContract, runContract
        }
        #endregion

        //static event that be listened to to hear when data has been returned from the blockchain based on queries
        public delegate void OnCryptoData(DataEvents evt, string result);
        public static OnCryptoData CryptoEvent;

        #region Methods
        /// <summary>
        /// Gets The Ethereum Account from Metamask (over the data channel) and will return the public key
        /// </summary>
        public static void GetAccount() {
            SendEvent(DataEvents.account, "");
        }

        /// <summary>
        /// Based on the connected metamask account, it will sign the message and return the hash
        /// </summary>
        public static void Signature(string message) {
            SendEvent(DataEvents.sign, message);
        }

        /// <summary>
        /// Sends a simple transaction to a selected address with a hex value
        /// for example {to : 0x74BAA21278E661eCea04992d5e8fBE6c29cF6f64 , value : 0x100000000000000}
        /// TODO : this method can be expanded to include a Data field for smart contracts
        /// </summary>
        public static void SendTransaction(string to, string value) {
            Dictionary<string, string> payload = new Dictionary<string, string> {
                ["to"] = to,
                ["value"] = value
            };
            
            SendEvent( DataEvents.sendTx, JsonConvert.SerializeObject(payload) );
        }

        /// <summary>
        /// Sends a transaction to register the contract with the client based on the address and abi
        /// This contract once registered can then be called while the client is connected
        /// </summary>
        public static void RegisterContract(string contractAddress, string abi) {
            Dictionary<string, string> payload = new Dictionary<string, string> {
                ["addr"] = contractAddress,
                ["abi"] = abi
            };

            SendEvent(DataEvents.setContract, JsonConvert.SerializeObject(payload));
        }

        /// <summary>
        /// Run a smart contract that has already been Registered With the Client
        /// Pass the relevant function name and arguments
        /// </summary>
        public static void RunTransaction(string contractAddress, string functionName, string[] args) {
            Dictionary<string, object> payload = new Dictionary<string, object> {
                ["addr"] = contractAddress,
                ["fn"] = functionName,
                ["args"] = args
            };

            SendEvent(DataEvents.runContract, JsonConvert.SerializeObject(payload));
        }

        /// <summary>
        /// Generic Send Event method that works with the Broadcaster to send the message over the data channel
        /// </summary>
        private static void SendEvent(DataEvents evt, string payload) {
            VRBroadcast.Instance.BroadcastDataMessage(evt.ToString(), payload);
            VRInputManager.CryptoEvent += Results;
        }

        /// <summary>
        /// Method to recieve the data from the Metamask wallet from the broadcaster
        /// </summary>
        private static void Results(DataEvents evt, string result) {
            VRInputManager.CryptoEvent -= Results;
            CryptoEvent?.Invoke(evt, result);
        }
        #endregion

    }
}

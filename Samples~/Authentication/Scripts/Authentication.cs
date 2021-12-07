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
using System.Collections;
using UnityEngine;
using Newtonsoft.Json;

namespace FusedVR.VRStreaming {
    /// <summary>
    /// Web3 Authentication Sample with the FusedVR ERC 1155 token
    /// </summary>
    public class Authentication : MonoBehaviour
    {
        //This contract is live on the polygon network
        public const string FUSEDVR_ERC_1155 = "0x4d66898952d879c27bdadb5876f38e38a083eff8";

        [Tooltip("Application Binary Interface for the Smart Contract")]
        public TextAsset abi;

        [Tooltip("The NFT ID that is store publically on Github")]
        public string NFT_ID = "1";

        private ClientStreams myStream; //represents the client that connects

        public GameObject noNFT; //what to show without an NFT
        public GameObject withNFT; //what to show with the NFT

        // Start is called before the first frame update
        void OnEnable()
        {
            //listen for when client joins
            ClientStreams.OnClientAdded += OnClientStreamAdd;
            ClientStreams.OnClientLeft += OnClientStreamLeft;
        }

        private void OnDisable() {
            //no longer listening for client
            ClientStreams.OnClientAdded -= OnClientStreamAdd;
            ClientStreams.OnClientLeft -= OnClientStreamLeft;
        }

        void OnClientStreamAdd(ClientStreams player) {
            if (myStream == null) {
                myStream = player; //save player for later
            }

            BlockchainData.CryptoEvent += Result; //listens for Crypto Events from Clients
            StartCoroutine(OnBlockchainActive());
        }

        IEnumerator OnBlockchainActive() {
            yield return new WaitForSeconds(0.5f); //wait half a second to let client register
            BlockchainData.RegisterContract(FUSEDVR_ERC_1155, abi.text); // call register first to make sure it is saved
        }

        void OnClientStreamLeft(ClientStreams player) {
            if (myStream == player) {
                myStream = null; //on player leave, de-register the player
            }
            BlockchainData.CryptoEvent -= Result; //listens for Crypto Events from Clients
        }

        private void OnAccount(string account) {
            List<string> paras = new List<string> {
                account,
                NFT_ID //nft id
            };

            BlockchainData.RunTransaction(FUSEDVR_ERC_1155, "balanceOf", paras.ToArray()); //call balanceOf function
        }

        /// <summary>
        /// Called from Crypto Event Handler when balanceOf function is called
        /// If the NFT has a balance, then we can show content, otherwise ask for a purchase
        /// </summary>
        private void OnBalance(string result) {
            Dictionary<string, string> map = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            if (map["hex"].StartsWith("0x") ){
                uint num = uint.Parse(map["hex"].Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier);

                bool hasNFT = num > 0;
                noNFT.SetActive(!hasNFT);
                withNFT.SetActive(hasNFT);
            }
        }

        /// <summary>
        /// Event Handler for listening for Crypto Events from Client
        /// These events are triggered from the Button Press events above
        /// </summary>
        void Result(BlockchainData.DataEvents evt, string result) {
            if (evt == BlockchainData.DataEvents.account) {
                OnAccount(result); //callback for account
            }

            if (evt == BlockchainData.DataEvents.setContract) {
                BlockchainData.GetAccount(); //after registration, get the account
            }

            if (evt == BlockchainData.DataEvents.runContract) {
                OnBalance(result); //callback for balance
            }
        }
    }
}

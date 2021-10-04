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
using UnityEngine;
using UnityEngine.UI;

namespace FusedVR.VRStreaming {
    /// <summary>
    /// Crypto UI is responsible for handling the UI button presses and then sending the 
    /// correct to the Clients to start a Crypto Blockchain Transaction
    /// </summary>
    public class CryptoUI : MonoBehaviour {

        #region Public Properties
        public Text accountText;

        public InputField message;

        public InputField toAddress;
        public InputField ethereum;

        public InputField smartContract;
        public TextAsset abi;

        public InputField functionName;
        public InputField param1;
        public InputField param2;
        #endregion

        #region Public Button Events

        /// <summary>
        /// Get the Public Key Address from the Web Browser
        /// </summary>
        public void OnButtonGetAccount() {
            BlockchainData.GetAccount();
        }

        /// <summary>
        /// Sign raw test message using the private key from the client
        /// </summary>
        public void OnButtonSignMessage() {
            BlockchainData.Signature(message.text);
        }

        /// <summary>
        /// Write an Ethereum transaction that sends ether (or equivalent main asset) between players
        /// </summary>
        public void SendEthereum() {
            long geth = (long)(float.Parse(ethereum.text) * Mathf.Pow(10, 18));
            Debug.LogError(geth.ToString("x"));
            BlockchainData.SendTransaction(toAddress.text, "0x" + geth.ToString("x"));
        }

        /// <summary>
        /// Register an ERC-20 token smart contract with the client based on the ABI
        /// This will allow a fast way to run smart contract functions on the client
        /// </summary>
        public void RegisterERC20() {
            BlockchainData.RegisterContract(smartContract.text, abi.text);
        }

        /// <summary>
        /// Run Smart Contract function with associated parameter on the client
        /// This can be either a Read or Write Transaction
        /// i.e. name / symbol / balanceOf
        /// But also transfer(to address, value)
        /// </summary>
        public void RunSmartContract() {
            List<string> paras = new List<string>();
            if (param1.text.Length > 0) paras.Add(param1.text);
            if (param2.text.Length > 0) paras.Add(param2.text);
            BlockchainData.RunTransaction(smartContract.text, functionName.text, paras.ToArray());
        }
        #endregion

        private void Start() {
            BlockchainData.CryptoEvent += Result; //listens for Crypto Events from Clients
        }

        /// <summary>
        /// Event Handler for listening for Crypto Events from Client
        /// These events are triggered from the Button Press events above
        /// </summary>
        void Result(BlockchainData.DataEvents evt, string result) {
            if (evt == BlockchainData.DataEvents.account) {
                accountText.text = result;
            }

            Debug.LogError("Event : " + evt.ToString());
            Debug.LogError("Result : " + result);
        }
    }
}


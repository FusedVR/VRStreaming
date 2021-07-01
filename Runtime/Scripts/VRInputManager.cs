/**
 * Copyright 2021 Vasanth Mohan. All rights and licenses reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 */

using System;
using Unity.RenderStreaming;
using UnityEngine;
using UnityEngine.Events;

namespace FusedVR.VRStreaming
{
    /// <summary>
    /// The manager class is responsible for handling the source data from the client and distributing accordingly
    /// This data will be recieved from the Broadcast channel on the Data Channel
    /// Since this is manages the Data Channel, it can be used to send and recieve data from the client
    /// </summary>
    public class VRInputManager : InputChannelReceiverBase
    {
        #region Constants
        /// <summary>
        /// Data Source for positional / rotational VR Data i.e. Head Left Hand or Right Hand
        /// </summary>
        public enum Source {
            Head,
            Left,
            Right
        }

        /// <summary>
        /// This is the ID for getting Controller Input Data from the client
        /// </summary>
        public const int VR_CONTROLLER_ID = 5;

        /// <summary>
        /// The Base ID for Recieving Positional / Rotational Data from the client
        /// BASEID + 0 = Head
        /// BASEID + 1 = Left Hand
        /// BASEID + 2 = Right Hand
        /// </summary>
        public const int VR_BASE_ID = 6;
        #endregion

        #region Events
        /// <summary>
        /// Wrapper class for the Unity Event to pass custom data. 
        /// </summary>
        [System.Serializable]
        public class VRPoseData : UnityEvent<Source, Vector3, Vector3> {

        }

        /// <summary>
        /// The Unity Event is responsible for sending data related to the positional or rotational data from the client
        /// </summary>
        public VRPoseData VRPoseEvent;

        /// <summary>
        /// C# Event responsible for sending Controller Data that is recieved from the client
        /// </summary>
        public delegate void OnControllerDataRecieved(int handID, int buttonID, bool pressed, bool touched);
        public static OnControllerDataRecieved ControllerDataEvent;
        #endregion

        #region Methods
        /// <summary>
        /// Public Method to send string data over the data channel for the client to respond to. 
        /// Currently, the client does not do anything with data that is recieved and any such action would need to be implemented
        /// </summary>
        public void SendData(string msg)
        {
            base.Channel.Send(msg);
        }

        /// <summary>
        /// Recieve Data from the client via the Data Channel
        /// Based on the first byte, this method determines how to process the data and what events to expose for the application to listen to
        /// </summary>
        protected override void OnMessage(byte[] bytes)
        {
            int index = bytes[0];

            if (index == VR_CONTROLLER_ID)
            { //VR Controller Data
                ControllerDataEvent?.Invoke(bytes[1], bytes[2], BitConverter.ToBoolean(bytes, 3), BitConverter.ToBoolean(bytes, 4));
            }

            if (index >= VR_BASE_ID)
            { //VR data
                Vector3 pos = new Vector3(BitConverter.ToSingle(bytes, 1),
                    BitConverter.ToSingle(bytes, 9), BitConverter.ToSingle(bytes, 17));

                Vector3 rot = new Vector3(Mathf.Rad2Deg * BitConverter.ToSingle(bytes, 25),
                    Mathf.Rad2Deg * BitConverter.ToSingle(bytes, 33), Mathf.Rad2Deg * BitConverter.ToSingle(bytes, 41));

                VRPoseEvent.Invoke((Source)(index - VR_BASE_ID), pos, rot);
            }
        }
        #endregion

    }
}
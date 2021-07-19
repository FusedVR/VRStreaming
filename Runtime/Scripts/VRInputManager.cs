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
        /// The Data Format  that we are getting from the VR headset
        /// </summary>
        public enum VRDataType {
            PosRot,
            Button,
            Axis
        }

        /// <summary>
        /// Data Source for positional / rotational VR Data i.e. Head Left Hand or Right Hand
        /// </summary>
        public enum Source {
            Head,
            Left,
            Right
        }

        /// <summary>
        /// The Base ID for Recieving VR Data from the Web client
        /// </summary>
        public const int VR_DEVICE_ID = 6;
        #endregion

        #region Events
        /// <summary>
        /// Wrapper class for the Unity Event to pass custom data. 
        /// </summary>
        [System.Serializable]
        public class VRPoseData : UnityEvent<Source, Vector3, Quaternion> {

        }

        /// <summary>
        /// The Unity Event is responsible for sending data related to the positional or rotational data from the client
        /// </summary>
        public VRPoseData VRPoseEvent;

        /// <summary>
        /// C# Event responsible for sending Controller Button Data that is recieved from the client
        /// </summary>
        public delegate void OnButtonDataRecieved(Source handID, int buttonID, bool pressed, bool touched);
        public static OnButtonDataRecieved ButtonDataEvent;


        /// <summary>
        /// C# Event responsible for sending Controller Axis Data that is recieved from the client
        /// </summary>
        public delegate void OnAxisDataRecieved(Source handID, int buttonID, float xaxis, float yaxis);
        public static OnAxisDataRecieved AxisDataEvent;
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
            if (bytes[0] == VR_DEVICE_ID) //VR Device Data
            { 
                int data_type = bytes[1]; //get input data source
                switch ( (VRDataType) data_type) {
                    case VRDataType.PosRot:
                        Source device_type = (Source) bytes[2]; //get source
                        Vector3 pos = new Vector3(BitConverter.ToSingle(bytes, 3),
                            BitConverter.ToSingle(bytes, 11), BitConverter.ToSingle(bytes, 19));

                        Quaternion rot = new Quaternion(BitConverter.ToSingle(bytes, 27) , BitConverter.ToSingle(bytes, 35),
                            -BitConverter.ToSingle(bytes, 43), -BitConverter.ToSingle(bytes, 51)); //flip z and w due to different coordinated system
                        VRPoseEvent.Invoke(device_type, pos, rot);
                        break;
                    case VRDataType.Button:
                        ButtonDataEvent?.Invoke((Source) bytes[2], bytes[3], BitConverter.ToBoolean(bytes, 4), 
                            BitConverter.ToBoolean(bytes, 5));
                        break;
                    case VRDataType.Axis:
                        if ( BitConverter.ToBoolean(bytes, 3) || BitConverter.ToBoolean(bytes, 12) ) { //if trackpad changed
                            AxisDataEvent?.Invoke((Source)bytes[2], 1, BitConverter.ToSingle(bytes, 4),
                                BitConverter.ToSingle(bytes, 13));
                        }

                        if (BitConverter.ToBoolean(bytes, 21) || BitConverter.ToBoolean(bytes, 30)) { //if joystick changed
                            AxisDataEvent?.Invoke((Source)bytes[2], 0, BitConverter.ToSingle(bytes, 22),
                                BitConverter.ToSingle(bytes, 31));
                        }

                        break;
                }
                
            }
        }
        #endregion

    }
}
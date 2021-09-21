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
using System.Text;
using Unity.RenderStreaming;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.Events;

namespace FusedVR.VRStreaming {
    /// <summary>
    /// The manager class is responsible for handling the source data from the client and distributing accordingly
    /// This data will be recieved from the Broadcast channel on the Data Channel
    /// Since this is manages the Data Channel, it can be used to send and recieve data from the client
    /// </summary>
    public class VRInputManager : InputChannelReceiverBase {

        #region Properties
        /// <summary>
        /// Enables whether the Simple Camera Controls script is added to the object at start
        /// </summary>
        [Tooltip("Enables whether the Simple Camera Controls script is added to the object at start")]
        public bool enableKeyboardTouchControls = true;

        /// <summary>
        /// Camera that are used for VR Render Streaming
        /// </summary>
        [Tooltip("The Cameras that are responsible for VR Render Streaming")]
        public VRCamStream VRCameras;

        /// <summary>
        /// Remote Input class to capture Remote Input and incorporate into Unity Input System
        /// </summary>
        private RemoteInput remoteInput;

        /// <summary>
        /// Utilized solely for 2D Camera Controls (Mouse, Touch , Keyboard)
        /// </summary>
        private CameraControls camControls;
        #endregion

        #region Constants
        /// <summary>
        /// The Data Format  that we are getting from the VR headset
        /// </summary>
        public enum VRDataType {
            PosRot,
            Button,
            Axis,
            Display,
            EnterVR,
            ExitVR
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
        /// The Base ID for Recieving VR Data from the Web Client
        /// </summary>
        public const int VR_DEVICE_ID = 6;

        /// <summary>
        /// Crypto ID for Recieving Crypto Data from the Web Clinet
        /// </summary>
        public const int CRYPTO_DEVICE_ID = 7;
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
        public OnButtonDataRecieved ButtonDataEvent;


        /// <summary>
        /// C# Event responsible for sending Controller Axis Data that is recieved from the client
        /// </summary>
        public delegate void OnAxisDataRecieved(Source handID, int buttonID, float xaxis, float yaxis);
        public OnAxisDataRecieved AxisDataEvent;

        /// <summary>
        /// C# Event responsible for sending the status of whether the client is in VR mode or not
        /// isVR = true means that player just entered VR; else they exited
        /// </summary>
        public delegate void OnVRMode(bool isVR);
        public OnVRMode VRModeEvent;

        /// <summary>
        /// C# Event responsible for returning Crypto Data from Client
        /// </summary>
        public delegate void OnCryptoData(BlockchainData.DataEvents evt, string result);
        public static OnCryptoData CryptoEvent;
        #endregion

        #region Methods

        private void Awake() {
            if (enableKeyboardTouchControls) {
                camControls = gameObject.AddComponent<CameraControls>();
            }
        }

        public override void SetChannel(string connectionId, RTCDataChannel channel) {
            if (channel == null) {
                if (remoteInput != null) {
                    remoteInput.Dispose();
                    camControls.RemoveDevices();
                    remoteInput = null;
                }
            } else {
                remoteInput = RemoteInputReceiver.Create();
                camControls.AddDevice(remoteInput.RemoteKeyboard);
                camControls.AddDevice(remoteInput.RemoteMouse);
                camControls.AddDevice(remoteInput.RemoteTouchscreen);
                channel.OnMessage += remoteInput.ProcessInput;
            }

            base.SetChannel(connectionId, channel);
        }

        /// <summary>
        /// Public Method to send string data over the data channel for the client to respond to. 
        /// Currently, the client does not do anything with data that is recieved and any such action would need to be implemented
        /// </summary>
        public void SendData(string msg) {
            base.Channel.Send(msg);
        }

        /// <summary>
        /// Recieve Data from the client via the Data Channel
        /// Based on the first byte, this method determines how to process the data and what events to expose for the application to listen to
        /// </summary>
        protected override void OnMessage(byte[] bytes) {
            if (bytes[0] == VR_DEVICE_ID) //VR or Crypto Device Data
            {
                int data_type = bytes[1]; //get input data source
                switch ((VRDataType)data_type) {
                    case VRDataType.PosRot:
                        Source device_type = (Source)bytes[2]; //get source
                        Vector3 pos = new Vector3(BitConverter.ToSingle(bytes, 3),
                            BitConverter.ToSingle(bytes, 11), BitConverter.ToSingle(bytes, 19));

                        Quaternion rot = new Quaternion(BitConverter.ToSingle(bytes, 27), BitConverter.ToSingle(bytes, 35),
                            -BitConverter.ToSingle(bytes, 43), -BitConverter.ToSingle(bytes, 51)); //flip z and w due to different coordinated system
                        VRPoseEvent.Invoke(device_type, pos, rot);
                        break;
                    case VRDataType.Button:
                        ButtonDataEvent?.Invoke((Source)bytes[2], bytes[3], BitConverter.ToBoolean(bytes, 4),
                            BitConverter.ToBoolean(bytes, 5));
                        break;
                    case VRDataType.Axis:
                        if (BitConverter.ToBoolean(bytes, 3) || BitConverter.ToBoolean(bytes, 12)) { //if trackpad changed
                            AxisDataEvent?.Invoke((Source)bytes[2], 1, BitConverter.ToSingle(bytes, 4),
                                BitConverter.ToSingle(bytes, 13));
                        }

                        if (BitConverter.ToBoolean(bytes, 21) || BitConverter.ToBoolean(bytes, 30)) { //if joystick changed
                            AxisDataEvent?.Invoke((Source)bytes[2], 0, BitConverter.ToSingle(bytes, 22),
                                BitConverter.ToSingle(bytes, 31));
                        }

                        break;
                    case VRDataType.Display:
                        int width = (int)BitConverter.ToUInt32(bytes, 2);
                        int height = (int)BitConverter.ToUInt32(bytes, 6);
                        //foreach (Camera cam in VRCameras) {
                        //    cam.targetTexture.Release();
                        //    cam.targetTexture.width = width;
                        //    cam.targetTexture.height = height;
                        //}
                        //TODO: need to find proper way to resize texture based on data so that the video channel updates

                        break;
                    case VRDataType.EnterVR:
                        VRModeEvent?.Invoke(true);
                        break;
                    case VRDataType.ExitVR:
                        VRModeEvent?.Invoke(false);
                        break;
                    default:
                        Debug.LogError(Encoding.UTF8.GetString(bytes, 1, bytes.Length-1)); //ignore header byte
                        break;
                }
            } else if ( bytes[0] == CRYPTO_DEVICE_ID ) {
                CryptoEvent?.Invoke( (BlockchainData.DataEvents) bytes[1] , 
                    Encoding.UTF8.GetString(bytes, 2, bytes.Length - 2)); //subtract 2 for the header bits
            }
        }
        #endregion

    }
}
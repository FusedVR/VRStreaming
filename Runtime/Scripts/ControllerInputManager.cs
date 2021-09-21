/**
 * Copyright 2021 Vasanth Mohan. All rights and licenses reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 */

using UnityEngine;
using UnityEngine.Events;

namespace FusedVR.VRStreaming
{
    /// <summary>
    /// The manager class for Controller Input Data i.e. Trigger, Grips Pressed, etc.
    /// This class provides an abstraction over the raw data ControllerDataEvent from VRInputManager 
    /// </summary>
    [RequireComponent(typeof(VRInputManager))]
    public class ControllerInputManager : MonoBehaviour
    {
        #region Enums
        /// <summary>
        /// Enum to represent handedness
        /// </summary>
        public enum Hand
        {
            Left,
            Right
        }

        /// <summary>
        /// Enum to represent data options for buttons
        /// </summary>
        public enum Button
        {
            Trigger,
            Grip,
            AButton,
            BButton,
            Joystick,
            Trackpad
        }
        #endregion

        #region Events
        /// <summary>
        /// The class wrapper for the VRControllerData Unity Event
        /// </summary>
        [System.Serializable]
        public class VRButtonData : UnityEvent<Hand, Button, bool, bool> {

        }

        /// <summary>
        /// The main controller input Unity Event. You may listen to this event via code or via the inspector just like any other Unity Event.
        /// </summary>
        public VRButtonData VRButtonEvent;

        /// <summary>
        /// The class wrapper for the VRControllerData Unity Event
        /// </summary>
        [System.Serializable]
        public class VRAxisData : UnityEvent<Hand, Button, float, float> {

        }

        /// <summary>
        /// The main controller input Unity Event. You may listen to this event via code or via the inspector just like any other Unity Event.
        /// </summary>
        public VRAxisData VRAxisEvent;
        #endregion

        private VRInputManager input; // handles recieving the data

        #region Methods
        // Start is called before the first frame update
        void Start()
        {
            input = GetComponent<VRInputManager>();
            input.ButtonDataEvent += ButtonEvents; //start listening
            input.AxisDataEvent += AxisEvents; //start listening
        }
        
        // OnDestroy is called when the game object is about to be destroyed in the scene
        private void OnDestroy() {
            input.ButtonDataEvent -= ButtonEvents; //end listening
            input.AxisDataEvent -= AxisEvents; //end listening
        }

        /// <summary>
        /// Callback function for Raw Data from VRInputManager ControllerDataEvent
        /// </summary>
        void ButtonEvents(VRInputManager.Source handID, int buttonID, bool pressed, bool touched)
        {
            VRButtonEvent?.Invoke((Hand)(handID -1), (Button)buttonID, pressed, touched); //invoke the Unity Event
        }

        /// <summary>
        /// Callback function for Raw Data from VRInputManager ControllerDataEvent
        /// </summary>
        void AxisEvents(VRInputManager.Source handID, int buttonID, float x, float y) {
            Button axisButton = (Button)((int)Button.Joystick + buttonID);            
            VRAxisEvent?.Invoke((Hand)(handID - 1), axisButton, x, y); //invoke the Unity Event
        }
        #endregion
    }
}
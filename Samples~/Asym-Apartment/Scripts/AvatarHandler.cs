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


namespace FusedVR.VRStreaming {
    /// <summary>
    /// Avatar Handler is responsible for swapping the avatar dependent on whether the player is in 2D or 3D mode
    /// </summary>
    public class AvatarHandler : MonoBehaviour {

        #region Properties
        public VRInputManager manager; //property for the input manager. set in inspector

        public GameObject VRAvatar; //the Game Object representing the VR Avatar
        public GameObject NonVRAvatar; //the Game Object representing the Non VR Avatar
        #endregion

        // Start is called before the first frame update
        void Start() {
            manager.VRModeEvent += OnVRModeChange; //listen to the VR Change Event for the player
            NonVRAvatar.SetActive(true); //on start, show non VR avatar 
            VRAvatar.SetActive(false); // on start, hide non VR avatar
        }

        /// <summary>
        /// Event Handler for when player switches between VR and Non VR Mode
        /// </summary>
        void OnVRModeChange(bool status) {
            VRAvatar.SetActive(status);
            NonVRAvatar.SetActive(!status);
        }
    }
}


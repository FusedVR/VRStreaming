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

namespace FusedVR.VRStreaming
{
    /// <summary>
    /// Apply the VR Positional and Rotational Data from the Client onto the Server.
    /// This data originates from the VRInputManager
    /// </summary>
    public class ApplyVRData : MonoBehaviour
    {
        public VRInputManager.Source index; //applied via the Inspector to indicate which data source to listen to

        /// <summary>
        /// Apply the VR Positional and Rotational Data from the Client onto the Server.
        /// </summary>
        public void ApplyData(VRInputManager.Source id, Vector3 position, Quaternion rotation)
        {
            if (index == id) //check if the data is from the correct source
            {
                transform.localPosition = new Vector3(position.x, position.y, -position.z); //note that z data is reversed on WebXR
                transform.localRotation = rotation; //apply rotation - note coordinate system change was already applied
            }
        }
    }
}
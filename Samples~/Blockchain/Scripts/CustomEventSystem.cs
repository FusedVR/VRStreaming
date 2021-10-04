/**
 * Copyright 2021 Vasanth Mohan. All rights and licenses reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 */

using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.EnhancedTouch;

namespace FusedVR.VRStreaming {
    /// <summary>
    /// This is needed as a result of this PR and documentation
    /// https://github.com/Unity-Technologies/UnityRenderStreaming/pull/339
    /// https://github.com/Unity-Technologies/UnityRenderStreaming/blob/develop/com.unity.renderstreaming/Documentation~/browser_input.md
    /// This Class Overrides some functionality in order to make it possible to run this Demo in the background
    /// Additionally, based on documentation you will need to lock the Game Input to the Editor
    /// This happens via Window > Analysis > Input Debugger > Options > Lock input to Game View
    /// </summary>
    public class CustomEventSystem : EventSystem {
        protected override void Awake() {
            base.Awake();
            EnhancedTouchSupport.Enable(); //supress touch input 
            unsafe {
                InputSystem.onDeviceCommand += InputSystemOnDeviceCommand;
            }
        }

        private static unsafe long? InputSystemOnDeviceCommand(InputDevice device, InputDeviceCommand* command) {
            if (command->type != QueryCanRunInBackground.Type) {
                // return null is skip this evaluation
                return null;
            }

            ((QueryCanRunInBackground*)command)->canRunInBackground = true;
            return InputDeviceCommand.GenericSuccess;
        }

        protected override void OnApplicationFocus(bool hasFocus) {
            //Do not change focus flag on eventsystem
        }
    }
}

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
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace FusedVR.VRStreaming {
    /// <summary>
    /// This class is to temporarily solve for a bug where Keyboard Input does not seem to be passed along via Input System
    /// Filed this Issue : https://github.com/Unity-Technologies/UnityRenderStreaming/issues/542
    /// After Issue is fixed, there will be less reason for this to exist
    /// (Although something to consider for the future is how to handle Copy / Paste)
    /// </summary>
    public class RemoteInput : MonoBehaviour {
        public VRInputManager manager;

        private Keyboard keyboard;

        private InputField textField = null;

        // Start is called before the first frame update
        void Start() {
            manager.onDeviceChange += Manager_onDeviceChange;
        }

        private void Update() {
            textField = EventSystem.current.currentSelectedGameObject?.GetComponent<InputField>();

            if (textField != null && keyboard != null && keyboard.backspaceKey.wasPressedThisFrame) {
                if (textField.isFocused && textField.caretPosition != 0) {
                    string text = textField.text;
                    textField.text = text.Substring(0, textField.caretPosition - 1)
                        + text.Substring(textField.caretPosition, text.Length - textField.caretPosition);
                    if (textField.caretPosition != textField.text.Length) {
                        textField.caretPosition--;
                    }
                }
            }
        }

        private void Manager_onDeviceChange(InputDevice device, InputDeviceChange change) {
            if (change == InputDeviceChange.Added) {
                switch (device) {
                    case Keyboard keyboard:
                        this.keyboard = keyboard;
                        keyboard.onTextInput += Current_onTextInput;
                        break;
                }
            } else if (change == InputDeviceChange.Removed) {
                switch (device) {
                    case Keyboard keyboard:
                        this.keyboard = null;
                        keyboard.onTextInput -= Current_onTextInput;
                        break;
                }
            }
        }

        private void Current_onTextInput(char obj) {
            if (textField.isFocused) {
                textField.text += obj;
                textField.caretPosition++;
            }

        }
    }
}



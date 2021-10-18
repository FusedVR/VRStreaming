/**
 * Copyright 2021 Vasanth Mohan. All rights and licenses reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 */

using Unity.RenderStreaming;
using Unity.WebRTC;
using UnityEngine;

/// <summary>
/// Microphone Receiver class is responsible for receiving microphone data from the client
/// </summary>
[RequireComponent(typeof(ReceiveAudioViewer))]
[RequireComponent(typeof(AudioSource))]
public class MicReceiver : MonoBehaviour {

    //TODO : This is only working when VRBroadcast SetFullConnection needs to be disabled
    //Comment line 125 out. For some reason there is a collision between the reciever and senders

    private AudioSource source;

    private void OnEnable() {
        GetComponent<ReceiveAudioViewer>().OnUpdateReceiveAudioClip += OnAudioClip;
        GetComponent<ReceiveAudioViewer>().OnStartedStream += OnStream;
        source = GetComponent<AudioSource>();
    }

    private void OnDisable() {
        GetComponent<ReceiveAudioViewer>().OnUpdateReceiveAudioClip -= OnAudioClip;
    }

    private void Update() {
        AudioStreamTrack track = GetComponent<ReceiveAudioViewer>().Track as AudioStreamTrack;
        Debug.LogError("UPDATE : " + track.Renderer);
        //TODO this is replaying old clip if not ready

        if ( !source.isPlaying && track.Renderer != null) {
            Debug.LogError("SET CLIP");
            source.clip = track.Renderer;
            source.Play();
        }
    }

    void OnAudioClip(AudioClip clip) {
        Debug.LogError("GOT CLIP");
        source.clip = clip;
    }

    void OnStream(string connectionId) {
        Debug.LogError("GOT STREAM START "  + GetComponent<ReceiveAudioViewer>().Clip);

    }

}

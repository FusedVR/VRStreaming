# Changelog
All notable changes to com.unity.renderstreaming package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.6.0] - 2021-10-04

### Added

- Added a **NEW** Sample : BlockChain Testing to quickly test Web3 integration with Client
- Added Support for Smart Contracts to be registered with Clients in order to easily make calls on the Blockchain
- Two Events to Client Streams for when Client is added / leaves

### Fixed

- Fixed Missing VRTK scripts that were required for the Apartment Sample
- Updated VRInput Manager to use the onDeviceChange Event
- Fixed Mouse Input to be Relative to viewable area

## [0.5.1] - 2021-09-20

### Added

- Added a **NEW** Sample : Asymmetrical Apartment for testing asymmetrical multiplayer
- Add CameraControls.cs to allow for the Camera to be controlled in 2D experiences
- Added Supported Versions to README : https://github.com/FusedVR/VRStreaming/issues/24
- Added enableKeyboardTouchControls to VRInputManager to control whether to use Camera Controls

### Changed

- Fixed HDRPVR Sample to reference correct Render Streaming Prefab
- Fixed Mouse & Camera Input from the Web Client to be correctly sent to Unity SDK using RemoteInput class
- Removed WebXR controls from client and instead send input to be handled by server 
- Fixed Overriding Prefab Positional & Rotational Data : https://github.com/FusedVR/VRStreaming/issues/19
- Rotation from headset is applied in local rotation instead of global rotation


## [0.5.0] - 2021-09-13

### Added

- Added a new ClientStreamer.cs file that is responsible for managing the streams for a given player
- Added a new BlockchainData.cs script to make it easier to integrate with new Block Chain integrations with client
- It is now possible to send Blockchain transactions to each client and wait for a response from the blockchain

### Changed

- **MAJOR** Revised the Render Streaming Service prefab. The new prefab is a single game object and now requires player prefabs, which will be spawned when a player connects. This change was made to support multi-players / devices
- Upgraded to Render Streaming version : 3.1.0-exp.1
- VRInputManager now listens for Blockchain data and passes along to the BlockchainData script. 
- VRBroadcast now exposes a Max Connection / Player Prefab field in addition to GameID (see README for more details).

## [0.4.1] - 2021-08-23

### Changed

- Reverting changes where left and right eye are combined due to encoder bug that limits the size of the video render texture. This is expected to be fixed soon and once fixed, will revert this change to a single video stream. 

## [0.4.0] - 2021-08-17

### Changed

- **MAJOR** Combined left eye and right eye into a single render texture on one video track to increase encoding performance & maintain synconization between eyes
- Removed Video Resizing as this implementation needs to be re-evaluated
- Increased default resolution to 2700 x 1500 (both eyes)

### Added

- Added new VRCamStreaming component that exposes bitrate and framerate encoder parameters

## [0.3.0] - 2021-08-10

### Changed

- Listen for Camera Resolution Data about the connected VR headset and dynamically update resolution accordingly
- Removed Editor Script to auto-import HDRP as it was triggering an infinite import loop

### Added

- Added Package Requirement for Newton JSON
- Added New GameID to allow the ability to filter for clients looking for a specific game

## [0.2.1] - 2021-07-18

### Changed

- Updated Rotation data to use WebXR Quaternions instead of Euler Angles. This helps avoid Gimbal Lock in certain cases when using archery

### Added

- Added New VRTK Sample to highlight how to use Input from the WebXR Client. Can be imported via the Package Manager Samples

## [0.2.0] - 2021-07-12

### Changed

- VR Controller Data Input Format from client for WebXR data

### Added

- VR Controller Data for Axis input i.e. Trackpad and Joystick
- HDRP VR Render Streaming Sample

## [0.1.1] - 2021-07-04

### Added

- VR Broadcaster Script to Replace Remote Render Streaming default Broadcaster

### Fixed

- VR Broadcaster enables streaming to just one VR headset to avoid eavesdropping

## [0.1.0] - 2021-07-03

- Initial Release of Experimental VR Render Streaming
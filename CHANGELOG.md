# Changelog
All notable changes to com.unity.renderstreaming package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

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
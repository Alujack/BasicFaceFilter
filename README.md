# Basic Face Filter

A Unity AR face filter app for iOS, built with AR Foundation 6.3 and ARKit. Detects the user's face via the front camera and overlays themed prop filters (cat, devil, goblin, classy gentleman) that the user can switch between with on-screen buttons.

## Built with

- Unity 6 (6000.3.16f1)
- AR Foundation 6.3.4 + Apple ARKit XR Plugin 6.3.4
- Universal Render Pipeline
- TextMesh Pro

## Project structure

- `Assets/Scenes/Basic_Face_Filter.unity` — main AR scene
- `Assets/_BasicFaceFilter/Prefabs/` — face prefab + 8 prop prefabs (cat ears, horns, moustache, etc.)
- `Assets/_BasicFaceFilter/Scripts/FaceTrackingChooser.cs` — custom AR configuration chooser that forces `ARFaceTrackingConfiguration` (front camera) instead of the default world-tracking fallback
- `Assets/_BasicFaceFilter/Scripts/FilterSwitcher.cs` — runtime swap of prop combinations when the user taps a filter button

## Requirements to run

- macOS with Xcode 26+
- iPhone X or newer (A12+ chip / TrueDepth camera), iOS 15+
- Apple Developer account for code signing

## Build steps

1. Open the project in Unity 6.
2. Switch platform to iOS in Build Profiles.
3. Confirm scene `Basic_Face_Filter` is the only enabled scene.
4. Build to a folder.
5. Open the generated `.xcodeproj` in Xcode, sign with your team, run on a physical iPhone.

# HackathonGame

Small Unity narrative detective prototype about a bird investigating feathers in a city park.

## Open

Open this folder as a Unity project. The project uses URP and expects Unity with Universal Render Pipeline package support.

## Build Scene

In Unity, choose:

`Feather Detective > Build Prototype Scene`

Open:

`Assets/FeatherDetective/Scenes/ParkPrototype.unity`

## Controls

- WASD or arrow keys: walk
- Space: hop
- G: glide to a linked perch
- E: inspect a feather or place a selected feather at the nest board
- Tab: cycle the selected collected feather

## Tests

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

## Prototype Goal

Find 12 feathers, compare five species-specific memory presentations, and complete the nest deduction board. The ending reveals the old man stayed at the hospital with his wife.

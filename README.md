# HackathonGame

Small narrative detective prototype about a bird investigating feathers in a city park.

The repository now tracks two directions:

- Unity prototype: a playable 3D park exploration slice.
- Web prototype: a lighter clickable narrative deduction version planned in `web-prototype/`.

## Web Prototype

The current recommended direction is a browser-based clickable deduction prototype. It lowers movement and 3D complexity, while preserving feather memories, clue comparison, the nest board, and the quiet hospital ending.

Read the full web gameplay structure and page flow here:

`web-prototype/README.md`

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

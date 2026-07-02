# Feather Detective Prototype Design

## Overview

Build a 15-minute Unity narrative detective prototype about a small bird investigating mysterious feathers in a calm city park. The player explores, hops, and glides between predefined perches while collecting feather memories. Each memory reveals only one aspect of an event based on the bird species it belongs to. The game never states this rule directly; the player infers it by comparing how different memories behave.

The prototype should feel cozy, non-violent, and quietly emotional. The final reconstruction reveals that an old man stopped feeding the birds because he stayed at the hospital with his wife. The ending is gentle rather than tragic.

## Project Setup

Create a fresh Unity project in this workspace using the Universal Render Pipeline. URP supports the desired low-poly look with clean lighting, soft shadows, color grading, and simple post-processing. The prototype should target desktop play with keyboard and mouse or gamepad-style input.

The project should be small and self-contained. It does not need save files, inventories, dialogue trees, voice acting, cinematic timeline sequences, or a large UI system.

## Player Experience

The player controls a small bird in a compact city park from a fixed third-person camera. The camera is angled slightly above and behind the bird, with a calm composition that keeps exploration readable. Player movement includes walking, hopping, and short glide travel between predefined perch points.

Perches are placed on benches, trees, signs, low rooftops, and other park features. The bird can land on these locations and use them to reach feathers or view environmental changes. Gliding is intentionally constrained to authored perch links so the small prototype remains robust and readable.

## Park Layout

The park is a low-poly scene with saturated but natural colors, clean silhouettes, and soft shadows. It should suggest a small urban green space rather than a wilderness area.

Core areas:

- The old feeding spot near a bench and paved path.
- A central green space with trees, grass, and insects.
- A fountain or small plaza that creates soft ambient sound.
- A signpost and notice board area.
- Low rooftops or awnings on the park edge.
- A visible path or street direction leading toward a hospital.
- A nest hub where the player assembles the final deduction.

The art direction should echo the warmth and openness of A Short Hike, Sable, and Season: A Letter to the Future without copying specific visual assets. The scene should use simple low-poly geometry, readable colors, and restrained post-processing.

## Core Investigation Loop

The player finds feathers in the park. Interacting with a feather briefly changes the world presentation to express that bird's memory. These memories are not cutscenes; the player remains inside the world and receives a sensory fragment.

The player gradually learns that every bird species remembers only one kind of reality:

- Crow remembers sounds.
- Magpie remembers colors.
- Pigeon remembers movement routes.
- Woodpecker remembers impacts, rhythms, and body movement.
- Sparrow remembers atmosphere and environmental tension.

The rules are implicit. The game should not explain them through tutorial text, dialogue, codex entries, or labels such as "Crow equals sound." Interface text may identify collected feathers, but the deduction must come from repeated contrast between memory presentations.

## Feathers And Clues

The park contains 12 feathers. Eight feathers belong to the main mystery and four are optional atmosphere or gentle misdirection. All 12 should reinforce the rule that bird species filter memory differently.

Main mystery feathers:

- Crow feather near the feeding bench: remembers hospital siren, distant wheels, and the old man's soft voice.
- Magpie feather near the bench: preserves a red hospital bracelet, blue thermos, and yellow seed bag while the rest fades gray.
- Pigeon feather along the path: draws the old man's altered route away from the park and toward the street.
- Woodpecker feather by the signpost: shows cane taps, hurried steps, a hand striking the notice board, and the rhythm of packing up.
- Sparrow feather in the trees: changes wind, insect volume, and flock stillness on the morning the birds waited.
- Crow feather near the fountain: remembers a phone ringing and the phrase-like shape of concern without readable words.
- Magpie feather near the notice board: highlights the blue-white hospital icon and a pale visiting-hours color cue.
- Pigeon feather near the park edge: traces repeated trips between hospital direction and home, showing care rather than disappearance.

Optional feathers:

- Sparrow feather near flowers: shows the park becoming quieter after feeding stopped.
- Woodpecker feather on a tree: shows unrelated maintenance rhythms, teaching that impact memories can be ordinary.
- Magpie feather on a rooftop: preserves bright picnic colors unrelated to the mystery.
- Crow feather by the fountain: remembers water, shoes, and children laughing, softening the mood.

## Memory Presentations

Memory effects must be short, interactive-feeling, and non-cinematic.

Crow memories darken or fully fade the screen while spatial audio remains. The player hears layered environmental sounds that point to location, time, and emotional context.

Magpie memories desaturate the world into grayscale except for important colors. The highlighted colors act as clues, such as hospital blue, bracelet red, seed-bag yellow, or thermos blue.

Pigeon memories draw minimal white route lines across the ground. The lines animate from origin to destination and show movement patterns, not motives.

Woodpecker memories shift briefly to a first-person body-motion view. The presentation focuses on footsteps, taps, impacts, and timing. It should feel physical but not violent.

Sparrow memories modify ambience in the current scene. Wind intensity, insect sound, bird flock spacing, and subtle motion change to communicate tension or calm.

## Deduction Board Ending

The nest is the deduction hub. After finding enough main mystery feathers, the player returns to the nest and places feathers onto a simple board-like arrangement made from twigs, leaves, thread, and small park objects.

The player does not choose dialogue. Instead, they place feathers into deduction slots that represent the reconstruction:

- Last feeding day.
- What changed in the park.
- Where the old man went.
- Why the birds waited.
- What the evidence actually means.

The board accepts sensible placements from the eight main feathers. When the reconstruction is complete, the park presentation gently resolves: the distant hospital direction becomes clear, the remembered colors and routes align, and the old feeding spot feels warm rather than abandoned.

The ending text or final environmental beat should communicate that the old man stayed at the hospital with his wife. There is no crime, death, or betrayal. The emotional center is absence, care, and the birds slowly understanding.

## Systems

### Player Controller

The player controller supports walking on the ground, hopping, interacting with nearby feathers, and gliding between authored perches. The glide system should use explicit perch nodes and links rather than free-flight physics.

### Perch Network

Perches are scene objects with landing positions. Nearby perches can be selected and reached by glide. The system should support benches, trees, signs, rooftops, and nest points.

### Feather Data

Each feather has a species, location, clue role, memory effect, and deduction-board compatibility. A data-driven representation is preferred so feathers can be authored without duplicating logic.

### Memory Playback

A central memory controller receives a feather interaction and triggers the correct presentation layer. Each bird memory type should be implemented as a separate effect component with a shared interface.

### Deduction Board

The nest board tracks collected feathers and slot placements. It validates the final arrangement and triggers the ending when the correct reconstruction is assembled. The board should be simple and readable rather than puzzle-box complex.

### Audio And Atmosphere

Audio is central to this prototype. Crow memories require spatial audio cues; Sparrow memories require ambient changes. Temporary authored audio can be simple, but the system should separate ambient loops, one-shot clues, and memory-specific sound layers.

## UI And Feedback

The UI should be minimal:

- A small interaction prompt near feathers and perches.
- A collected-feather indicator that avoids explaining the hidden species rule.
- A simple nest-board interface for placing feathers.
- A quiet completion message or environmental ending beat.

The prototype should avoid tutorial popups that spell out deduction rules. It can teach controls through brief prompts such as "Hop," "Glide," "Inspect," and "Place."

## Testing And Acceptance Criteria

The prototype is complete when:

- A fresh Unity project opens without missing script errors.
- The player can walk, hop, inspect feathers, glide between perches, and return to the nest.
- The park contains 12 collectible feathers.
- Each of the five bird memory styles works and is visibly or audibly distinct.
- At least eight main feathers can be placed into the nest deduction board.
- The correct deduction triggers the quiet hospital-with-wife ending.
- The game can be played end-to-end in roughly 15 minutes.

## Out Of Scope

The prototype will not include combat, fail states, branching dialogue, cinematic cutscenes, complex NPC schedules, procedural feather placement, save/load support, or a large open-world map.

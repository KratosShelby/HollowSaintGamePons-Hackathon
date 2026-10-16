# Hollow Saint — GameJamPlus 2026/27 Azerbaijan

Theme: **Something's Off**. Submission window: **Oct 18, 10:00-18:00 GMT+4** on platform.gamejamplus.com.

## Open and play

This repo root **is** the Unity project.

1. Install Unity Hub and Editor **6000.0.85f1** (Unity 6 LTS). Free Personal license is fine.
   The project was created from the **2D (Built-In Render Pipeline)** template, which is Unity 6's name for "2D (Core)".
2. Unity Hub → Add → this folder. Open it.
3. Open `Assets/Scenes/Hotel.unity` and press Play. That is the start of the game.
   Any case scene can also be played directly; it adopts its own case so you can test it in isolation.

Controls: **WASD / arrows** move · **E** examine / advance / confirm · **B** (or Tab) The Book · **Esc** back out.
The newspaper lead, Book buttons and tool rows are also clickable.

## The loop

Hotel room → newspaper (bold lead) → case scene → examine clues (each logs a symptom to the case file) →
The Book (pages tick off symptoms you've seen; name the creature) → confront → pick the method → case closes, counter +1 → back to the hotel.
Case 4 turns the same loop on Dutch himself.

## Where things live

- `Assets/Data/` — **all story content**, as ScriptableObjects editable in the Inspector:
  `Cases/` (newspaper text, arrival/naming/resolution lines, clues), `Bestiary/` (Book pages), `Tools/` (Dutch's kit),
  and `Assets/Resources/GameConfig.asset` (case order, radio/mirror lines, ending).
- `Assets/Scenes/` — `Hotel`, `Case_Famished`, `Case_Echo`, `Case_Lodger`.
- `Assets/Scripts/Core` — `Game` (persistent run state: active case, case file, kit, counter, which screen has input).
- `Assets/Scripts/Investigation` — clue / case / Book / tool data and the case file.
- `Assets/Scripts/World` — interactables: clues, pickups, confront points, mirror, newspaper.
- `Assets/Scripts/UI` — runtime-built HUD, dialogue box, Book, tool select, newspaper, end card.
- `Assets/Editor` — `Hollow Saint` menu: **Build Content** (re-seeds the data assets from `ContentBuilder.cs`) and
  **Build Scenes** (rebuilds the four rooms from `SceneBuilder.cs`). Re-running Build Content overwrites Inspector edits to those assets.
- `Assets/Art/` — imported art. Every scene object is a root (collider + behaviour) with a `Visual` child holding the
  SpriteRenderer, so painted art can be swapped in via the Inspector without touching colliders.
- `Design/` — narrative, art direction, Book text. `Design/unity_dev_final.md` is the current brief.
- `Art/` — the original pixel sprites (superseded by the painted-noir direction; not imported).

## Scoring reminder (why scope stays tight)

Game Quality 50% (Mechanics 20%, Art 15%, **Scope Management 15%**), Creativity & Theme 30% (Originality 15%, Theme Integration 15%), Presentation & Pitch 20% (Pitch 10%, Team Dynamics 10%). Scope Management is explicitly graded — finish small and polished over big and broken.

# Hollow Saint — GameJamPlus 2026/27 Azerbaijan

Theme: **Something's Off**. Submission window: **Oct 18, 10:00-18:00 GMT+4** on platform.gamejamplus.com.

## Setup on the build PC (Windows)

1. Install Unity Hub, sign in (free Personal license is fine).
2. In Unity Hub, install an Editor version (2022 LTS recommended).
3. Create a new project: **2D (Core)** template — this counts as a "basic template" under the event rules, not a forbidden complex one.
4. Copy the contents of this folder's `Assets/Scripts/` into your new project's `Assets/Scripts/` folder.
5. Open each script in the Unity Editor once so it compiles; fix any namespace/package errors (TextMeshPro will prompt to import its Essential Resources the first time you use it — accept that prompt).
6. From here it's manual Editor work: create the scenes, drag these scripts onto GameObjects, wire up the Inspector references (sprites, UI text fields, etc.), build the bestiary entries as ScriptableObject assets (right-click in Project window → Create → Hollow Saint → Bestiary Entry).

## Folder guide

- `Design/story.md` — locked narrative, bestiary, scope notes. Read this first.
- `Design/agent_prompts.md` — kickoff prompts for each of the 5 build roles.
- `Assets/Scripts/Core` — GameManager (day/case state tracking)
- `Assets/Scripts/Player` — movement
- `Assets/Scripts/Interaction` — interact-with-objects system
- `Assets/Scripts/Clues` — symptom/clue collection + case file
- `Assets/Scripts/Bestiary` — bestiary data + matching logic
- `Assets/Scripts/UI` — Book UI, tool select, dialogue, newspaper
- `Assets/Scripts/Confrontation` — apply-tool-and-resolve logic
- `Assets/Scripts/Twist` — the final case's special handling

## Scoring reminder (why scope stays tight)

Game Quality 50% (Mechanics 20%, Art 15%, **Scope Management 15%**), Creativity & Theme 30% (Originality 15%, Theme Integration 15%), Presentation & Pitch 20% (Pitch 10%, Team Dynamics 10%). Scope Management is explicitly graded — finish small and polished over big and broken.

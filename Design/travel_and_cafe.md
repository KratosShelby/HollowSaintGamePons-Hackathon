Addendum — walkable street + player-driven car (stretch goal, read the build order at the bottom before starting)

This replaces the earlier cafe/map note — same goal, now with real player control instead of an auto-drive animation. Still a stretch addition: the current build (hotel → instant scene transition → case scene) is a complete, shippable submission on its own. Build this in the order below so it degrades gracefully — if time runs out partway, whatever step you've reached is still fine to ship with.

## The flow

1. **Hotel room** (unchanged) — wake, mirror. The door is now a real exit, not flavor.
2. **The street** (new scene) — walking through the hotel door loads this. Same top-down movement/interact system you've already built, just a bigger outdoor scene — this part is low-risk, it's the same controller on new art. Contains: the hotel's exterior door (walk back in), the cafe's door, and Dutch's car (parked, interactable, like any other object).
3. **Cafe** (new scene) — walking through its door loads it. Dutch gets a drink, reads the newspaper here — same newspaper-hub interaction already built, just relocated. One light source (hanging bulb), 1-2 static background silhouettes for atmosphere. Leaving puts him back on the street.
4. **The car** — interacting with it (same Interactable system as everything else) starts a short **player-controlled driving segment**: simple top-down car movement, accelerate/steer, along a single fixed road to that day's destination. No traffic, no open city to explore — one short linear road per trip, reused/reskinned per case if needed. Reaching the end transitions into the case scene exactly like a normal scene transition.
5. **Case scene** (unchanged).

## Build order — do it in this sequence, ship whatever's done when time runs out

1. **Street scene + working doors** (hotel ↔ street ↔ cafe). Reuses existing movement/interact code entirely — just new scene art and door triggers. Do this first; it's an upgrade over instant transitions on its own even with nothing else below it.
2. **Car interactable that triggers a drive sequence** — start with the simplest possible version: the car "drives" via a short auto-animation along a fixed path (3-5 seconds, no player input) straight into the case scene. This alone gives the feeling of "he drove there."
3. **Only once that's solid:** upgrade the auto-drive into real player-controlled steering on that same fixed short road. This is the polish layer — nice to have, not required for the feature to feel complete.

Don't start any of this until the current rework (painted art direction + Book/newspaper text) is stable and pushed. Push to the repo at each stable step.

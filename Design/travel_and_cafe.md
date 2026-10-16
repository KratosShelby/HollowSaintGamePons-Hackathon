# Addendum — cafe + drivable map (STRETCH GOAL, explicitly optional)

Read this as a stretch addition, not a requirement. The current build (hotel → instant scene transition → case scene) is a complete, shippable submission on its own. Everything below only adds to that if there's time — if it's not done by a sensible cutoff, ship without it. Nothing here should put the working core loop at risk.

## The flow being added

1. **Hotel room** (unchanged) — wake, mirror.
2. **Cafe** (new, small scene) — Dutch gets a drink, sits down, reads the newspaper here instead of in the hotel room. Same newspaper-hub interaction already built, just relocated. Small room, one light source (hanging bulb), 1-2 static background silhouettes for atmosphere — no new systems, this part is cheap.
3. **Map screen** (new) — after picking the real lead, cut to a small fixed city map. Only that day's destination is marked/interactable. A car icon travels from the cafe to the marked location.
4. **Case scene** (unchanged) — arrival transitions in exactly as now.

## How to scope the map/drive so it's actually buildable in the time left

Keep this as simple as possible — the *feeling* of driving there matters more than real vehicle physics:

- **Simplest version (do this first):** the map is a single static image with a road path drawn on it. Clicking/confirming the destination plays a short animation of the car icon sliding along a fixed path (3-5 seconds), then cuts to the scene. This is mechanically just a dressed-up loading screen — no input system, no collision, nothing that can really break.
- **If there's time left after that works:** upgrade to actual player-controlled top-down driving for that short stretch (simple acceleration/steering, no traffic/obstacles needed) — but only as a polish pass on top of the simple version above, never as a replacement that could leave the game unfinished if it runs long.
- **One map, reused for every case:** don't build 4 different maps — one city map, 4 marked destinations (Harbor Street Diner, Lantern Lane, Caravanserai Inn, and back to Room 414 for the finale), only the current day's marker active.

## Why this order

Building the simple slide-animation version first means the feature is "done" (ships fine) very quickly, and the real-driving upgrade becomes a nice-to-have on top rather than a blocker. If the upgrade doesn't get finished, nobody notices — the game still flows hotel → cafe → map transition → scene, which already reads as "he drove there."

Don't start this until the current rework (painted art direction + Book/newspaper text) is in a stable, pushed state.

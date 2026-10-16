# Dutch's notebook — reskin of the existing case-file system

No new logic. This is a visual/content treatment of data that's already tracked (symptoms logged, tools tried, cases resolved). Swap the current case-file display for notebook pages that look hand-scrawled.

## What each entry looks like

- **On logging a symptom:** a short handwritten-style line appears, e.g. *"cold spot by the mirror — 3rd one this week?"* rather than a clean UI label like "Symptom: Cold Spot." Same underlying data (ties to the existing symptom/case-file fields), just phrased in-voice and rendered in a scrawled/journal font.
- **On a wrong tool attempt:** the line it corresponds to gets a single strikethrough, not deleted — visually builds a trail of "things that didn't work," which doubles as proof the deduction wasn't instant/lucky.
- **On case resolved:** the day's newspaper headline (already written — see `newspaper_copy.md`) appears pasted in as a torn clipping, like he kept it. Costs nothing new — it's content that already exists, just given a second visual life.
- **Case counter:** instead of a plain "2/4," render it as tally marks on the inside cover — same counter value, just drawn differently.

## Why this is safe to build

Every one of the above is a presentation layer on top of state that's already tracked and tested (symptom log, tool-fail state, case-resolved counter). No new data model, no new input, no new failure surface — purely art + a bit of UI text formatting. Hand off whenever he has a gap; it's parallel-safe with whatever else is in progress.

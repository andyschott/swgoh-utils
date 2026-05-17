---
name: update-marquee-characters
description: Update the six-character marquee batch in SWGOH by collecting six character names, one shared introduction date, and the first marquee event date, then computing and inserting all derived marquee, shipment, farm, and acceleration dates into src/app/marquee/marquee-dates.ts.
---

# Update Marquee Characters

1. Ask the user for exactly three inputs before editing files:
- Six character names, in marquee order.
- Shared introduction date for all six characters in `YYYY-MM-DD` format.
- First marquee event date in `YYYY-MM-DD` format.

2. Confirm there are exactly six names and both dates are valid ISO dates.

3. Run:
```bash
python3 /Users/aschott/.codex/skills/update-marquee-characters/scripts/update_marquee_dates.py \
  --names "Name 1" "Name 2" "Name 3" "Name 4" "Name 5" "Name 6" \
  --introduction YYYY-MM-DD \
  --first-marquee YYYY-MM-DD \
  --file /Users/aschott/Developer/swgoh-utils/src/app/marquee/marquee-dates.ts
```

4. Date rules implemented by the script:
- `marqueeEvent` for character 1 uses the user-provided first marquee date.
- All six characters use the user-provided introduction date.
- Characters 2-6 use marquee dates at `+14` days from previous character.
- `shipment` for all six is `3 weeks + 1 day` after the final marquee event.
- `farm` for character 1 equals `shipment`; characters 2-6 are `+14` days from previous farm date.
- `acceleration` for all six is exactly one calendar year after introduction.

5. The script inserts six new objects at the top of the `marquees` array, preserving existing data.

6. After running, review the diff and run relevant tests.

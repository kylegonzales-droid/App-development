# Sound and haptic direction

## Principle

**The game must be fully playable muted** — no information exists only in audio
(`.claude/rules/hig-for-games.md`). Everything below is atmosphere and feedback
layered on top of a silent-playable game.

## The soundscape is the setting

Half of "this is Britain" is carried by sound. Specific, unglamorous, accurate:

**Street.** Pelican crossing beeps. Bus door hiss and the bell. A diesel taxi
idling. Wheelie bins. Roadworks. A shutter coming down. Gulls on the river —
Kingston has them, and they read instantly as *English river town*.

**Rail.** A train crossing the viaduct, felt more than heard. Station tannoy,
muffled and unintelligible — never actually a scripted line.

**Night.** Pub noise spilling when a door opens and cutting when it shuts. The
extractor fan behind the chicken shop. A car stereo passing. Distant **two-tone
UK siren** — never an American wail. This is the single most common way a British
setting is ruined, and it is non-negotiable.

**Weather.** Rain on different surfaces (tarmac, canvas awning, car roof, the
river). Wind through the arches. The particular deadness of fog.

**Interiors.** Fridge hum and the fryer at The Late Plate. Compressor and radio
at Verrall's. Strip-light buzz in the car park stairwell.

## Music

**Original compositions only. No sampling of any existing recording, ever.**

Direction: UK electronic — garage, grime, drill and dubstep lineage — arranged
sparse and cold rather than club-loud. Sub-bass, swung hats, space. The reference
is *tension in an empty street at 2 am*, not a party.

**Adaptive in layers, not tracks:**
```
bed        always present, per zone, barely there
pulse      adds with heat / proximity to trouble
lead       story beats only, authored
```
Layers cross-fade on a musical boundary, never abruptly.

**Silence is a tool.** The Charter at 3 am should have almost no music at all.

**Licensing:** every track commissioned or licensed with written, game-scoped,
worldwide, in-perpetuity rights covering App Store distribution and streamed
gameplay. Get this right before a note is commissioned — retro-fitting clearance
is the most common indie legal disaster.

## Voice

Cast **actual South-West London / South London voices**. The single fastest way
to lose authenticity is a generic "London" accent performed by someone who has
never been. Range matters too: Kingston is genuinely multicultural and multi-generational.

Delivery direction: understated, overlapping, unfinished sentences. People talk
over each other and trail off. Nobody declaims.

Scope honestly: full VO for every line is expensive. Recommended — **full VO for
Persistent characters, barks for Reactive archetypes, text-only for everything
else.** Subtitles always, on by default, with speaker labels.

## Haptics

**Additive only. Never load-bearing.** Respects system settings, and a separate
in-game intensity slider (Off / Light / Full).

| Event | Haptic |
|---|---|
| UI selection | Light tick |
| Context action available | Very soft tick on appear |
| Melee connect | Sharp transient, scaled to force |
| Taking damage | Dull thud |
| Vehicle impact | Heavy transient, scaled to speed |
| Tyres losing grip | Low continuous texture |
| Surface change (tarmac → cobbles → gravel) | Subtle continuous texture |
| Mission complete | Two soft ticks — restrained, not a fanfare |
| Heat tier rises | Rising double-tap |

Never: continuous rumble for ambience, haptics during cutscenes, or haptics as
the only signal for anything.

**Framework note:** Core Haptics is **not adopted yet**
(`.claude/rules/game-stack.md`). The table above is the requirement that would
justify adopting it at the appropriate milestone — not permission to add it now.

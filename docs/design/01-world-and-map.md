# World and map plan

Grounded in `docs/research/kingston-geography.md`. **Real geographic
relationships are preserved; every business, building interior, estate and
organisation is fictional.**

## Naming policy

| Layer | Policy | Example |
|---|---|---|
| Natural features | Real names — geography is not ownable | River Thames, Hogsmill |
| Historic public structures | Real or lightly shifted | Clattern Bridge, Kings Bridge |
| Streets | Invented, echoing local cadence | Quay Street, Farriers Row |
| Businesses | **Always fictional** | The Bentham Centre, Verrall's Motors |
| Estates | **Always fictional, fictional location** | Greenside |
| Organisations, people | **Always fictional** | Quayside, Five Star |

No real business, real resident, real estate, or real criminal organisation
appears. No mapping-provider tiles, imagery, labels, styling or data are used.

## Scale

Playable area **≈ 1.2 km × 1.0 km**, compressed to roughly **60–70 % of real
linear distance**. Kingston's real centre is walkable in about twenty minutes;
compressed, crossing the map on foot is **four to five minutes**, by car
**ninety seconds**. Dense enough that every street earns its place; large enough
that knowing a shortcut is worth something.

Built as a **chunk grid** so neighbourhoods can be appended at the edges later
without re-authoring the core. Expansion hooks at the bridge (west), the station
line (south), and the Hogsmill corridor (south-east).

## Zone plan

```
                    N
        ┌───────────────────────────┐
        │   B  THE REACH            │   Thames towpath, Canbury-style
        │   (river, towpath, boats) │   greens, boatyard, moorings
   ═════╪═══ C  KINGS BRIDGE ═══════╪══ → Hampton Side (expansion)
        │   + THE CROSSING          │   arch → car park → ramp
        │  ┌─────────────────────┐  │
        │  │  A   THE CHARTER    │  │   pedestrianised medieval core
   D    │  │  (no vehicles)      │  │   market, mall, church
  THE   │  └─────────────────────┘  │
  LOOP ═╪═══════════════════════════╪═  one-way ring, both bus stations
        │  F  ARRIVALS   G  BACK    │   station + viaduct | yards, garages
        │                OF HOUSE   │
        │   E  GREENSIDE            │   fictional estate
        │   H  HOGSMILL WALK  ~~~~~~╪~~ green corridor, culverts
        └───────────────────────────┘
                    S
```

### A — THE CHARTER *(pedestrianised core)*
Medieval plot widths, 3–8 m frontages, narrow passages between. The **Ancient
Market** with stalls that are cover, obstacle and crowd. **The Bentham Centre**
(covered mall, fictional) pressed against the old market. **All Hallows Church**
and its yard. **Eden Arcade**.

*Mechanically:* **vehicles cannot enter.** Forces the player out of the car and
changes the verb set. Bollards and stall frames are a parkour and chase surface.
Crowd density is the highest in the game by day and near-zero by 1 am, when the
same space is echoing and hostile.

*Politically:* Quayside's power base. They believe they hold the market by right.

### B — THE REACH *(riverside)*
The Thames on the west edge. Towpath running north — a long, car-free traversal
spine past moored narrowboats, a slipway, a rowing club, a boatyard, and a
riverside pub with a terrace. Greens set back from the water.

*Mechanically:* the towpath is a **pedestrian-and-bike corridor with no vehicle
access** — a completely different chase geometry from the Loop. River fog rolls
in at night and at dawn, cutting sightlines both ways.

*Politically:* **Marsh Self-Store & Van Hire**, on a service road behind the
boatyard, is Quayside's actual operational base.

### C — KINGS BRIDGE & THE CROSSING *(the contested route)*
The single most important space in the game. The road bridge west over the
Thames; the railway bridge just upstream.

**The Crossing** is a specific chain:
```
riverside loading yard ─► railway arch ─► car park lower deck ─► ramp ─► the Loop
```
It is the only way to move volume through Kingston at night without a camera or a
queue. Quayside holds the yard lease. Five Star knows the timing. **Toyin died on
that ramp.** Every major story beat returns here.

*Mechanically:* verticality. Car park decks give elevation, sightlines, drops,
and a natural chase arena with a readable floor plan.

### D — THE LOOP *(the one-way system)*
A fast one-way ring encircling the Charter. **Cromwell Gate** and **Fairfield
Gate** bus stations sit on it, physically separated by it. The four-way
convergence at the top of the ring is the busiest junction in the game.

*Mechanically:* **the primary vehicle playground and pursuit arena.** Because it
is one-way, going the wrong way is fast, obvious, and locally authentic as a
transgression — so it is wired directly into the **heat system**: wrong-way
driving raises heat quickly, but cuts corners the police must drive around.
Risk and reward in one geographic fact.

### E — GREENSIDE *(fictional estate)*
**Fictional, in a fictional pocket. Corresponds to no real estate.**

Deck-access blocks over a parade of shops, garage lock-ups behind, drying yards,
a community hall, bin stores, a play area with one working swing. Remi grew up in
Block C.

*Politically:* a lock-up on the garage row is Five Star's dispatch point.

**Portrayal rule, binding on all content:** Greenside is a *home*. The people who
live there are overwhelmingly ordinary — working, studying, raising kids,
running the hall's Tuesday lunch club. The crew operating from one lock-up is a
handful of people, and the estate resents them roughly as much as it resents the
police. Any scene that treats residents as scenery, threat-wallpaper, or a
punchline is wrong and gets cut. Greenside's best NPCs should be its most
sympathetic in the game.

### F — ARRIVALS *(station area)*
**Kingston Cross** station under the viaduct. Taxi rank, bus interchange,
multi-storey station car park, a chicken shop and a kebab shop that only matter
after eleven.

*Mechanically:* the strongest **time-of-day** zone. Rush hour is a wall of
commuters — the best place in the game to lose a tail in a crowd. At 1 am it's
six people and a night bus. The **last train** is a recurring soft deadline.

### G — BACK OF HOUSE *(industrial / service)*
**Ashdown Works.** Railway arches, trade counters, a tyre place, a hand car wash,
a scrapyard, self-store. **Verrall's Motors** — the MOT garage where Remi does
cash-in-hand work — is the player's home base, stash, and respray shop.

*Mechanically:* few witnesses, few cameras. Where you do what you can't do in the
Charter. Vehicle repair, respray to shed heat, storage.

### H — HOGSMILL WALK *(green corridor)*
The Hogsmill joining the Thames under **Clattern Bridge**. A green corridor
south-east: footbridges, **culverts**, playing fields, allotments, a skate park.

*Mechanically:* **culverts and footbridges are pedestrian shortcuts no car can
follow** — the counterweight to the Loop. Unlit at night and genuinely tense;
the only zone where the player is meaningfully alone.

### Expansion — HAMPTON SIDE *(west bank, later)*
One street's worth across Kings Bridge, mostly a boundary. Keeps the bridge
meaningful: crossing it means leaving your manor.

## Time and weather

**1 real minute = 1 game hour.** A full day is 24 minutes. Six day-parts drive
NPC schedules, lighting, faction activity and job availability:

| Day-part | Hours | Character |
|---|---|---|
| Early | 05–08 | Deliveries, street cleaners, empty roads |
| Commute | 08–10 | Peak crowds at Arrivals, gridlocked Loop |
| Trade | 10–16 | Market running, shoppers, shutters up |
| Turnover | 16–19 | Second peak, schools out, shops closing |
| Night | 19–00 | Pubs and bars, the Charter empties, Five Star's shift |
| Dead | 00–05 | Cleaners, night buses, fog, almost nobody |

Weather: **clear, overcast, drizzle, rain, river fog.** Overcast is the default —
this is Britain. Weather changes reflections, NPC density, police sightlines and
vehicle grip. Fog is scripted for key story beats as well as random.

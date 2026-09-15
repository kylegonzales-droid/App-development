# Development roadmap

No dates. Dates invented before the slice exists are fiction. Bands and gates,
each with an exit criterion that is a decision, not a deliverable.

## Phase 1 — Pre-production *(this phase — complete)*
Research, vision, world, factions, protagonist, systems design, technical
direction, visual direction, slice plan.
**Exit:** engine decision confirmed, protagonist confirmed, art resourcing
decided. → `15-open-decisions.md`

## Phase 2 — Scaffold and feel *(VS0)*
Xcode project, four packages, CI, lint, import-boundary enforcement. Movement,
camera, controls, one block at final art quality.
**Exit gate:** the VS0 criteria in `13-vertical-slice.md`. This gate can kill the
project cheaply, which is its purpose.

## Phase 3 — Vertical slice *(VS1–VS5)*
The world, life, driving, heat and one complete mission.
**Exit:** "Last Orders" is playable start to finish at 60 fps, and external
playtesters want to play it again.

## Phase 4 — Production readiness
Second and third zones. Mission authoring tooling so designers write data, not
code. Full save/progression. Accessibility pass against
`.claude/rules/hig-for-games.md`. Performance pass with Instruments and ETTrace.
**Exit:** content can be added by authoring data, with no engineering per mission.

## Phase 5 — Content
All eight zones. 20–30 missions. Full faction rosters and character arcs. The
phone system. Full VO for persistent characters. Music. GameKit achievements and
leaderboards.
**Exit:** the critical path is completable and the world feels inhabited.

## Phase 6 — Polish and ship
Device matrix testing. MetricKit telemetry. Age rating (expect **17+ / PEGI 18**).
App Store assets. Privacy manifest. Localisation decision. Soft launch in a
single market, then wide.

## Post-launch
Hampton Side and further neighbourhoods at the expansion hooks. Seasonal events.
New mission arcs as data.

---

## Standing risks

| Risk | Severity | Mitigation |
|---|---|---|
| **Art volume** | **Critical** | Dominant cost. Secure an artist or narrow scope before Phase 3. Claude cannot produce production art. |
| **Scope** | **Critical** | Open-world crime is among the most expensive genres. The zone structure and slice gates exist to allow shipping something small and excellent. |
| **Feel on a touchscreen** | High | VS0 gate exists precisely to answer this early and cheaply. |
| **Third-person expectation** | High | Resolve the 2D/3D decision now. Discovering it at VS2 is catastrophic. |
| **App Store review** | Medium | Crime themes ship routinely at 17+. Avoid: drug-dealing as a rewarded core loop, real-world hate content, gambling. Restraint on gore and firearms already helps. |
| **Real-world depiction** | Medium | Fictional businesses, fictional estate in a fictional location, no real people or organisations. Already binding in `01-world-and-map.md`. |
| **Music clearance** | Medium | Original or written game-scoped licences only. Settle before commissioning. |
| **Cultural authenticity** | Medium | Cast and consult South-West London voices. Kingston residents should recognise it and not wince. |
| **Performance on old devices** | Low | 2D at 60 fps is tractable; pooling and streaming designed in from the start. |

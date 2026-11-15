# MyVR — Little Lenny delivery checklist

This is the working checklist for the first visible Lenny manifestation. It is intentionally ordered so that a readable Pages experience exists before headset playtesting becomes relevant.

## First: Pages reader

- [x] Add a root Pages entry point with the canonical Workshop moniker.
- [x] Read the existing Experiences `story.json`; do not fork the manuscript or page order.
- [x] Present title, dedication, all 33 numbered pages, and closing pledge.
- [x] Add previous/next navigation, progress, keyboard navigation, and text sizing.
- [ ] Validate the reader in a real browser and check narrow/mobile layouts.
- [ ] Confirm the MyVR GitHub Pages source/configuration and make the preview reachable at the published Pages URL (requires Pages deployment/configuration authorization if not already enabled).

## Second: workshop landing and spatial reader

- [x] Fix the current prototype's reader activation, rendering, and retry paths. (Browser exercise remains open.)
- [ ] Keep the Workshop moniker as the entrance, then the reusable landing scene, then Lenny as a destination.
- [ ] Make desktop navigation complete before treating WebXR controller input as usable.
- [ ] Add source/page identity to the reader projection without creating a second manuscript model.
- [ ] Replace the text-first spatial panel with StoryBuilder composition and author-reviewed visual assets when those are ready.

## Third: artifact and architecture integration

- [ ] Resolve an immutable Experience artifact identity (BundleId + Version + ContentHash); do not treat a mutable development-branch URL as immutable.
- [ ] Verify how Experiences publishes and serves its canonical artifact; avoid inventing a new manifest or persistence system.
- [ ] Consume StoryBuilder composition output and stable source/page IDs through the existing integration path.
- [ ] Keep MyVR responsible for device/session/spatial presentation, not story authoring or Experience identity.

## Fourth: verification and playtest

- [ ] Add automated checks for the Pages reader's 33-page contract and navigation boundaries.
- [ ] Run available repository tests and CI; record actual results.
- [ ] Test desktop browser rendering and network-failure recovery.
- [ ] Test WebXR in a compatible browser.
- [ ] Schedule physical Quest 2 playtest with Trent; record device/browser/controller results separately from simulation.
- [ ] Tune comfort, scale, text readability, controller targeting, and return-to-Workshop behavior from playtest findings.

## Release boundaries

- No merge, release, package publication, or deployment without explicit authorization.
- Do not mark physical headset verification complete until a real Quest playtest has occurred.
- Preserve the author's manuscript and keep provisional artwork clearly labeled.

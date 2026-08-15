---
name: apple-design
description: Use when the user wants a page, app, or component designed or restyled in Apple's own visual language — apple.com and Apple platform UI as reference. Covers restrained near-white/black grounds, product-hero storytelling, SF Pro-style type, frosted-glass chrome, pill buttons, and scroll-driven one-idea-per-viewport pacing. Load before writing markup/CSS for an "Apple-style", "Apple-like", or "make it feel premium like Apple" request. Not for arbitrary minimalism — this is a specific, recognizable system, not just "clean and white."
version: 1.0.0
user-invocable: true
argument-hint: "[target]"
license: Apache 2.0
---

Apple's design language is not "minimalism" as a mood — it is a small set of load-bearing, repeatable decisions. Copying the vibe (white background, big sans-serif type, lots of air) without the mechanics reads as a cheap imitation. This skill is the mechanics.

If the project already uses the `impeccable` skill, treat this as a **pinned brief**: the visual world is Apple's own, decided, not rolled. Skip impeccable's direction/concept-seed step for the world itself — go straight to composition — but its craft-floor checks (contrast, real states, no lazy card grids, etc.) still apply on top of everything below.

## The load-bearing mechanics

**One idea per viewport.** Apple marketing pages are a sequence of single-message sections, not a dense page you scan. Each section makes exactly one claim, demonstrated once, before the next section replaces it entirely. If a section needs a bullet list to explain itself, it is two sections wearing one.

**The product (or the single UI element) is the hero, not the copy.** Headlines are short and declarative — a claim, not a paragraph. Supporting copy is one sentence, set noticeably smaller, directly beneath. The image/render/video is usually larger on screen than the text that introduces it.

**Extreme, intentional whitespace.** Negative space is not "what's left over" — it is sized deliberately to make the one remaining thing feel inevitable. When in doubt, cut content before cutting space. A section with 40% of the viewport empty around a centered subject is normal, not unfinished.

**Near-white / near-black grounds, color reserved for the subject.** Chrome (nav, body copy, backgrounds) stays neutral — `#ffffff`, `#f5f5f7`, `#1d1d1f`, `#000000`. Saturated color shows up on the *product* (a finish color, a UI screenshot, a video), almost never as a decorative background wash, gradient mesh, or brand-color panel behind text. If a section is loud, it's because the product photography is loud — not the layout.

**Typography carries hierarchy, not decoration.** One typeface family (SF Pro character: humanist grotesk, moderate x-height, tight optical tracking at large sizes, generous tracking at small caps/eyebrow sizes). Scale is dramatic — hero type can run 64–120px+ desktop — but weight and size do the work, never color, never gradient text, never multiple competing display faces on one page. Body copy sits at a conservative, highly legible size (17–21px) with generous line-height (1.4–1.6), because the display type already did the shouting.

**Pill buttons, minimal chrome, one link color.** Primary actions are fully-rounded pill buttons, usually near-black fill on light grounds or white fill on dark grounds — never a gradient, never a drop shadow doing the work of affordance. Inline text links use a single restrained blue (`#0066cc` territory), underline-on-hover or no underline at all, never a full button trying to be a link. Secondary actions are often just "Learn more ›" as text, not a second competing button.

**Frosted glass, used sparingly and functionally.** Translucent, blurred chrome (`backdrop-filter: blur(...)` + a semi-transparent fill) belongs on things that float *over* content while scrolling — sticky nav bars, overlays, control surfaces — never as a background texture applied to a whole section for atmosphere. If nothing is scrolling under it, it doesn't need to be glass.

**Photography and render fidelity is non-negotiable.** Product shots read as real studio photography or physically-accurate 3D render: soft, long, believable shadows; specular highlights that respect the material (glass reflects, aluminum has brushed anisotropic highlights, ceramic is matte); seamless studio backgrounds, not floating cutouts with a fake drop shadow. Flat vector "hero illustrations" of a physical product are the tell that this system was skipped, not followed.

**Motion is physical and restrained, not decorative.** Scroll-scrubbed reveals (a product rotates or assembles as the user scrolls past a *pinned* viewport), soft spring/ease-out transitions between states, gentle scale+fade on section entry, a nav bar that condenses and gains its blur only after the user scrolls past the hero. Nothing spins, bounces, or particle-effects for attention; nothing animates just because it can.

**Precision grid, symmetric centering.** Content sits in a constrained, centered column (roughly 980–1200px max width) with generous consistent side gutters; full-bleed only for hero imagery/video between text blocks. Alignment is exact — optical centering over mathematical centering where they'd visibly differ (a headline over a product shot gets nudged, not just `margin: auto`'d).

**Dark sections earn true black, not gray-900.** When a section goes dark (often for a "Pro"/premium product tier, or night/space imagery), the ground is genuinely near-black (`#000` / `#1d1d1f`), with color popping hard against it — this is a deliberate register shift, used once or twice per page, not the whole site's default.

## What this is NOT

- Not "just make it white with lots of padding" — every rule above is specific, not a vibe.
- Not a license to use Apple's actual logo, wordmarks, product names as your own, or to reproduce Apple's copy verbatim — build an *homage to the system*, with the current product's own real name, copy, and content.
- Not appropriate for dense operate/read surfaces (dashboards, admin tools, docs) without adaptation — this is fundamentally a **Persuade**-mode language (product marketing). Applying it verbatim to a data table produces a beautiful page that hides the data. Keep the mechanics (type discipline, restrained color, precise grid) but drop the one-idea-per-viewport pacing and giant hero type for task-dense screens.
- Not permission to skip real content, real states, or accessibility — Apple's own execution ships full states, real contrast, and working keyboard nav under all that restraint.

## Applying it

1. Identify the single claim each section/viewport should make. If you can't state it in one short sentence, the section isn't ready to design.
2. Pick the neutral ground (white/`#f5f5f7` light, or `#000`/`#1d1d1f` dark) for the *whole page* first — don't mix light and dark sections without a deliberate reason tied to content.
3. Size the hero subject (product, screenshot, or the single UI idea) big and centered; write the headline short; cut everything that isn't the claim plus one supporting line.
4. Reach for `reference/tokens.md` for concrete type scale, spacing, and color values when you need to commit to numbers instead of vibes.
5. Build motion last, and only where scroll or state changes actually need it — restraint is the point.

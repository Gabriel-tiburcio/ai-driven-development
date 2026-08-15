# Apple-design reference tokens

Concrete starting values for the mechanics described in SKILL.md. Treat these as a well-calibrated default, not a locked spec — adjust to the actual product's brand color and content, but stay inside the same *system*.

## Color

```
--ground-light:     #ffffff
--ground-light-alt: #f5f5f7   /* the "off-white" Apple uses for secondary sections */
--ground-dark:      #000000
--ground-dark-alt:  #1d1d1f   /* the "near-black" for most dark sections */

--ink-on-light:      #1d1d1f
--ink-on-light-dim:  #6e6e73  /* secondary/supporting copy on light */
--ink-on-dark:        #f5f5f7
--ink-on-dark-dim:    #86868b

--link-blue:   #0066cc
--link-blue-hover: #147ce5

--divider-light: rgba(0, 0, 0, 0.08)
--divider-dark:  rgba(255, 255, 255, 0.12)
```

Product/campaign color (a finish color, an accent tied to one specific product) is chosen per-product — do not invent a generic "brand blue/purple" fallback. If the product has no signature color, stay neutral rather than inventing one.

## Type

System-native stack that approximates SF Pro's character without licensing it:

```
--font-display: -apple-system, "SF Pro Display", "Helvetica Neue", Arial, sans-serif;
--font-text:    -apple-system, "SF Pro Text", "Helvetica Neue", Arial, sans-serif;
```

If self-hosting a face for a non-Apple-platform web build (SF Pro itself is not freely licensable for arbitrary web embedding), reach for a humanist grotesk with a similar moderate x-height and even stroke weight: **Inter**, **Helvetica Now**, or **General Sans** are closer substitutes than a display serif or anything geometric/quirky. Do not default to Helvetica/Arial as the *display* voice if a better-matched licensed alternative is available — those read as "no font was chosen," not as Apple.

Scale (desktop, fluid down on mobile — never fixed-px hero type on small screens):

```
--type-hero:     clamp(2.75rem, 6vw, 7rem);   /* 44px – 112px, one word or a very short phrase */
--type-h1:       clamp(2.25rem, 4vw, 3.5rem);
--type-h2:       clamp(1.75rem, 2.6vw, 2.5rem);
--type-eyebrow:  0.8125rem;   /* 13px, letter-spacing 0.02–0.04em, uppercase or small-caps register, used above a headline sparingly */
--type-body:     1.0625rem;   /* 17px */
--type-body-lg:  1.3125rem;   /* 21px, for a single lead sentence under a big headline */
--type-caption:  0.8125rem;   /* 13px, legal/footnote register */

--tracking-hero: -0.02em;   /* tighten large display type */
--tracking-eyebrow: 0.04em; /* open up small caps/labels */

--leading-display: 1.05;
--leading-body: 1.5;
```

## Spacing / grid

```
--content-max: 980px;   /* Apple's classic text column; up to ~1200px for wider modern layouts */
--gutter: clamp(1.25rem, 4vw, 2.5rem);

--section-pad-y: clamp(4rem, 10vw, 9rem);   /* generous vertical rhythm between one-idea sections */
```

Full-bleed hero media (video/product render) breaks out of `--content-max`; headline and supporting copy stay inside it, usually centered.

## Buttons

```
.btn-pill {
  border-radius: 980px;         /* effectively a full pill at any reasonable height */
  padding: 0.7rem 1.5rem;
  font-size: 0.9375rem;         /* 15px */
  font-weight: 500;
}
.btn-pill--primary-on-light { background: #1d1d1f; color: #fff; }
.btn-pill--primary-on-dark  { background: #fff; color: #1d1d1f; }
.btn-pill:hover { opacity: 0.86; }  /* Apple's own hover is a subtle opacity/tone shift, not a shadow pop */
```

Text links (not buttons): `color: var(--link-blue); text-decoration: none;` with underline or a chevron (`›`) on hover, not a background fill.

## Glass / frosted chrome

```
.glass-nav {
  background: rgba(255, 255, 255, 0.72);   /* rgba(0,0,0,0.72) on dark */
  backdrop-filter: saturate(180%) blur(20px);
  -webkit-backdrop-filter: saturate(180%) blur(20px);
  border-bottom: 1px solid var(--divider-light);
}
```

Only apply to elements that float over moving content (sticky nav, modals, control overlays). A static section background does not get this treatment.

## Motion

```
--ease-apple: cubic-bezier(0.28, 0.11, 0.32, 1);   /* soft, slightly overshoot-free ease-out */
--duration-fast: 200ms;
--duration-base: 400ms;
--duration-slow: 700ms;    /* section-entry reveals */
```

Entrance pattern for a section coming into view: opacity 0→1 plus a small translateY (16–32px) or scale (0.96→1), never both a bounce and a spin. Scroll-pinned product reveals (rotate/assemble while the section stays pinned) are the signature move but are expensive to build well — reserve for the one hero moment that deserves it, not every section.

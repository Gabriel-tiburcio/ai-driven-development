import type { ReactElement, SVGProps } from "react";

export type IconName =
  | "menu"
  | "compass"
  | "receipt"
  | "headset"
  | "info"
  | "restaurant"
  | "kids"
  | "service"
  | "clipboard-list"
  | "search"
  | "chevron-right"
  | "check"
  | "clock"
  | "timer"
  | "arrow-left"
  | "send"
  | "plus"
  | "wifi"
  | "towel"
  | "pillow"
  | "spray"
  | "wrench"
  | "bottle"
  | "qrcode"
  | "leaf"
  | "dumbbell"
  | "music"
  | "sun";

const PATHS: Record<IconName, ReactElement> = {
  menu: (
    <>
      <line x1="3" y1="6" x2="21" y2="6" />
      <line x1="3" y1="12" x2="21" y2="12" />
      <line x1="3" y1="18" x2="21" y2="18" />
    </>
  ),
  compass: (
    <>
      <circle cx="12" cy="12" r="9" />
      <polygon points="15.5,8.5 13,13 8.5,15.5 11,11" />
    </>
  ),
  receipt: (
    <>
      <path d="M6 3h12v18l-3-2-3 2-3-2-3 2Z" />
      <line x1="8.5" y1="8" x2="15.5" y2="8" />
      <line x1="8.5" y1="12" x2="15.5" y2="12" />
    </>
  ),
  headset: (
    <>
      <path d="M4 13v-1a8 8 0 0 1 16 0v1" />
      <rect x="3" y="13" width="4" height="6" rx="1.5" />
      <rect x="17" y="13" width="4" height="6" rx="1.5" />
      <path d="M19 19a4 4 0 0 1-4 3h-2" />
    </>
  ),
  info: (
    <>
      <circle cx="12" cy="12" r="9" />
      <line x1="12" y1="11" x2="12" y2="16.5" />
      <circle cx="12" cy="7.7" r="0.8" fill="currentColor" stroke="none" />
    </>
  ),
  restaurant: (
    <>
      <path d="M7 3v7a2 2 0 0 0 2 2v9" />
      <path d="M7 3v5M10 3v5" />
      <path d="M17 3c-1.4 0-2.5 1.6-2.5 5.5S16 13 17 13v8" />
    </>
  ),
  kids: (
    <>
      <circle cx="12" cy="6" r="2.3" />
      <path d="M6 21c0-4 3-6 6-6s6 2 6 6" />
    </>
  ),
  service: (
    <>
      <path d="M4 20c1-4 4-6 8-6s7 2 8 6" />
      <circle cx="12" cy="8" r="4" />
    </>
  ),
  "clipboard-list": (
    <>
      <rect x="5" y="4" width="14" height="17" rx="2" />
      <path d="M9 3h6v3H9z" />
      <line x1="8.5" y1="11" x2="15.5" y2="11" />
      <line x1="8.5" y1="15" x2="15.5" y2="15" />
    </>
  ),
  search: (
    <>
      <circle cx="10.5" cy="10.5" r="6.5" />
      <line x1="15.2" y1="15.2" x2="20" y2="20" />
    </>
  ),
  "chevron-right": <polyline points="9,5 16,12 9,19" />,
  check: <polyline points="5,12.5 10,17 19,7" />,
  clock: (
    <>
      <circle cx="12" cy="12" r="9" />
      <polyline points="12,7 12,12 16,14" />
    </>
  ),
  timer: (
    <>
      <circle cx="12" cy="13" r="8" />
      <line x1="12" y1="13" x2="12" y2="9" />
      <line x1="9.5" y1="2" x2="14.5" y2="2" />
    </>
  ),
  "arrow-left": (
    <>
      <line x1="19" y1="12" x2="5" y2="12" />
      <polyline points="11,6 5,12 11,18" />
    </>
  ),
  send: <path d="M4 20l17-8L4 4l2 7.5L4 20Z" />,
  plus: (
    <>
      <line x1="12" y1="5" x2="12" y2="19" />
      <line x1="5" y1="12" x2="19" y2="12" />
    </>
  ),
  wifi: (
    <>
      <path d="M3 9a14 14 0 0 1 18 0" />
      <path d="M6.3 12.7a9.5 9.5 0 0 1 11.4 0" />
      <path d="M9.6 16.3a5 5 0 0 1 4.8 0" />
      <circle cx="12" cy="19.5" r="1" fill="currentColor" stroke="none" />
    </>
  ),
  towel: (
    <>
      <path d="M5 4h14v6a7 7 0 0 1-14 0Z" />
      <line x1="5" y1="10" x2="5" y2="20" />
    </>
  ),
  pillow: <path d="M4 8a4 4 0 0 1 4-4h8a4 4 0 0 1 4 4v6a4 4 0 0 1-4 4c-1 0-1.5-1-4-1s-3 1-4 1a4 4 0 0 1-4-4Z" />,
  spray: (
    <>
      <path d="M9 8h6l1 13H8Z" />
      <path d="M11 8V5h2v3" />
      <line x1="15" y1="4" x2="19" y2="2" />
      <line x1="16" y1="7" x2="20" y2="6" />
      <line x1="15" y1="10" x2="19" y2="10" />
    </>
  ),
  wrench: (
    <path d="M14.7 6.3a4 4 0 0 0-5.4 5.4L4 17l3 3 5.3-5.3a4 4 0 0 0 5.4-5.4l-2.8 2.8-2-2Z" />
  ),
  bottle: (
    <>
      <path d="M10 2h4v3l1.5 2v14a1 1 0 0 1-1 1h-5a1 1 0 0 1-1-1V7L10 5Z" />
      <line x1="9" y1="12" x2="15" y2="12" />
    </>
  ),
  qrcode: (
    <>
      <rect x="3" y="3" width="7" height="7" />
      <rect x="14" y="3" width="7" height="7" />
      <rect x="3" y="14" width="7" height="7" />
      <line x1="15" y1="15" x2="15" y2="15.01" />
      <line x1="19" y1="15" x2="19" y2="19" />
      <line x1="15" y1="19" x2="15" y2="19.01" />
      <line x1="19" y1="19" x2="19.01" y2="19" />
    </>
  ),
  leaf: (
    <path d="M20 4C10 4 4 10 4 18v2h2c8 0 14-6 14-16Z M6 20c2-4 6-8 12-13" />
  ),
  dumbbell: (
    <>
      <rect x="2" y="9" width="3" height="6" rx="1" />
      <rect x="19" y="9" width="3" height="6" rx="1" />
      <line x1="6" y1="12" x2="18" y2="12" />
      <rect x="5" y="7" width="2.4" height="10" rx="1" />
      <rect x="16.6" y="7" width="2.4" height="10" rx="1" />
    </>
  ),
  music: (
    <>
      <circle cx="7" cy="18" r="2.4" />
      <circle cx="17" cy="16" r="2.4" />
      <path d="M9.4 18V5.5L19.4 4v11.5" />
    </>
  ),
  sun: (
    <>
      <circle cx="12" cy="12" r="4.2" />
      <line x1="12" y1="2.5" x2="12" y2="5" />
      <line x1="12" y1="19" x2="12" y2="21.5" />
      <line x1="2.5" y1="12" x2="5" y2="12" />
      <line x1="19" y1="12" x2="21.5" y2="12" />
      <line x1="5" y1="5" x2="6.8" y2="6.8" />
      <line x1="17.2" y1="17.2" x2="19" y2="19" />
      <line x1="5" y1="19" x2="6.8" y2="17.2" />
      <line x1="17.2" y1="6.8" x2="19" y2="5" />
    </>
  ),
};

export default function Icon({ name, ...props }: { name: IconName } & SVGProps<SVGSVGElement>) {
  return (
    <svg
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth={1.8}
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
      {...props}
    >
      {PATHS[name]}
    </svg>
  );
}

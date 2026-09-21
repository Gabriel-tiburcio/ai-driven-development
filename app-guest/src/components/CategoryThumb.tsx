import Icon, { type IconName } from "./Icon";

export type ContentKind = "activity" | "event" | "experience" | "restaurant" | "service" | "kids";

interface Visual {
  icon: IconName;
  className: string;
}

const CATEGORY_RULES: { pattern: RegExp; visual: Visual }[] = [
  { pattern: /spa|bem.?estar|massage|relax|wellness/i, visual: { icon: "leaf", className: "cat-spa" } },
  { pattern: /esport|t[êe]nis|fitness|academia|quadra|nata[çc][ãa]o|piscina/i, visual: { icon: "dumbbell", className: "cat-sport" } },
  { pattern: /gastronom|culin[áa]ria|chef|jantar|almo[çc]o|caf[ée]/i, visual: { icon: "restaurant", className: "cat-food" } },
  { pattern: /m[úu]sica|festa|show|entreten/i, visual: { icon: "music", className: "cat-music" } },
  { pattern: /passeio|trilha|mergulho|tour|aventura|externa/i, visual: { icon: "sun", className: "cat-outdoor" } },
  { pattern: /infantil|kids|crian[çc]a/i, visual: { icon: "kids", className: "cat-kids" } },
];

const KIND_DEFAULTS: Record<ContentKind, Visual> = {
  activity: { icon: "compass", className: "cat-default" },
  event: { icon: "music", className: "cat-music" },
  experience: { icon: "sun", className: "cat-outdoor" },
  restaurant: { icon: "restaurant", className: "cat-food" },
  service: { icon: "service", className: "cat-default" },
  kids: { icon: "kids", className: "cat-kids" },
};

function getVisual(category: string | null | undefined, kind: ContentKind): Visual {
  const match = CATEGORY_RULES.find((r) => r.pattern.test(category ?? ""));
  return match?.visual ?? KIND_DEFAULTS[kind];
}

/** Fills its parent's photo container: the real photo when set, otherwise a category-themed placeholder. */
export default function CategoryThumb({
  imageUrl,
  category,
  kind,
}: {
  imageUrl?: string | null;
  category?: string | null;
  kind: ContentKind;
}) {
  if (imageUrl) return <img src={imageUrl} alt="" />;

  const visual = getVisual(category, kind);
  return (
    <div className={`category-fallback ${visual.className}`}>
      <Icon name={visual.icon} />
    </div>
  );
}

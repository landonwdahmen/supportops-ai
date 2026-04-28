import type { ReactNode } from "react";

type BadgeProps = {
  children: ReactNode;
  tone?: "neutral" | "green" | "red" | "amber" | "blue";
};

export function Badge({ children, tone = "neutral" }: BadgeProps) {
  return <span className={`badge ${tone}`}>{children}</span>;
}

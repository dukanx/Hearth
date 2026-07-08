import type { HTMLAttributes } from 'react'

/* Osnovni glass panel — sve „kartice" u aplikaciji polaze odavde. */
export function GlassCard({
  className = '',
  ...rest
}: HTMLAttributes<HTMLDivElement>) {
  return <div className={`glass rounded-glass ${className}`} {...rest} />
}

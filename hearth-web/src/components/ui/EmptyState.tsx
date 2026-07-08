import type { ReactNode } from 'react'
import { GlassCard } from './GlassCard'

interface EmptyStateProps {
  icon: ReactNode
  title: string
  description: string
  action?: ReactNode
}

export function EmptyState({ icon, title, description, action }: EmptyStateProps) {
  return (
    <GlassCard className="px-6 py-12 text-center animate-fade-up">
      <div className="mx-auto flex size-14 items-center justify-center rounded-full bg-ember-50 text-ember-600 [&>svg]:size-7">
        {icon}
      </div>
      <p className="mt-4 font-semibold text-ink">{title}</p>
      <p className="mx-auto mt-1 max-w-xs text-sm text-ink-soft">{description}</p>
      {action && <div className="mt-5 flex justify-center">{action}</div>}
    </GlassCard>
  )
}

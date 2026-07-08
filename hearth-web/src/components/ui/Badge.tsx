import type { ReactNode } from 'react'

type Tone = 'neutral' | 'ember' | 'progress' | 'done' | 'danger'

const TONES: Record<Tone, string> = {
  neutral: 'bg-ink/6 text-ink-soft',
  ember: 'bg-ember-50 text-ember-700',
  progress: 'bg-progress-soft text-progress',
  done: 'bg-done-soft text-done',
  danger: 'bg-danger-soft text-danger',
}

export function Badge({
  tone = 'neutral',
  children,
}: {
  tone?: Tone
  children: ReactNode
}) {
  return (
    <span
      className={`inline-flex items-center gap-1 rounded-full px-2.5 py-0.5 text-xs font-semibold ${TONES[tone]}`}
    >
      {children}
    </span>
  )
}

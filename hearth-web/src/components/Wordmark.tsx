import { Flame } from 'lucide-react'

/* Brend potpis — plamen u ember pilulici + naziv. */
export function Wordmark({ size = 'md' }: { size?: 'md' | 'lg' }) {
  const lg = size === 'lg'
  return (
    <span className="inline-flex items-center gap-2.5">
      <span
        className={`flex items-center justify-center rounded-2xl bg-ember-500 text-white shadow-lg shadow-ember-500/35 ${
          lg ? 'size-12 rounded-3xl' : 'size-8'
        }`}
      >
        <Flame size={lg ? 26 : 18} strokeWidth={2.5} aria-hidden />
      </span>
      <span
        className={`font-bold tracking-tight text-ink ${lg ? 'text-3xl' : 'text-lg'}`}
      >
        Hearth
      </span>
    </span>
  )
}

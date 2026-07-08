import type {
  InputHTMLAttributes,
  ReactNode,
  SelectHTMLAttributes,
  TextareaHTMLAttributes,
} from 'react'

const CONTROL_CLASS =
  'w-full rounded-2xl border border-line bg-white/70 px-4 py-2.5 text-[15px] text-ink placeholder:text-ink-faint outline-none transition focus:border-ember-500 focus:ring-4 focus:ring-ember-500/15'

interface FieldProps {
  label: string
  error?: string
  hint?: string
  icon?: ReactNode
  children: ReactNode
}

/* Label + kontrola + greška/hint; ikonica se apsolutno pozicionira u kontrolu. */
export function Field({ label, error, hint, icon, children }: FieldProps) {
  return (
    <label className="block">
      <span className="mb-1.5 block text-[13px] font-semibold text-ink-soft">
        {label}
      </span>
      <span className="relative block">
        {icon && (
          <span
            aria-hidden
            className="pointer-events-none absolute top-1/2 left-4 -translate-y-1/2 text-ink-faint [&>svg]:size-4.5"
          >
            {icon}
          </span>
        )}
        <span className={icon ? 'block **:data-control:pl-11' : 'block'}>
          {children}
        </span>
      </span>
      {error && <span className="mt-1.5 block text-[13px] text-danger">{error}</span>}
      {!error && hint && (
        <span className="mt-1.5 block text-xs text-ink-faint">{hint}</span>
      )}
    </label>
  )
}

export function TextInput({
  className = '',
  ...rest
}: InputHTMLAttributes<HTMLInputElement>) {
  return <input data-control className={`${CONTROL_CLASS} ${className}`} {...rest} />
}

export function TextArea({
  className = '',
  ...rest
}: TextareaHTMLAttributes<HTMLTextAreaElement>) {
  return (
    <textarea data-control className={`${CONTROL_CLASS} resize-none ${className}`} {...rest} />
  )
}

export function Select({
  className = '',
  ...rest
}: SelectHTMLAttributes<HTMLSelectElement>) {
  return (
    <select data-control className={`${CONTROL_CLASS} appearance-none ${className}`} {...rest} />
  )
}

import { useState, type InputHTMLAttributes } from 'react'
import { Eye, EyeOff } from 'lucide-react'
import { TextInput } from './Field'

/* Lozinka sa "prikaži/sakrij" dugmetom. */
export function PasswordInput({
  className = '',
  ...rest
}: Omit<InputHTMLAttributes<HTMLInputElement>, 'type'>) {
  const [visible, setVisible] = useState(false)

  return (
    <span className="relative block">
      <TextInput
        {...rest}
        type={visible ? 'text' : 'password'}
        className={`pr-11 ${className}`}
      />
      <button
        type="button"
        tabIndex={-1}
        onClick={() => setVisible((v) => !v)}
        aria-label={visible ? 'Sakrij lozinku' : 'Prikaži lozinku'}
        className="absolute top-1/2 right-3.5 -translate-y-1/2 text-ink-faint transition hover:text-ink"
      >
        {visible ? <EyeOff size={18} /> : <Eye size={18} />}
      </button>
    </span>
  )
}

import { Minus, Plus } from 'lucide-react'

interface QuantityStepperProps {
  value: number
  onChange: (value: number) => void
  min?: number
  max?: number
}

export function QuantityStepper({
  value,
  onChange,
  min = 1,
  max = 99,
}: QuantityStepperProps) {
  return (
    <span className="inline-flex items-center gap-1 rounded-full bg-ink/6 p-1">
      <StepButton
        label="Smanji količinu"
        disabled={value <= min}
        onClick={() => onChange(Math.max(min, value - 1))}
      >
        <Minus size={14} />
      </StepButton>
      <span className="min-w-6 text-center text-sm font-bold text-ink tabular-nums">
        {value}
      </span>
      <StepButton
        label="Povećaj količinu"
        disabled={value >= max}
        onClick={() => onChange(Math.min(max, value + 1))}
      >
        <Plus size={14} />
      </StepButton>
    </span>
  )
}

function StepButton({
  label,
  disabled,
  onClick,
  children,
}: {
  label: string
  disabled: boolean
  onClick: () => void
  children: React.ReactNode
}) {
  return (
    <button
      type="button"
      aria-label={label}
      disabled={disabled}
      onClick={onClick}
      className="flex size-7 items-center justify-center rounded-full bg-white text-ink-soft shadow-sm transition hover:text-ink active:scale-90 disabled:opacity-40 disabled:shadow-none"
    >
      {children}
    </button>
  )
}

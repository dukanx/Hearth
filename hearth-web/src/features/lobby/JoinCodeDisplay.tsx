import { useState } from 'react'
import { Check, Copy } from 'lucide-react'

interface JoinCodeDisplayProps {
  label: string
  code: string
}

export function JoinCodeDisplay({ label, code }: JoinCodeDisplayProps) {
  const [copied, setCopied] = useState(false)

  async function handleCopy() {
    await navigator.clipboard.writeText(code)
    setCopied(true)
    window.setTimeout(() => setCopied(false), 2000)
  }

  return (
    <div className="rounded-2xl border border-line bg-white/70 p-4">
      <p className="text-[13px] font-semibold text-ink-soft">{label}</p>
      <div className="mt-2 flex items-center justify-between gap-3">
        <span className="text-2xl font-bold tracking-[0.3em] text-ink tabular-nums">
          {code}
        </span>
        <button
          type="button"
          onClick={handleCopy}
          className={`flex shrink-0 items-center gap-1.5 rounded-full px-3.5 py-1.5 text-xs font-semibold transition active:scale-95 ${
            copied
              ? 'bg-done-soft text-done'
              : 'bg-ink/6 text-ink-soft hover:bg-ink/10 hover:text-ink'
          }`}
        >
          {copied ? <Check size={14} /> : <Copy size={14} />}
          {copied ? 'Kopirano' : 'Kopiraj'}
        </button>
      </div>
    </div>
  )
}

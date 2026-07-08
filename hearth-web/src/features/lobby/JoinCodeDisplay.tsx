import { useState } from 'react'

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
    <div className="rounded-xl border border-hearth-200 bg-hearth-50 p-4">
      <p className="text-sm font-medium text-stone-600">{label}</p>
      <div className="mt-2 flex items-center justify-between gap-3">
        <span className="font-mono text-2xl font-bold tracking-widest text-hearth-800">
          {code}
        </span>
        <button
          type="button"
          onClick={handleCopy}
          className="shrink-0 rounded-lg border border-hearth-300 px-3 py-1.5 text-xs font-medium text-hearth-800 hover:bg-white"
        >
          {copied ? 'Copied!' : 'Copy'}
        </button>
      </div>
    </div>
  )
}

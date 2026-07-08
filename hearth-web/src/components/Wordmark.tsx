/* Brend potpis — logo + naziv. */
export function Wordmark({ size = 'md' }: { size?: 'md' | 'lg' }) {
  const lg = size === 'lg'
  return (
    <span className="inline-flex items-center gap-2">
      <img
        src="/logo.png"
        alt=""
        aria-hidden
        className={lg ? 'size-14' : 'size-9'}
      />
      <span
        className={`font-bold tracking-tight text-ink ${lg ? 'text-3xl' : 'text-lg'}`}
      >
        Hearth
      </span>
    </span>
  )
}

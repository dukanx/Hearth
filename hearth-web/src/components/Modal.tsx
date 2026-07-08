import { useEffect, type ReactNode } from 'react'
import { X } from 'lucide-react'

interface ModalProps {
  title: string
  onClose: () => void
  children: ReactNode
}

/* Na telefonu bottom-sheet (slide-up), na većim ekranima centrirani glass dijalog. */
export function Modal({ title, onClose, children }: ModalProps) {
  useEffect(() => {
    function onKeyDown(event: KeyboardEvent) {
      if (event.key === 'Escape') onClose()
    }
    document.addEventListener('keydown', onKeyDown)
    return () => document.removeEventListener('keydown', onKeyDown)
  }, [onClose])

  return (
    <div className="fixed inset-0 z-50 flex items-end justify-center sm:items-center sm:p-4">
      <button
        type="button"
        aria-label="Zatvori"
        onClick={onClose}
        className="absolute inset-0 bg-ink/25 backdrop-blur-sm animate-fade-in"
      />
      <div
        role="dialog"
        aria-modal="true"
        aria-label={title}
        className="glass relative max-h-[88dvh] w-full overflow-y-auto rounded-t-glass p-6 pb-[max(1.5rem,env(safe-area-inset-bottom))] animate-slide-up sm:max-w-lg sm:rounded-glass sm:pb-6 sm:animate-pop-in"
      >
        <div className="mb-5 flex items-center justify-between gap-4">
          <h2 className="text-lg font-bold tracking-tight text-ink">{title}</h2>
          <button
            type="button"
            onClick={onClose}
            aria-label="Zatvori"
            className="flex size-8 items-center justify-center rounded-full bg-ink/6 text-ink-soft transition hover:bg-ink/10 hover:text-ink active:scale-90"
          >
            <X size={16} />
          </button>
        </div>
        {children}
      </div>
    </div>
  )
}

import {
  createContext,
  useCallback,
  useContext,
  useRef,
  useState,
  type ReactNode,
} from 'react'
import { BellRing } from 'lucide-react'

interface Toast {
  id: number
  message: string
}

interface ToastContextValue {
  showToast: (message: string) => void
}

const ToastContext = createContext<ToastContextValue | null>(null)

const TOAST_DURATION_MS = 4500

export function ToastProvider({ children }: { children: ReactNode }) {
  const [toasts, setToasts] = useState<Toast[]>([])
  const nextId = useRef(0)

  const showToast = useCallback((message: string) => {
    const id = nextId.current++
    setToasts((current) => [...current.slice(-2), { id, message }])
    window.setTimeout(() => {
      setToasts((current) => current.filter((t) => t.id !== id))
    }, TOAST_DURATION_MS)
  }, [])

  return (
    <ToastContext.Provider value={{ showToast }}>
      {children}

      {/* Živa obaveštenja — iznad svega, ne blokiraju dodir */}
      <div
        aria-live="polite"
        className="pointer-events-none fixed inset-x-0 top-4 z-60 flex flex-col items-center gap-2 px-5"
      >
        {toasts.map((toast) => (
          <div
            key={toast.id}
            className="glass flex max-w-sm items-center gap-3 rounded-3xl py-3 pr-5 pl-3.5 animate-pop-in"
          >
            <span className="flex size-8 shrink-0 items-center justify-center rounded-full bg-ember-500 text-white shadow-lg shadow-ember-500/30">
              <BellRing size={15} aria-hidden />
            </span>
            <p className="text-sm font-medium text-ink">{toast.message}</p>
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  )
}

export function useToast() {
  const context = useContext(ToastContext)
  if (!context) {
    throw new Error('useToast must be used within ToastProvider')
  }
  return context
}

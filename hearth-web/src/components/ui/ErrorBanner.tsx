import { CircleAlert } from 'lucide-react'

export function ErrorBanner({ message }: { message: string }) {
  return (
    <div
      role="alert"
      className="flex items-start gap-2.5 rounded-2xl bg-danger-soft px-4 py-3 text-sm text-danger animate-fade-up"
    >
      <CircleAlert size={18} className="mt-0.5 shrink-0" aria-hidden />
      <span>{message}</span>
    </div>
  )
}

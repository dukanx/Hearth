import { Outlet } from 'react-router-dom'
import { Wordmark } from './Wordmark'

export function AuthLayout() {
  return (
    <div className="flex min-h-dvh items-center justify-center px-5 py-12">
      <div className="ambient" aria-hidden />
      <div className="w-full max-w-md">
        <div className="mb-8 flex flex-col items-center gap-3 text-center animate-fade-up">
          <Wordmark size="lg" />
          <p className="text-sm text-ink-soft">
            Zadaci, kupovina i obaveštenja — za celo domaćinstvo.
          </p>
        </div>
        <Outlet />
      </div>
    </div>
  )
}

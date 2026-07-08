import { Link, NavLink, Outlet } from 'react-router-dom'
import { House, ListChecks, LogOut, ShoppingBasket } from 'lucide-react'
import { useAuth } from '../auth/AuthContext'
import { Wordmark } from './Wordmark'

const TABS = [
  { to: '/', label: 'Početna', icon: House, end: true },
  { to: '/tasks', label: 'Zadaci', icon: ListChecks, end: false },
  { to: '/shopping', label: 'Kupovina', icon: ShoppingBasket, end: false },
]

export function AppLayout() {
  const { user, logout } = useAuth()

  return (
    <div className="min-h-dvh">
      <div className="ambient" aria-hidden />

      {/* Top bar — translucentna traka, iOS stil */}
      <header className="glass-bar sticky top-0 z-40 border-b border-white/50">
        <div className="mx-auto flex max-w-xl items-center justify-between px-5 py-3">
          <Link to="/" aria-label="Hearth — početna">
            <Wordmark />
          </Link>

          <div className="flex items-center gap-2">
            <span className="hidden max-w-40 truncate text-sm font-medium text-ink-soft sm:inline">
              {user?.email.split('@')[0]}
            </span>
            <button
              type="button"
              onClick={logout}
              aria-label="Odjavi se"
              title="Odjavi se"
              className="flex size-9 items-center justify-center rounded-full bg-ink/6 text-ink-soft transition hover:bg-ink/10 hover:text-ink active:scale-90"
            >
              <LogOut size={16} />
            </button>
          </div>
        </div>
      </header>

      <main className="mx-auto max-w-xl px-5 pt-6 pb-32">
        <Outlet />
      </main>

      {/* Plutajući tab bar — liquid glass pilula */}
      <nav
        aria-label="Glavna navigacija"
        className="fixed inset-x-0 z-40 flex justify-center"
        style={{ bottom: 'max(1rem, env(safe-area-inset-bottom))' }}
      >
        <div className="glass flex gap-1 rounded-full p-1.5">
          {TABS.map(({ to, label, icon: Icon, end }) => (
            <NavLink
              key={to}
              to={to}
              end={end}
              className={({ isActive }) =>
                `flex w-19 flex-col items-center gap-0.5 rounded-full py-2 transition duration-200 active:scale-95 ${
                  isActive
                    ? 'bg-ember-500 text-white shadow-lg shadow-ember-500/30'
                    : 'text-ink-soft hover:bg-ink/5 hover:text-ink'
                }`
              }
            >
              <Icon size={20} strokeWidth={2.25} aria-hidden />
              <span className="text-[11px] font-semibold">{label}</span>
            </NavLink>
          ))}
        </div>
      </nav>
    </div>
  )
}

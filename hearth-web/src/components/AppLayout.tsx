import { Link, NavLink, Outlet } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

function navClass({ isActive }: { isActive: boolean }) {
  return isActive
    ? 'rounded-lg bg-hearth-700 px-3 py-2 text-sm font-medium text-white'
    : 'rounded-lg px-3 py-2 text-sm font-medium text-stone-600 hover:bg-hearth-100 hover:text-hearth-900'
}

export function AppLayout() {
  const { user, logout } = useAuth()

  return (
    <div className="min-h-screen">
      <header className="border-b border-hearth-200 bg-white/80 backdrop-blur">
        <div className="mx-auto flex max-w-5xl items-center justify-between gap-4 px-4 py-3">
          <Link to="/" className="text-lg font-semibold text-hearth-800">
            Hearth
          </Link>

          <nav className="flex items-center gap-1">
            <NavLink to="/" end className={navClass}>
              Home
            </NavLink>
            <NavLink to="/tasks" className={navClass}>
              Tasks
            </NavLink>
            <NavLink to="/shopping" className={navClass}>
              Shopping
            </NavLink>
          </nav>

          <div className="flex items-center gap-3">
            <span className="hidden text-sm text-stone-500 sm:inline">
              {user?.email}
              {user?.role ? ` · ${user.role}` : ''}
            </span>
            <button
              type="button"
              onClick={logout}
              className="rounded-lg border border-hearth-200 px-3 py-2 text-sm font-medium text-hearth-800 hover:bg-hearth-50"
            >
              Log out
            </button>
          </div>
        </div>
      </header>

      <main className="mx-auto max-w-5xl px-4 py-8">
        <Outlet />
      </main>
    </div>
  )
}

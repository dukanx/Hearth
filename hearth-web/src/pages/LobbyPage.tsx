import { Link } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

export function LobbyPage() {
  const { user, logout } = useAuth()

  return (
    <div className="mx-auto max-w-lg px-4 py-16">
      <div className="rounded-2xl border border-hearth-200 bg-white p-8 shadow-sm">
        <h1 className="text-2xl font-semibold text-stone-900">
          Welcome{user?.email ? `, ${user.email}` : ''}
        </h1>
        <p className="mt-2 text-stone-600">
          You&apos;re signed in but not part of a household yet. Create one or
          join with a code — coming in F2.
        </p>

        <div className="mt-6 flex flex-col gap-3 sm:flex-row">
          <button
            type="button"
            disabled
            className="flex-1 rounded-lg bg-hearth-700 px-4 py-2.5 text-sm font-semibold text-white opacity-50"
          >
            Create household (F2)
          </button>
          <button
            type="button"
            disabled
            className="flex-1 rounded-lg border border-hearth-300 px-4 py-2.5 text-sm font-semibold text-hearth-800 opacity-50"
          >
            Join with code (F2)
          </button>
        </div>

        <p className="mt-6 text-center text-sm text-stone-500">
          <button
            type="button"
            onClick={logout}
            className="font-medium text-hearth-700 hover:underline"
          >
            Sign out
          </button>
          {' · '}
          <Link to="/login" className="font-medium text-hearth-700 hover:underline">
            Back to login
          </Link>
        </p>
      </div>
    </div>
  )
}

import { useAuth } from '../auth/AuthContext'

export function HomePage() {
  const { user } = useAuth()

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-semibold text-stone-900">Dashboard</h1>
      <p className="text-stone-600">
        Signed in as <span className="font-medium">{user?.email}</span>
        {user?.role && (
          <>
            {' '}
            · <span className="font-medium">{user.role}</span>
          </>
        )}
        .
      </p>
      <p className="rounded-xl border border-hearth-200 bg-white p-4 text-sm text-stone-500">
        Tasks, shopping, and notifications arrive in F3–F5.
      </p>
    </div>
  )
}

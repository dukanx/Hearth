import { Link, Outlet } from 'react-router-dom'

export function AuthLayout() {
  return (
    <div className="flex min-h-screen items-center justify-center px-4 py-12">
      <div className="w-full max-w-md">
        <div className="mb-8 text-center">
          <Link to="/" className="text-3xl font-bold text-hearth-800">
            Hearth
          </Link>
          <p className="mt-2 text-sm text-stone-500">
            Household tasks, shopping, and more
          </p>
        </div>
        <Outlet />
      </div>
    </div>
  )
}

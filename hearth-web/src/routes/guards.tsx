import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

export function GuestRoute() {
  const { isAuthenticated, hasHousehold } = useAuth()
  const location = useLocation()

  if (isAuthenticated && hasHousehold) {
    return <Navigate to="/" replace state={{ from: location }} />
  }

  if (isAuthenticated) {
    return <Navigate to="/lobby" replace />
  }

  return <Outlet />
}

export function LobbyRoute() {
  const { isAuthenticated, hasHousehold } = useAuth()
  const location = useLocation()

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location }} />
  }

  if (hasHousehold) {
    return <Navigate to="/" replace />
  }

  return <Outlet />
}

export function AppRoute() {
  const { isAuthenticated, hasHousehold } = useAuth()
  const location = useLocation()

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location }} />
  }

  if (!hasHousehold) {
    return <Navigate to="/lobby" replace />
  }

  return <Outlet />
}

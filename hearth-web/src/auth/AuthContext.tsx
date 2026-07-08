import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import type { AuthUser } from '../types/auth'
import {
  clearStoredToken,
  getStoredToken,
  setStoredToken,
} from './auth-storage'
import { claimsToUser, decodeToken, isTokenExpired } from './jwt'

interface AuthContextValue {
  user: AuthUser | null
  token: string | null
  isAuthenticated: boolean
  hasHousehold: boolean
  setSession: (token: string) => void
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

function readSession(): { token: string | null; user: AuthUser | null } {
  const token = getStoredToken()
  if (!token || isTokenExpired(token)) {
    if (token) clearStoredToken()
    return { token: null, user: null }
  }

  try {
    const claims = decodeToken(token)
    return { token, user: claimsToUser(claims) }
  } catch {
    clearStoredToken()
    return { token: null, user: null }
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSessionState] = useState(readSession)

  const setSession = useCallback((token: string) => {
    setStoredToken(token)
    const claims = decodeToken(token)
    setSessionState({ token, user: claimsToUser(claims) })
  }, [])

  const logout = useCallback(() => {
    clearStoredToken()
    setSessionState({ token: null, user: null })
  }, [])

  const value = useMemo<AuthContextValue>(
    () => ({
      user: session.user,
      token: session.token,
      isAuthenticated: session.user !== null,
      hasHousehold: Boolean(session.user?.householdId),
      setSession,
      logout,
    }),
    [session, setSession, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider')
  }
  return context
}

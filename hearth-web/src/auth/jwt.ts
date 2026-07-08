import { jwtDecode } from 'jwt-decode'
import type { TokenClaims, UserRole } from '../types/auth'

const ROLE_CLAIM =
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

interface RawJwtPayload {
  sub: string
  email: string
  householdId?: string
  exp: number
  [ROLE_CLAIM]?: string
}

export function decodeToken(token: string): TokenClaims {
  const payload = jwtDecode<RawJwtPayload>(token)

  const role = payload[ROLE_CLAIM] as UserRole | undefined

  return {
    sub: payload.sub,
    email: payload.email,
    householdId: payload.householdId,
    role,
    exp: payload.exp,
  }
}

export function isTokenExpired(token: string): boolean {
  try {
    const { exp } = decodeToken(token)
    return Date.now() >= exp * 1000
  } catch {
    return true
  }
}

export function claimsToUser(claims: TokenClaims) {
  return {
    id: claims.sub,
    email: claims.email,
    householdId: claims.householdId,
    role: claims.role,
  }
}

export interface AuthResponse {
  token: string
  expiresAt: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  password: string
  displayName: string
}

export type UserRole = 'Adult' | 'Child'

export interface TokenClaims {
  sub: string
  email: string
  householdId?: string
  role?: UserRole
  exp: number
}

export interface AuthUser {
  id: string
  email: string
  householdId?: string
  role?: UserRole
}

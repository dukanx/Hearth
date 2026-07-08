import type {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
} from '../types/auth'
import { apiFetch } from './client'

export function login(request: LoginRequest) {
  return apiFetch<AuthResponse>('/api/auth/login', {
    method: 'POST',
    body: request,
    auth: false,
  })
}

export function register(request: RegisterRequest) {
  return apiFetch<AuthResponse>('/api/auth/register', {
    method: 'POST',
    body: request,
    auth: false,
  })
}

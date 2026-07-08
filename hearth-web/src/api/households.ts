import type {
  CreateHouseholdRequest,
  CreateHouseholdResponse,
  JoinHouseholdRequest,
} from '../types/household'
import type { AuthResponse } from '../types/auth'
import { apiFetch } from './client'

export function createHousehold(request: CreateHouseholdRequest) {
  return apiFetch<CreateHouseholdResponse>('/api/households', {
    method: 'POST',
    body: request,
  })
}

export function joinHousehold(request: JoinHouseholdRequest) {
  return apiFetch<AuthResponse>('/api/households/join', {
    method: 'POST',
    body: request,
  })
}

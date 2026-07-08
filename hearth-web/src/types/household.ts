import type { AuthResponse } from './auth'

export interface Household {
  id: string
  name: string
  adultJoinCode: string
  childJoinCode: string
}

export interface CreateHouseholdRequest {
  name: string
}

export interface CreateHouseholdResponse {
  token: AuthResponse
  household: Household
}

export interface JoinHouseholdRequest {
  joinCode: string
}

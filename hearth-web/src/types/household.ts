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

export interface HouseholdMember {
  id: string
  displayName: string
  role: 'Adult' | 'Child' | null
}

// Kodovi su null kad je trenutni korisnik dete.
export interface MyHousehold {
  id: string
  name: string
  adultJoinCode: string | null
  childJoinCode: string | null
}

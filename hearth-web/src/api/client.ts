import { clearStoredToken, getStoredToken } from '../auth/auth-storage'

export interface ProblemDetails {
  title?: string
  detail?: string
  status?: number
  errors?: Record<string, string[]>
}

export class ApiError extends Error {
  status: number
  code?: string
  fieldErrors?: Record<string, string[]>

  constructor(
    message: string,
    status: number,
    code?: string,
    fieldErrors?: Record<string, string[]>,
  ) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.code = code
    this.fieldErrors = fieldErrors
  }
}

interface RequestOptions extends Omit<RequestInit, 'body'> {
  body?: unknown
  auth?: boolean
}

export async function apiFetch<T>(
  path: string,
  options: RequestOptions = {},
): Promise<T> {
  const { body, auth = true, headers: customHeaders, ...rest } = options

  const headers = new Headers(customHeaders)

  if (body !== undefined) {
    headers.set('Content-Type', 'application/json')
  }

  let sentToken = false
  if (auth) {
    const token = getStoredToken()
    if (token) {
      headers.set('Authorization', `Bearer ${token}`)
      sentToken = true
    }
  }

  let response: Response
  try {
    response = await fetch(path, {
      ...rest,
      headers,
      body: body !== undefined ? JSON.stringify(body) : undefined,
    })
  } catch {
    throw new ApiError(
      'Nema veze sa serverom. Proveri internet i pokušaj ponovo.',
      0,
      'Network',
    )
  }

  if (response.status === 204) {
    return undefined as T
  }

  const contentType = response.headers.get('content-type') ?? ''
  const isJson = contentType.includes('application/json')
  const data = isJson ? await response.json() : null

  if (!response.ok) {
    // Poslali smo token, a server kaže 401 -> sesija je istekla; nazad na prijavu.
    // (Login sa pogrešnom lozinkom ne ulazi ovde — tada se token i ne šalje.)
    if (response.status === 401 && sentToken) {
      clearStoredToken()
      window.location.assign('/login')
    }

    const problem = data as ProblemDetails | null
    throw new ApiError(
      problem?.detail ?? problem?.title ?? response.statusText,
      response.status,
      problem?.title,
      problem?.errors,
    )
  }

  return data as T
}

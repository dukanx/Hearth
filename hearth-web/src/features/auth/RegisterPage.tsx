import { type FormEvent, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { register } from '../../api/auth'
import { useAuth } from '../../auth/AuthContext'
import { decodeToken } from '../../auth/jwt'
import { ApiError } from '../../api/client'

export function RegisterPage() {
  const navigate = useNavigate()
  const { setSession } = useAuth()
  const [displayName, setDisplayName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [fieldErrors, setFieldErrors] = useState<Record<string, string[]>>({})

  const mutation = useMutation({
    mutationFn: register,
    onSuccess: (data) => {
      setSession(data.token)
      const claims = decodeToken(data.token)
      navigate(claims.householdId ? '/' : '/lobby', { replace: true })
    },
    onError: (err) => {
      if (err instanceof ApiError) {
        setError(err.message)
        setFieldErrors(err.fieldErrors ?? {})
      } else {
        setError('Something went wrong. Please try again.')
      }
    },
  })

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setFieldErrors({})
    mutation.mutate({ email, password, displayName })
  }

  function fieldError(name: string) {
    return fieldErrors[name]?.[0]
  }

  return (
    <div className="rounded-2xl border border-hearth-200 bg-white p-6 shadow-sm">
      <h1 className="text-xl font-semibold text-stone-900">Create account</h1>
      <p className="mt-1 text-sm text-stone-500">
        Join Hearth and set up your household.
      </p>

      <form onSubmit={handleSubmit} className="mt-6 space-y-4">
        {error && (
          <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
            {error}
          </div>
        )}

        <label className="block">
          <span className="text-sm font-medium text-stone-700">Display name</span>
          <input
            type="text"
            required
            autoComplete="name"
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
            className="mt-1 w-full rounded-lg border border-stone-300 px-3 py-2 text-stone-900 outline-none focus:border-hearth-500 focus:ring-2 focus:ring-hearth-200"
          />
          {fieldError('DisplayName') && (
            <p className="mt-1 text-sm text-red-600">{fieldError('DisplayName')}</p>
          )}
        </label>

        <label className="block">
          <span className="text-sm font-medium text-stone-700">Email</span>
          <input
            type="email"
            required
            autoComplete="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="mt-1 w-full rounded-lg border border-stone-300 px-3 py-2 text-stone-900 outline-none focus:border-hearth-500 focus:ring-2 focus:ring-hearth-200"
          />
          {fieldError('Email') && (
            <p className="mt-1 text-sm text-red-600">{fieldError('Email')}</p>
          )}
        </label>

        <label className="block">
          <span className="text-sm font-medium text-stone-700">Password</span>
          <input
            type="password"
            required
            minLength={6}
            autoComplete="new-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="mt-1 w-full rounded-lg border border-stone-300 px-3 py-2 text-stone-900 outline-none focus:border-hearth-500 focus:ring-2 focus:ring-hearth-200"
          />
          {fieldError('Password') && (
            <p className="mt-1 text-sm text-red-600">{fieldError('Password')}</p>
          )}
          <p className="mt-1 text-xs text-stone-400">At least 6 characters</p>
        </label>

        <button
          type="submit"
          disabled={mutation.isPending}
          className="w-full rounded-lg bg-hearth-700 px-4 py-2.5 text-sm font-semibold text-white hover:bg-hearth-800 disabled:opacity-60"
        >
          {mutation.isPending ? 'Creating account…' : 'Create account'}
        </button>
      </form>

      <p className="mt-6 text-center text-sm text-stone-500">
        Already have an account?{' '}
        <Link to="/login" className="font-medium text-hearth-700 hover:underline">
          Sign in
        </Link>
      </p>
    </div>
  )
}

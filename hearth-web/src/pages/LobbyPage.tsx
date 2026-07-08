import { type FormEvent, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { createHousehold, joinHousehold } from '../api/households'
import { ApiError } from '../api/client'
import { useAuth } from '../auth/AuthContext'
import { JoinCodeDisplay } from '../features/lobby/JoinCodeDisplay'
import type { Household } from '../types/household'

type LobbyMode = 'choose' | 'create' | 'join' | 'created'

export function LobbyPage() {
  const navigate = useNavigate()
  const { user, logout, setSession } = useAuth()
  const [mode, setMode] = useState<LobbyMode>('choose')
  const [name, setName] = useState('')
  const [joinCode, setJoinCode] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [createdHousehold, setCreatedHousehold] = useState<Household | null>(null)

  const createMutation = useMutation({
    mutationFn: createHousehold,
    onSuccess: (data) => {
      setSession(data.token.token)
      setCreatedHousehold(data.household)
      setMode('created')
      setError(null)
    },
    onError: (err) => {
      setError(
        err instanceof ApiError
          ? err.message
          : 'Could not create household. Please try again.',
      )
    },
  })

  const joinMutation = useMutation({
    mutationFn: joinHousehold,
    onSuccess: (data) => {
      setSession(data.token)
      navigate('/', { replace: true })
    },
    onError: (err) => {
      setError(
        err instanceof ApiError
          ? err.message
          : 'Could not join household. Please try again.',
      )
    },
  })

  function handleCreateSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    createMutation.mutate({ name: name.trim() })
  }

  function handleJoinSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    joinMutation.mutate({ joinCode: joinCode.trim().toUpperCase() })
  }

  function handleJoinCodeChange(value: string) {
    setJoinCode(value.toUpperCase().replace(/[^A-Z0-9]/g, '').slice(0, 6))
  }

  function resetMode(next: LobbyMode) {
    setError(null)
    setMode(next)
  }

  return (
    <div className="mx-auto max-w-lg px-4 py-16">
      <div className="rounded-2xl border border-hearth-200 bg-white p-8 shadow-sm">
        {mode === 'choose' && (
          <>
            <h1 className="text-2xl font-semibold text-stone-900">
              Welcome{user?.email ? `, ${user.email}` : ''}
            </h1>
            <p className="mt-2 text-stone-600">
              Create a new household or join an existing one with a 6-character
              code.
            </p>

            <div className="mt-6 flex flex-col gap-3 sm:flex-row">
              <button
                type="button"
                onClick={() => resetMode('create')}
                className="flex-1 rounded-lg bg-hearth-700 px-4 py-2.5 text-sm font-semibold text-white hover:bg-hearth-800"
              >
                Create household
              </button>
              <button
                type="button"
                onClick={() => resetMode('join')}
                className="flex-1 rounded-lg border border-hearth-300 px-4 py-2.5 text-sm font-semibold text-hearth-800 hover:bg-hearth-50"
              >
                Join with code
              </button>
            </div>
          </>
        )}

        {mode === 'create' && (
          <>
            <button
              type="button"
              onClick={() => resetMode('choose')}
              className="text-sm font-medium text-hearth-700 hover:underline"
            >
              ← Back
            </button>
            <h1 className="mt-4 text-2xl font-semibold text-stone-900">
              Create household
            </h1>
            <p className="mt-2 text-stone-600">
              You&apos;ll become the adult member and receive join codes for
              others.
            </p>

            <form onSubmit={handleCreateSubmit} className="mt-6 space-y-4">
              {error && <ErrorBanner message={error} />}

              <label className="block">
                <span className="text-sm font-medium text-stone-700">
                  Household name
                </span>
                <input
                  type="text"
                  required
                  maxLength={100}
                  autoFocus
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  placeholder="e.g. Smith Family"
                  className="mt-1 w-full rounded-lg border border-stone-300 px-3 py-2 text-stone-900 outline-none focus:border-hearth-500 focus:ring-2 focus:ring-hearth-200"
                />
              </label>

              <button
                type="submit"
                disabled={createMutation.isPending || !name.trim()}
                className="w-full rounded-lg bg-hearth-700 px-4 py-2.5 text-sm font-semibold text-white hover:bg-hearth-800 disabled:opacity-60"
              >
                {createMutation.isPending ? 'Creating…' : 'Create household'}
              </button>
            </form>
          </>
        )}

        {mode === 'join' && (
          <>
            <button
              type="button"
              onClick={() => resetMode('choose')}
              className="text-sm font-medium text-hearth-700 hover:underline"
            >
              ← Back
            </button>
            <h1 className="mt-4 text-2xl font-semibold text-stone-900">
              Join household
            </h1>
            <p className="mt-2 text-stone-600">
              Enter the 6-character code shared by a household member. Adult and
              child codes assign different roles.
            </p>

            <form onSubmit={handleJoinSubmit} className="mt-6 space-y-4">
              {error && <ErrorBanner message={error} />}

              <label className="block">
                <span className="text-sm font-medium text-stone-700">
                  Join code
                </span>
                <input
                  type="text"
                  required
                  autoFocus
                  value={joinCode}
                  onChange={(e) => handleJoinCodeChange(e.target.value)}
                  placeholder="ABC123"
                  className="mt-1 w-full rounded-lg border border-stone-300 px-3 py-2 font-mono text-lg tracking-widest text-stone-900 uppercase outline-none focus:border-hearth-500 focus:ring-2 focus:ring-hearth-200"
                />
                <p className="mt-1 text-xs text-stone-400">Exactly 6 characters</p>
              </label>

              <button
                type="submit"
                disabled={joinMutation.isPending || joinCode.length !== 6}
                className="w-full rounded-lg bg-hearth-700 px-4 py-2.5 text-sm font-semibold text-white hover:bg-hearth-800 disabled:opacity-60"
              >
                {joinMutation.isPending ? 'Joining…' : 'Join household'}
              </button>
            </form>
          </>
        )}

        {mode === 'created' && createdHousehold && (
          <>
            <h1 className="text-2xl font-semibold text-stone-900">
              {createdHousehold.name} is ready
            </h1>
            <p className="mt-2 text-stone-600">
              Share these codes so family members can join. You&apos;re set up as
              an Adult.
            </p>

            <div className="mt-6 space-y-3">
              <JoinCodeDisplay
                label="Adult join code"
                code={createdHousehold.adultJoinCode}
              />
              <JoinCodeDisplay
                label="Child join code"
                code={createdHousehold.childJoinCode}
              />
            </div>

            <button
              type="button"
              onClick={() => navigate('/', { replace: true })}
              className="mt-6 w-full rounded-lg bg-hearth-700 px-4 py-2.5 text-sm font-semibold text-white hover:bg-hearth-800"
            >
              Enter Hearth
            </button>
          </>
        )}

        {mode !== 'created' && (
          <p className="mt-6 text-center text-sm text-stone-500">
            <button
              type="button"
              onClick={logout}
              className="font-medium text-hearth-700 hover:underline"
            >
              Sign out
            </button>
            {' · '}
            <Link to="/login" className="font-medium text-hearth-700 hover:underline">
              Back to login
            </Link>
          </p>
        )}
      </div>
    </div>
  )
}

function ErrorBanner({ message }: { message: string }) {
  return (
    <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
      {message}
    </div>
  )
}

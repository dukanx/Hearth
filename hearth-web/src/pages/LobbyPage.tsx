import { type FormEvent, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import {
  ArrowLeft,
  ChevronRight,
  HousePlus,
  KeyRound,
  LogOut,
  PartyPopper,
} from 'lucide-react'
import { createHousehold, joinHousehold } from '../api/households'
import { ApiError } from '../api/client'
import { useAuth } from '../auth/AuthContext'
import { JoinCodeDisplay } from '../features/lobby/JoinCodeDisplay'
import { Wordmark } from '../components/Wordmark'
import { GlassCard } from '../components/ui/GlassCard'
import { Button } from '../components/ui/Button'
import { ErrorBanner } from '../components/ui/ErrorBanner'
import { Field, TextInput } from '../components/ui/Field'
import type { Household } from '../types/household'

type LobbyMode = 'choose' | 'create' | 'join' | 'created'

export function LobbyPage() {
  const navigate = useNavigate()
  const { logout, setSession } = useAuth()
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
          : 'Domaćinstvo nije napravljeno. Pokušaj ponovo.',
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
          : 'Pridruživanje nije uspelo. Pokušaj ponovo.',
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
    <div className="flex min-h-dvh items-center justify-center px-5 py-12">
      <div className="ambient" aria-hidden />
      <div className="w-full max-w-md">
        <div className="mb-8 flex justify-center animate-fade-up">
          <Wordmark size="lg" />
        </div>

        <GlassCard className="p-7 animate-fade-up [animation-delay:80ms]">
          {mode === 'choose' && (
            <>
              <h1 className="text-xl font-bold tracking-tight text-ink">
                Tvoje domaćinstvo
              </h1>
              <p className="mt-1 text-sm text-ink-soft">
                Napravi novo ili se pridruži postojećem pomoću koda od 6
                karaktera.
              </p>

              <div className="mt-6 space-y-3">
                <OptionCard
                  icon={<HousePlus />}
                  title="Napravi domaćinstvo"
                  description="Postaješ odrasli član i dobijaš kodove za ostale."
                  onClick={() => resetMode('create')}
                />
                <OptionCard
                  icon={<KeyRound />}
                  title="Pridruži se kodom"
                  description="Unesi kod koji ti je poslao član domaćinstva."
                  onClick={() => resetMode('join')}
                />
              </div>
            </>
          )}

          {mode === 'create' && (
            <>
              <BackButton onClick={() => resetMode('choose')} />
              <h1 className="mt-4 text-xl font-bold tracking-tight text-ink">
                Novo domaćinstvo
              </h1>
              <p className="mt-1 text-sm text-ink-soft">
                Daj mu ime — kodove za pridruživanje dobijaš odmah posle.
              </p>

              <form onSubmit={handleCreateSubmit} className="mt-6 space-y-4">
                {error && <ErrorBanner message={error} />}

                <Field label="Naziv domaćinstva">
                  <TextInput
                    type="text"
                    required
                    maxLength={100}
                    autoFocus
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    placeholder="npr. Dukići"
                  />
                </Field>

                <Button
                  type="submit"
                  loading={createMutation.isPending}
                  disabled={!name.trim()}
                  className="w-full"
                >
                  Napravi domaćinstvo
                </Button>
              </form>
            </>
          )}

          {mode === 'join' && (
            <>
              <BackButton onClick={() => resetMode('choose')} />
              <h1 className="mt-4 text-xl font-bold tracking-tight text-ink">
                Pridruži se
              </h1>
              <p className="mt-1 text-sm text-ink-soft">
                Kod za odrasle i kod za decu dodeljuju različite uloge.
              </p>

              <form onSubmit={handleJoinSubmit} className="mt-6 space-y-4">
                {error && <ErrorBanner message={error} />}

                <Field label="Kod za pridruživanje" hint="Tačno 6 karaktera">
                  <TextInput
                    type="text"
                    required
                    autoFocus
                    value={joinCode}
                    onChange={(e) => handleJoinCodeChange(e.target.value)}
                    placeholder="ABC123"
                    className="text-center text-xl font-bold tracking-[0.35em] uppercase"
                  />
                </Field>

                <Button
                  type="submit"
                  loading={joinMutation.isPending}
                  disabled={joinCode.length !== 6}
                  className="w-full"
                >
                  Pridruži se
                </Button>
              </form>
            </>
          )}

          {mode === 'created' && createdHousehold && (
            <>
              <div className="flex size-12 items-center justify-center rounded-full bg-done-soft text-done">
                <PartyPopper size={24} aria-hidden />
              </div>
              <h1 className="mt-4 text-xl font-bold tracking-tight text-ink">
                {createdHousehold.name} je spremno
              </h1>
              <p className="mt-1 text-sm text-ink-soft">
                Podeli kodove da se ukućani pridruže — ti si već odrasli član.
              </p>

              <div className="mt-6 space-y-3">
                <JoinCodeDisplay
                  label="Kod za odrasle"
                  code={createdHousehold.adultJoinCode}
                />
                <JoinCodeDisplay
                  label="Kod za decu"
                  code={createdHousehold.childJoinCode}
                />
              </div>

              <Button
                onClick={() => navigate('/', { replace: true })}
                className="mt-6 w-full"
              >
                Uđi u Hearth
              </Button>
            </>
          )}

          {mode !== 'created' && (
            <div className="mt-6 flex justify-center border-t border-line pt-5">
              <Button variant="ghost" size="sm" onClick={logout}>
                <LogOut size={14} aria-hidden />
                Odjavi se
              </Button>
            </div>
          )}
        </GlassCard>
      </div>
    </div>
  )
}

function OptionCard({
  icon,
  title,
  description,
  onClick,
}: {
  icon: React.ReactNode
  title: string
  description: string
  onClick: () => void
}) {
  return (
    <button
      type="button"
      onClick={onClick}
      className="group flex w-full items-center gap-4 rounded-2xl border border-line bg-white/70 p-4 text-left transition duration-200 hover:-translate-y-0.5 hover:border-ember-500/40 hover:shadow-lg hover:shadow-ember-500/10 active:scale-[0.98]"
    >
      <span className="flex size-11 shrink-0 items-center justify-center rounded-2xl bg-ember-50 text-ember-600 transition group-hover:bg-ember-500 group-hover:text-white [&>svg]:size-5.5">
        {icon}
      </span>
      <span className="min-w-0 flex-1">
        <span className="block font-semibold text-ink">{title}</span>
        <span className="mt-0.5 block text-[13px] text-ink-soft">{description}</span>
      </span>
      <ChevronRight
        size={18}
        className="shrink-0 text-ink-faint transition group-hover:translate-x-0.5 group-hover:text-ember-600"
        aria-hidden
      />
    </button>
  )
}

function BackButton({ onClick }: { onClick: () => void }) {
  return (
    <button
      type="button"
      onClick={onClick}
      className="inline-flex items-center gap-1.5 text-sm font-semibold text-ink-soft transition hover:text-ink"
    >
      <ArrowLeft size={16} aria-hidden />
      Nazad
    </button>
  )
}

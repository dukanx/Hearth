import { type FormEvent, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { LockKeyhole, Mail } from 'lucide-react'
import { login } from '../../api/auth'
import { useAuth } from '../../auth/AuthContext'
import { decodeToken } from '../../auth/jwt'
import { ApiError } from '../../api/client'
import { GlassCard } from '../../components/ui/GlassCard'
import { Button } from '../../components/ui/Button'
import { ErrorBanner } from '../../components/ui/ErrorBanner'
import { Field, TextInput } from '../../components/ui/Field'

export function LoginPage() {
  const navigate = useNavigate()
  const { setSession } = useAuth()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)

  const mutation = useMutation({
    mutationFn: login,
    onSuccess: (data) => {
      setSession(data.token)
      const claims = decodeToken(data.token)
      navigate(claims.householdId ? '/' : '/lobby', { replace: true })
    },
    onError: (err) => {
      setError(
        err instanceof ApiError
          ? err.message
          : 'Nešto je pošlo naopako. Pokušaj ponovo.',
      )
    },
  })

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    mutation.mutate({ email, password })
  }

  return (
    <GlassCard className="p-7 animate-fade-up [animation-delay:80ms]">
      <h1 className="text-xl font-bold tracking-tight text-ink">Prijava</h1>
      <p className="mt-1 text-sm text-ink-soft">
        Dobro došao/la nazad u svoje domaćinstvo.
      </p>

      <form onSubmit={handleSubmit} className="mt-6 space-y-4">
        {error && <ErrorBanner message={error} />}

        <Field label="Email" icon={<Mail />}>
          <TextInput
            type="email"
            required
            autoComplete="email"
            placeholder="ime@primer.com"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </Field>

        <Field label="Lozinka" icon={<LockKeyhole />}>
          <TextInput
            type="password"
            required
            autoComplete="current-password"
            placeholder="••••••"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </Field>

        <Button type="submit" loading={mutation.isPending} className="w-full">
          Prijavi se
        </Button>
      </form>

      <p className="mt-6 text-center text-sm text-ink-soft">
        Nemaš nalog?{' '}
        <Link
          to="/register"
          className="font-semibold text-ember-600 hover:underline"
        >
          Registruj se
        </Link>
      </p>
    </GlassCard>
  )
}

import { type FormEvent, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { LockKeyhole, Mail, UserRound } from 'lucide-react'
import { register } from '../../api/auth'
import { useAuth } from '../../auth/AuthContext'
import { decodeToken } from '../../auth/jwt'
import { ApiError } from '../../api/client'
import { GlassCard } from '../../components/ui/GlassCard'
import { Button } from '../../components/ui/Button'
import { ErrorBanner } from '../../components/ui/ErrorBanner'
import { Field, TextInput } from '../../components/ui/Field'
import { PasswordInput } from '../../components/ui/PasswordInput'

export function RegisterPage() {
  const navigate = useNavigate()
  const { setSession } = useAuth()
  const [displayName, setDisplayName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
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
        setError('Nešto je pošlo naopako. Pokušaj ponovo.')
      }
    },
  })

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setFieldErrors({})
    if (password !== confirmPassword) {
      setError('Lozinke se ne poklapaju.')
      return
    }
    mutation.mutate({ email, password, displayName })
  }

  const passwordsMismatch =
    confirmPassword.length > 0 && confirmPassword !== password

  function fieldError(name: string) {
    return fieldErrors[name]?.[0]
  }

  return (
    <GlassCard className="p-7 animate-fade-up [animation-delay:80ms]">
      <h1 className="text-xl font-bold tracking-tight text-ink">Novi nalog</h1>
      <p className="mt-1 text-sm text-ink-soft">
        Registruj se, pa napravi domaćinstvo ili se pridruži postojećem.
      </p>

      <form onSubmit={handleSubmit} className="mt-6 space-y-4">
        {error && <ErrorBanner message={error} />}

        <Field label="Ime" icon={<UserRound />} error={fieldError('DisplayName')}>
          <TextInput
            type="text"
            required
            autoComplete="name"
            placeholder="npr. Nikola"
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
          />
        </Field>

        <Field label="Email" icon={<Mail />} error={fieldError('Email')}>
          <TextInput
            type="email"
            required
            autoComplete="email"
            placeholder="ime@primer.com"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </Field>

        <Field
          label="Lozinka"
          icon={<LockKeyhole />}
          error={fieldError('Password')}
          hint="Najmanje 6 karaktera"
        >
          <PasswordInput
            required
            minLength={6}
            autoComplete="new-password"
            placeholder="••••••"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </Field>

        <Field
          label="Potvrdi lozinku"
          icon={<LockKeyhole />}
          error={passwordsMismatch ? 'Lozinke se ne poklapaju.' : undefined}
        >
          <PasswordInput
            required
            autoComplete="new-password"
            placeholder="••••••"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
          />
        </Field>

        <Button
          type="submit"
          loading={mutation.isPending}
          disabled={passwordsMismatch}
          className="w-full"
        >
          Napravi nalog
        </Button>
      </form>

      <p className="mt-6 text-center text-sm text-ink-soft">
        Već imaš nalog?{' '}
        <Link to="/login" className="font-semibold text-ember-600 hover:underline">
          Prijavi se
        </Link>
      </p>
    </GlassCard>
  )
}

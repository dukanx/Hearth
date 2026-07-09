import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import {
  ChevronRight,
  ClipboardCheck,
  ListChecks,
  ShoppingBasket,
  UserRoundPlus,
  UsersRound,
} from 'lucide-react'
import { getTasks } from '../api/tasks'
import { getMyHousehold } from '../api/households'
import { getShoppingItems } from '../api/shopping'
import { useAuth } from '../auth/AuthContext'
import { useMembers } from '../features/household/useMembers'
import { JoinCodeDisplay } from '../features/lobby/JoinCodeDisplay'
import { Modal } from '../components/Modal'
import { GlassCard } from '../components/ui/GlassCard'
import { Badge } from '../components/ui/Badge'

function greeting() {
  const hour = new Date().getHours()
  if (hour < 10) return 'Dobro jutro'
  if (hour < 18) return 'Dobar dan'
  return 'Dobro veče'
}

const TODAY = new Intl.DateTimeFormat('sr-Latn-RS', {
  weekday: 'long',
  day: 'numeric',
  month: 'long',
}).format(new Date())

export function HomePage() {
  const { user } = useAuth()
  const { members, self } = useMembers()
  const [inviteOpen, setInviteOpen] = useState(false)

  const tasksQuery = useQuery({
    queryKey: ['tasks', {}],
    queryFn: () => getTasks(),
  })

  const householdQuery = useQuery({
    queryKey: ['household'],
    queryFn: getMyHousehold,
    staleTime: 5 * 60 * 1000,
  })
  const household = householdQuery.data

  const shoppingQuery = useQuery({
    queryKey: ['shopping', 'Needed'],
    queryFn: () => getShoppingItems('Needed'),
  })

  const tasks = tasksQuery.data ?? []
  const openTasks = tasks.filter((t) => t.status !== 'Done')
  const myOpenTasks = openTasks.filter((t) => t.assignedToUserId === user?.id)
  const neededCount = shoppingQuery.data?.length ?? 0

  return (
    <div className="space-y-6">
      <header className="animate-fade-up">
        <p className="text-sm font-medium text-ink-soft first-letter:uppercase">
          {household ? `${household.name} · ${TODAY}` : TODAY}
        </p>
        <h1 className="mt-1 text-[1.7rem] leading-tight font-bold tracking-tight text-ink">
          {greeting()}
          {self ? `, ${self.displayName}` : ''}
        </h1>
      </header>

      <div className="grid grid-cols-2 gap-3 animate-fade-up [animation-delay:60ms]">
        <StatCard
          to="/tasks"
          icon={<ListChecks />}
          value={tasksQuery.isLoading ? '–' : openTasks.length}
          label="Aktivni zadaci"
        />
        <StatCard
          to="/tasks"
          icon={<ClipboardCheck />}
          value={tasksQuery.isLoading ? '–' : myOpenTasks.length}
          label="Moji zadaci"
        />
      </div>

      <GlassCard className="p-5 animate-fade-up [animation-delay:120ms]">
        <div className="flex items-center justify-between gap-2">
          <div className="flex items-center gap-2 text-ink-soft">
            <UsersRound size={16} aria-hidden />
            <h2 className="text-[13px] font-bold tracking-wide uppercase">
              Ukućani
            </h2>
          </div>
          {household?.adultJoinCode && (
            <button
              type="button"
              onClick={() => setInviteOpen(true)}
              className="inline-flex items-center gap-1.5 rounded-full bg-ember-50 px-3 py-1.5 text-xs font-semibold text-ember-700 transition hover:bg-ember-100 active:scale-95"
            >
              <UserRoundPlus size={13} aria-hidden />
              Pozovi
            </button>
          )}
        </div>
        <ul className="mt-4 space-y-3">
          {members.map((member) => (
            <li key={member.id} className="flex items-center gap-3">
              <span className="flex size-9 shrink-0 items-center justify-center rounded-full bg-ember-50 text-sm font-bold text-ember-700">
                {member.displayName.slice(0, 1).toUpperCase()}
              </span>
              <span className="min-w-0 flex-1 truncate font-medium text-ink">
                {member.displayName}
                {member.id === user?.id && (
                  <span className="text-ink-faint"> (ja)</span>
                )}
              </span>
              <Badge tone={member.role === 'Adult' ? 'ember' : 'neutral'}>
                {member.role === 'Adult' ? 'Odrasli' : 'Dete'}
              </Badge>
            </li>
          ))}
        </ul>
      </GlassCard>

      <Link
        to="/shopping"
        className="group flex items-center gap-4 rounded-glass p-5 glass transition duration-200 hover:-translate-y-0.5 animate-fade-up [animation-delay:180ms]"
      >
        <span className="flex size-11 shrink-0 items-center justify-center rounded-2xl bg-ember-50 text-ember-600">
          <ShoppingBasket size={22} aria-hidden />
        </span>
        <span className="min-w-0 flex-1">
          <span className="block font-semibold text-ink">Lista za kupovinu</span>
          <span className="mt-0.5 block text-[13px] text-ink-soft">
            {neededCount > 0
              ? `${neededCount} ${neededCount === 1 ? 'stavka čeka' : 'stavki čeka'}`
              : 'Šta nedostaje u kući?'}
          </span>
        </span>
        <ChevronRight
          size={18}
          className="text-ink-faint transition group-hover:translate-x-0.5 group-hover:text-ember-600"
          aria-hidden
        />
      </Link>

      {inviteOpen && household?.adultJoinCode && household.childJoinCode && (
        <Modal
          title={`Pozovi u ${household.name}`}
          onClose={() => setInviteOpen(false)}
        >
          <p className="mb-4 text-sm text-ink-soft">
            Novi član se registruje, pa u lobiju unese jedan od ovih kodova —
            kod određuje ulogu.
          </p>
          <div className="space-y-3">
            <JoinCodeDisplay label="Kod za odrasle" code={household.adultJoinCode} />
            <JoinCodeDisplay label="Kod za decu" code={household.childJoinCode} />
          </div>
        </Modal>
      )}
    </div>
  )
}

function StatCard({
  to,
  icon,
  value,
  label,
}: {
  to: string
  icon: React.ReactNode
  value: number | string
  label: string
}) {
  return (
    <Link
      to={to}
      className="glass rounded-glass p-5 transition duration-200 hover:-translate-y-0.5"
    >
      <span className="flex size-9 items-center justify-center rounded-xl bg-ember-50 text-ember-600 [&>svg]:size-4.5">
        {icon}
      </span>
      <span className="mt-3 block text-3xl font-bold tracking-tight text-ink tabular-nums">
        {value}
      </span>
      <span className="mt-0.5 block text-[13px] font-medium text-ink-soft">
        {label}
      </span>
    </Link>
  )
}

import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import {
  CalendarDays,
  Check,
  Pencil,
  Play,
  Trash2,
  Undo2,
  UserRound,
} from 'lucide-react'
import { useAuth } from '../../auth/AuthContext'
import { assignTask, changeTaskStatus, deleteTask } from '../../api/tasks'
import { ApiError } from '../../api/client'
import type { HouseholdTask, TaskStatus } from '../../types/task'
import { Badge } from '../../components/ui/Badge'
import {
  canChangeStatus,
  formatDueDate,
  isAdult,
  isOverdue,
} from './task-utils'

interface TaskCardProps {
  task: HouseholdTask
  memberOptions: { id: string; label: string }[]
  memberName: (userId: string | null | undefined) => string | null
  onEdit: (task: HouseholdTask) => void
}

export function TaskCard({ task, memberOptions, memberName, onEdit }: TaskCardProps) {
  const { user } = useAuth()
  const queryClient = useQueryClient()
  const adult = isAdult(user)
  const [confirmingDelete, setConfirmingDelete] = useState(false)

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ['tasks'] })

  const statusMutation = useMutation({
    mutationFn: (status: TaskStatus) => changeTaskStatus(task.id, status),
    onSuccess: invalidate,
  })

  const assignMutation = useMutation({
    mutationFn: (assignedToUserId: string | null) =>
      assignTask(task.id, assignedToUserId),
    onSuccess: invalidate,
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteTask(task.id),
    onSuccess: invalidate,
  })

  const mutationError = [statusMutation, assignMutation, deleteMutation]
    .map((m) => m.error)
    .find((e) => e instanceof ApiError)?.message

  const done = task.status === 'Done'
  const inProgress = task.status === 'InProgress'
  const mayChangeStatus = Boolean(user && canChangeStatus(user, task))
  const overdue = isOverdue(task)
  const assignee = memberName(task.assignedToUserId)
  const hasFooter = adult || (mayChangeStatus && !done)

  /* Krug: čeka/u toku -> završi; gotov -> vrati u rad (ogledalo state-machine). */
  function toggleComplete() {
    statusMutation.mutate(done ? 'InProgress' : 'Done')
  }

  return (
    <article className="glass rounded-3xl p-4">
      <div className="flex items-start gap-3.5">
        <button
          type="button"
          onClick={toggleComplete}
          disabled={!mayChangeStatus || statusMutation.isPending}
          aria-label={done ? 'Vrati u rad' : 'Označi kao gotov'}
          title={done ? 'Vrati u rad' : 'Označi kao gotov'}
          className={`group mt-0.5 flex size-7 shrink-0 items-center justify-center rounded-full border-2 transition duration-200 active:scale-90 disabled:pointer-events-none disabled:opacity-40 ${
            done
              ? 'border-done bg-done text-white'
              : inProgress
                ? 'border-progress text-transparent hover:bg-done-soft hover:text-done'
                : 'border-ink-faint text-transparent hover:border-done hover:bg-done-soft hover:text-done'
          }`}
        >
          <Check size={14} strokeWidth={3} aria-hidden />
        </button>

        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-start justify-between gap-x-3 gap-y-1">
            <h3
              className={`font-semibold ${
                done ? 'text-ink-faint line-through' : 'text-ink'
              }`}
            >
              {task.title}
            </h3>
            <span className="flex gap-1.5">
              {inProgress && <Badge tone="progress">U toku</Badge>}
              {task.priority === 'High' && !done && (
                <Badge tone="danger">Hitno</Badge>
              )}
              {task.priority === 'Low' && !done && (
                <Badge tone="neutral">Nizak</Badge>
              )}
            </span>
          </div>

          {task.description && (
            <p
              className={`mt-1 line-clamp-2 text-sm ${
                done ? 'text-ink-faint' : 'text-ink-soft'
              }`}
            >
              {task.description}
            </p>
          )}

          <div className="mt-2.5 flex flex-wrap items-center gap-x-4 gap-y-1 text-[13px] text-ink-soft">
            {task.dueDate && (
              <span
                className={`inline-flex items-center gap-1.5 ${
                  overdue ? 'font-semibold text-danger' : ''
                }`}
              >
                <CalendarDays size={14} aria-hidden />
                {formatDueDate(task.dueDate)}
                {overdue && ' · kasni'}
              </span>
            )}
            <span className="inline-flex items-center gap-1.5">
              <UserRound size={14} aria-hidden />
              {assignee ?? 'Nedodeljen'}
            </span>
          </div>
        </div>
      </div>

      {mutationError && (
        <p className="mt-3 rounded-xl bg-danger-soft px-3 py-2 text-[13px] text-danger">
          {mutationError}
        </p>
      )}

      {hasFooter && (
        <div className="mt-3.5 flex flex-wrap items-center gap-2 border-t border-line/70 pt-3.5">
          {mayChangeStatus && !done && (
            <button
              type="button"
              disabled={statusMutation.isPending}
              onClick={() =>
                statusMutation.mutate(inProgress ? 'ToDo' : 'InProgress')
              }
              className={`inline-flex items-center gap-1.5 rounded-full px-3.5 py-1.5 text-xs font-semibold transition active:scale-95 disabled:opacity-50 ${
                inProgress
                  ? 'bg-ink/6 text-ink-soft hover:bg-ink/10'
                  : 'bg-progress-soft text-progress hover:bg-progress/15'
              }`}
            >
              {inProgress ? <Undo2 size={13} /> : <Play size={13} />}
              {inProgress ? 'Vrati na čekanje' : 'Započni'}
            </button>
          )}

          {adult && (
            <span className="ml-auto flex items-center gap-1.5">
              <select
                value={task.assignedToUserId ?? ''}
                disabled={assignMutation.isPending}
                onChange={(e) => assignMutation.mutate(e.target.value || null)}
                aria-label="Dodeli zadatak"
                className="max-w-36 appearance-none truncate rounded-full bg-ink/6 py-1.5 pr-3 pl-3.5 text-xs font-semibold text-ink-soft transition hover:bg-ink/10 disabled:opacity-50"
              >
                <option value="">Nedodeljen</option>
                {memberOptions.map((member) => (
                  <option key={member.id} value={member.id}>
                    {member.label}
                  </option>
                ))}
              </select>

              <IconButton label="Izmeni" onClick={() => onEdit(task)}>
                <Pencil size={14} />
              </IconButton>

              {confirmingDelete ? (
                <span className="flex items-center gap-1.5 animate-fade-in">
                  <span className="text-xs font-semibold text-ink-soft">
                    Obrisati?
                  </span>
                  <button
                    type="button"
                    disabled={deleteMutation.isPending}
                    onClick={() => deleteMutation.mutate()}
                    className="rounded-full bg-danger px-3 py-1.5 text-xs font-bold text-white transition hover:bg-danger/85 active:scale-95 disabled:opacity-50"
                  >
                    Da
                  </button>
                  <button
                    type="button"
                    onClick={() => setConfirmingDelete(false)}
                    className="rounded-full bg-ink/6 px-3 py-1.5 text-xs font-semibold text-ink-soft transition hover:bg-ink/10 active:scale-95"
                  >
                    Ne
                  </button>
                </span>
              ) : (
                <IconButton
                  label="Obriši"
                  danger
                  onClick={() => setConfirmingDelete(true)}
                >
                  <Trash2 size={14} />
                </IconButton>
              )}
            </span>
          )}
        </div>
      )}
    </article>
  )
}

function IconButton({
  label,
  danger = false,
  onClick,
  children,
}: {
  label: string
  danger?: boolean
  onClick: () => void
  children: React.ReactNode
}) {
  return (
    <button
      type="button"
      onClick={onClick}
      aria-label={label}
      title={label}
      className={`flex size-8 items-center justify-center rounded-full transition active:scale-90 ${
        danger
          ? 'bg-ink/6 text-ink-soft hover:bg-danger-soft hover:text-danger'
          : 'bg-ink/6 text-ink-soft hover:bg-ink/10 hover:text-ink'
      }`}
    >
      {children}
    </button>
  )
}

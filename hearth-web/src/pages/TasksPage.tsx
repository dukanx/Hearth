import { useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { ListChecks, Plus, UserRound } from 'lucide-react'
import { Modal } from '../components/Modal'
import { useAuth } from '../auth/AuthContext'
import { createTask, getTasks, updateTask } from '../api/tasks'
import { ApiError } from '../api/client'
import type { HouseholdTask, TaskStatus } from '../types/task'
import { TASK_STATUSES } from '../types/task'
import { TaskCard } from '../features/tasks/TaskCard'
import { TaskForm, type TaskFormValues } from '../features/tasks/TaskForm'
import { STATUS_LABELS, isAdult } from '../features/tasks/task-utils'
import { useMembers } from '../features/household/useMembers'
import { Button } from '../components/ui/Button'
import { EmptyState } from '../components/ui/EmptyState'
import { ErrorBanner } from '../components/ui/ErrorBanner'
import { SegmentedControl } from '../components/ui/SegmentedControl'

type StatusFilter = TaskStatus | 'All'

const FILTER_OPTIONS: { value: StatusFilter; label: string }[] = [
  { value: 'All', label: 'Svi' },
  ...TASK_STATUSES.map((status) => ({
    value: status as StatusFilter,
    label: STATUS_LABELS[status],
  })),
]

export function TasksPage() {
  const { user } = useAuth()
  const queryClient = useQueryClient()
  const { members, memberName } = useMembers()
  const [statusFilter, setStatusFilter] = useState<StatusFilter>('All')
  const [mineOnly, setMineOnly] = useState(false)
  const [modal, setModal] = useState<'create' | 'edit' | null>(null)
  const [editingTask, setEditingTask] = useState<HouseholdTask | null>(null)
  const [formError, setFormError] = useState<string | null>(null)

  const filters = useMemo(
    () => ({
      status: statusFilter === 'All' ? undefined : statusFilter,
      assignedToUserId: mineOnly && user ? user.id : undefined,
    }),
    [statusFilter, mineOnly, user],
  )

  const tasksQuery = useQuery({
    queryKey: ['tasks', filters],
    queryFn: () => getTasks(filters),
    enabled: Boolean(user),
  })

  const memberOptions = useMemo(
    () =>
      members.map((m) => ({
        id: m.id,
        label: m.id === user?.id ? `${m.displayName} (ja)` : m.displayName,
      })),
    [members, user],
  )

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ['tasks'] })

  const createMutation = useMutation({
    mutationFn: createTask,
    onSuccess: () => {
      invalidate()
      closeModal()
    },
    onError: (err) => setFormError(getErrorMessage(err)),
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, values }: { id: string; values: TaskFormValues }) =>
      updateTask(id, toUpdateRequest(values)),
    onSuccess: () => {
      invalidate()
      closeModal()
    },
    onError: (err) => setFormError(getErrorMessage(err)),
  })

  function closeModal() {
    setModal(null)
    setEditingTask(null)
    setFormError(null)
  }

  function openCreate() {
    setFormError(null)
    setEditingTask(null)
    setModal('create')
  }

  function openEdit(task: HouseholdTask) {
    setFormError(null)
    setEditingTask(task)
    setModal('edit')
  }

  function handleCreateSubmit(values: TaskFormValues) {
    setFormError(null)
    createMutation.mutate({
      title: values.title.trim(),
      description: values.description.trim() || null,
      priority: values.priority,
      dueDate: values.dueDate ? toIsoDate(values.dueDate) : null,
      assignedToUserId: values.assignedToUserId || null,
    })
  }

  function handleEditSubmit(values: TaskFormValues) {
    if (!editingTask) return
    setFormError(null)
    updateMutation.mutate({ id: editingTask.id, values })
  }

  const tasks = tasksQuery.data ?? []
  const adult = isAdult(user)

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between gap-4 animate-fade-up">
        <h1 className="text-[1.7rem] font-bold tracking-tight text-ink">
          Zadaci
        </h1>
        <Button onClick={openCreate}>
          <Plus size={16} aria-hidden />
          Novi
        </Button>
      </div>

      <div className="flex flex-wrap items-center gap-2 animate-fade-up [animation-delay:60ms]">
        <SegmentedControl
          ariaLabel="Filter po statusu"
          options={FILTER_OPTIONS}
          value={statusFilter}
          onChange={setStatusFilter}
        />
        <button
          type="button"
          aria-pressed={mineOnly}
          onClick={() => setMineOnly((v) => !v)}
          className={`inline-flex items-center gap-1.5 rounded-full px-3.5 py-1.5 text-[13px] font-semibold transition duration-200 active:scale-95 ${
            mineOnly
              ? 'bg-ember-500 text-white shadow-lg shadow-ember-500/25'
              : 'bg-ink/6 text-ink-soft hover:text-ink'
          }`}
        >
          <UserRound size={14} aria-hidden />
          Moji
        </button>
      </div>

      {tasksQuery.isLoading && (
        <div className="space-y-3" aria-label="Učitavanje">
          {[0, 1, 2].map((i) => (
            <div key={i} className="glass h-24 animate-pulse rounded-3xl" />
          ))}
        </div>
      )}

      {tasksQuery.isError && (
        <ErrorBanner
          message={
            tasksQuery.error instanceof ApiError
              ? tasksQuery.error.message
              : 'Zadaci nisu učitani. Pokušaj ponovo.'
          }
        />
      )}

      {!tasksQuery.isLoading && !tasksQuery.isError && tasks.length === 0 && (
        <EmptyState
          icon={<ListChecks />}
          title="Nema zadataka"
          description={
            statusFilter === 'All' && !mineOnly
              ? 'Dodaj prvi zadatak za svoje domaćinstvo.'
              : 'Nijedan zadatak ne odgovara izabranim filterima.'
          }
          action={
            statusFilter === 'All' && !mineOnly ? (
              <Button onClick={openCreate}>
                <Plus size={16} aria-hidden />
                Novi zadatak
              </Button>
            ) : undefined
          }
        />
      )}

      <div className="space-y-3">
        {tasks.map((task, index) => (
          <div
            key={task.id}
            className="animate-fade-up"
            style={{ animationDelay: `${Math.min(index, 8) * 45}ms` }}
          >
            <TaskCard
              task={task}
              memberOptions={memberOptions}
              memberName={memberName}
              onEdit={openEdit}
            />
          </div>
        ))}
      </div>

      {modal === 'create' && (
        <Modal title="Novi zadatak" onClose={closeModal}>
          {formError && (
            <div className="mb-4">
              <ErrorBanner message={formError} />
            </div>
          )}
          <TaskForm
            memberOptions={memberOptions}
            showAssignee={adult}
            submitLabel="Dodaj zadatak"
            isSubmitting={createMutation.isPending}
            onSubmit={handleCreateSubmit}
            onCancel={closeModal}
          />
        </Modal>
      )}

      {modal === 'edit' && editingTask && (
        <Modal title="Izmena zadatka" onClose={closeModal}>
          {formError && (
            <div className="mb-4">
              <ErrorBanner message={formError} />
            </div>
          )}
          <TaskForm
            initial={editingTask}
            memberOptions={memberOptions}
            submitLabel="Sačuvaj"
            isSubmitting={updateMutation.isPending}
            onSubmit={handleEditSubmit}
            onCancel={closeModal}
          />
        </Modal>
      )}
    </div>
  )
}

function getErrorMessage(err: unknown) {
  return err instanceof ApiError
    ? err.message
    : 'Nešto je pošlo naopako. Pokušaj ponovo.'
}

function toIsoDate(date: string) {
  return new Date(`${date}T00:00:00`).toISOString()
}

function toUpdateRequest(values: TaskFormValues) {
  return {
    title: values.title.trim(),
    description: values.description.trim() || null,
    priority: values.priority,
    dueDate: values.dueDate ? toIsoDate(values.dueDate) : null,
  }
}

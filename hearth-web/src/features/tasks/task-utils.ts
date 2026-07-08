import type { AuthUser } from '../../types/auth'
import type { HouseholdTask, TaskPriority, TaskStatus } from '../../types/task'

export const STATUS_LABELS: Record<TaskStatus, string> = {
  ToDo: 'Čeka',
  InProgress: 'U toku',
  Done: 'Gotov',
}

export const PRIORITY_LABELS: Record<TaskPriority, string> = {
  Low: 'Nizak',
  Medium: 'Srednji',
  High: 'Visok',
}

/* Ogledalo backend state-machine (HouseholdTask.CanTransitionTo). */
const TRANSITIONS: Record<TaskStatus, TaskStatus[]> = {
  ToDo: ['InProgress', 'Done'],
  InProgress: ['ToDo', 'Done'],
  Done: ['InProgress'],
}

export function isAdult(user: AuthUser | null | undefined) {
  return user?.role === 'Adult'
}

export function getAllowedTransitions(status: TaskStatus) {
  return TRANSITIONS[status]
}

export function canChangeStatus(user: AuthUser, task: HouseholdTask) {
  if (isAdult(user)) return true
  return task.assignedToUserId === user.id
}

const DUE_DATE_FORMAT = new Intl.DateTimeFormat('sr-Latn-RS', {
  day: 'numeric',
  month: 'short',
})

export function formatDueDate(value: string | null) {
  if (!value) return null
  return DUE_DATE_FORMAT.format(new Date(value))
}

export function isOverdue(task: HouseholdTask) {
  if (!task.dueDate || task.status === 'Done') return false
  return new Date(task.dueDate).getTime() < Date.now()
}

export function toDateInputValue(value: string | null) {
  if (!value) return ''
  return value.slice(0, 10)
}

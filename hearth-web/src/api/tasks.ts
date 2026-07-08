import type {
  CreateTaskRequest,
  HouseholdTask,
  TaskFilters,
  TaskStatus,
  UpdateTaskRequest,
} from '../types/task'
import { apiFetch } from './client'

function buildQuery(filters: TaskFilters = {}) {
  const params = new URLSearchParams()
  if (filters.status) params.set('status', filters.status)
  if (filters.assignedToUserId) {
    params.set('assignedToUserId', filters.assignedToUserId)
  }
  const query = params.toString()
  return query ? `?${query}` : ''
}

export function getTasks(filters?: TaskFilters) {
  return apiFetch<HouseholdTask[]>(`/api/tasks${buildQuery(filters)}`)
}

export function getTask(id: string) {
  return apiFetch<HouseholdTask>(`/api/tasks/${id}`)
}

export function createTask(request: CreateTaskRequest) {
  return apiFetch<HouseholdTask>('/api/tasks', {
    method: 'POST',
    body: request,
  })
}

export function updateTask(id: string, request: UpdateTaskRequest) {
  return apiFetch<HouseholdTask>(`/api/tasks/${id}`, {
    method: 'PUT',
    body: request,
  })
}

export function changeTaskStatus(id: string, status: TaskStatus) {
  return apiFetch<HouseholdTask>(`/api/tasks/${id}/status`, {
    method: 'PUT',
    body: { status },
  })
}

export function assignTask(id: string, assignedToUserId: string | null) {
  return apiFetch<HouseholdTask>(`/api/tasks/${id}/assign`, {
    method: 'PUT',
    body: { assignedToUserId },
  })
}

export function deleteTask(id: string) {
  return apiFetch<void>(`/api/tasks/${id}`, { method: 'DELETE' })
}

export type TaskStatus = 'ToDo' | 'InProgress' | 'Done'
export type TaskPriority = 'Low' | 'Medium' | 'High'

export interface HouseholdTask {
  id: string
  title: string
  description: string | null
  status: TaskStatus
  priority: TaskPriority
  dueDate: string | null
  assignedToUserId: string | null
  createdByUserId: string
  createdAt: string
}

export interface CreateTaskRequest {
  title: string
  description?: string | null
  priority: TaskPriority
  dueDate?: string | null
  assignedToUserId?: string | null
}

export interface UpdateTaskRequest {
  title: string
  description?: string | null
  priority: TaskPriority
  dueDate?: string | null
}

export interface TaskFilters {
  status?: TaskStatus
  assignedToUserId?: string
}

export const TASK_STATUSES: TaskStatus[] = ['ToDo', 'InProgress', 'Done']
export const TASK_PRIORITIES: TaskPriority[] = ['Low', 'Medium', 'High']

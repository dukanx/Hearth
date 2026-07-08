import type { HearthNotification } from '../types/notification'
import { apiFetch } from './client'

export function getNotifications(unreadOnly = false) {
  const query = unreadOnly ? '?unreadOnly=true' : ''
  return apiFetch<HearthNotification[]>(`/api/notifications${query}`)
}

export function markNotificationRead(id: string) {
  return apiFetch<void>(`/api/notifications/${id}/read`, { method: 'PUT' })
}

export function markAllNotificationsRead() {
  return apiFetch<void>('/api/notifications/read-all', { method: 'PUT' })
}

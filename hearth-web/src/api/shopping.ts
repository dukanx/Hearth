import type {
  CreateShoppingItemRequest,
  ShoppingItem,
  ShoppingItemStatus,
  UpdateShoppingItemRequest,
} from '../types/shopping'
import { apiFetch } from './client'

export function getShoppingItems(status?: ShoppingItemStatus) {
  const query = status ? `?status=${status}` : ''
  return apiFetch<ShoppingItem[]>(`/api/shopping${query}`)
}

export function createShoppingItem(request: CreateShoppingItemRequest) {
  return apiFetch<ShoppingItem>('/api/shopping', {
    method: 'POST',
    body: request,
  })
}

export function updateShoppingItem(id: string, request: UpdateShoppingItemRequest) {
  return apiFetch<ShoppingItem>(`/api/shopping/${id}`, {
    method: 'PUT',
    body: request,
  })
}

export function changeShoppingItemStatus(id: string, status: ShoppingItemStatus) {
  return apiFetch<ShoppingItem>(`/api/shopping/${id}/status`, {
    method: 'PUT',
    body: { status },
  })
}

export function deleteShoppingItem(id: string) {
  return apiFetch<void>(`/api/shopping/${id}`, { method: 'DELETE' })
}

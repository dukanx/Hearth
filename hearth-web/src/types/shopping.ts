export type ShoppingItemStatus = 'Needed' | 'Bought'

export interface ShoppingItem {
  id: string
  name: string
  quantity: number
  status: ShoppingItemStatus
  requestedByUserId: string
  boughtByUserId: string | null
  boughtAt: string | null
  createdAt: string
}

export interface CreateShoppingItemRequest {
  name: string
  quantity: number
}

export interface UpdateShoppingItemRequest {
  name: string
  quantity: number
}

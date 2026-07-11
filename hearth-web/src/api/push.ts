import { apiFetch } from './client'

export function getVapidPublicKey() {
  return apiFetch<{ publicKey: string }>('/api/push/public-key')
}

export function subscribeToPush(subscription: {
  endpoint: string
  p256dh: string
  auth: string
}) {
  return apiFetch<void>('/api/push/subscribe', {
    method: 'POST',
    body: subscription,
  })
}

export interface PushSubscriptionInfo {
  service: string
  createdAt: string
}

// Dijagnostika: koje uređaje backend zna za trenutnog korisnika.
export function getMyPushSubscriptions() {
  return apiFetch<PushSubscriptionInfo[]>('/api/push/subscriptions')
}

export function unsubscribeFromPush(endpoint: string) {
  return apiFetch<void>('/api/push/unsubscribe', {
    method: 'POST',
    body: { endpoint },
  })
}

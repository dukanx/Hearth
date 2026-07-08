import { useCallback, useEffect, useState } from 'react'
import {
  getVapidPublicKey,
  subscribeToPush,
  unsubscribeFromPush,
} from '../../api/push'

type PushStatus = 'unsupported' | 'denied' | 'off' | 'on' | 'busy'

function urlBase64ToUint8Array(base64: string) {
  const padding = '='.repeat((4 - (base64.length % 4)) % 4)
  const normalized = (base64 + padding).replace(/-/g, '+').replace(/_/g, '/')
  const raw = window.atob(normalized)
  return Uint8Array.from(raw, (c) => c.charCodeAt(0))
}

function isSupported() {
  return (
    'serviceWorker' in navigator &&
    'PushManager' in window &&
    'Notification' in window
  )
}

/*
  Sistemske push notifikacije (kad je app zatvorena) — dopuna SignalR-a.
  Napomena: service worker je aktivan u production buildu; u dev režimu
  status ostaje "unsupported" i red se ne prikazuje.
*/
export function usePushSubscription() {
  // Ostaje "unsupported" (sakriveno) dok ne nađemo registrovan service worker.
  const [status, setStatus] = useState<PushStatus>('unsupported')

  useEffect(() => {
    if (!isSupported()) return
    let cancelled = false

    async function detect() {
      const registration = await navigator.serviceWorker.getRegistration()
      if (!registration || cancelled) return
      if (Notification.permission === 'denied') {
        setStatus('denied')
        return
      }
      const subscription = await registration.pushManager.getSubscription()
      if (!cancelled) setStatus(subscription ? 'on' : 'off')
    }

    void detect()
    return () => {
      cancelled = true
    }
  }, [])

  const enable = useCallback(async () => {
    setStatus('busy')
    try {
      const permission = await Notification.requestPermission()
      if (permission !== 'granted') {
        setStatus(permission === 'denied' ? 'denied' : 'off')
        return
      }

      const registration = await navigator.serviceWorker.ready
      const { publicKey } = await getVapidPublicKey()
      const subscription = await registration.pushManager.subscribe({
        userVisibleOnly: true,
        applicationServerKey: urlBase64ToUint8Array(publicKey),
      })

      const json = subscription.toJSON()
      await subscribeToPush({
        endpoint: subscription.endpoint,
        p256dh: json.keys?.p256dh ?? '',
        auth: json.keys?.auth ?? '',
      })
      setStatus('on')
    } catch {
      setStatus('off')
    }
  }, [])

  const disable = useCallback(async () => {
    setStatus('busy')
    try {
      const registration = await navigator.serviceWorker.getRegistration()
      const subscription = await registration?.pushManager.getSubscription()
      if (subscription) {
        await subscription.unsubscribe()
        await unsubscribeFromPush(subscription.endpoint)
      }
      setStatus('off')
    } catch {
      setStatus('on')
    }
  }, [])

  return { status, enable, disable }
}

/*
  Web Push handleri — uvezeni u generisani service worker (workbox.importScripts).
  Ako je aplikacija vidljiva, OS notifikacija se preskače: SignalR toast je već prikazao poruku.
*/
self.addEventListener('push', (event) => {
  if (!event.data) return
  const data = event.data.json()

  event.waitUntil(
    (async () => {
      const clientList = await self.clients.matchAll({
        type: 'window',
        includeUncontrolled: true,
      })
      const appVisible = clientList.some(
        (client) => client.visibilityState === 'visible',
      )
      if (appVisible) return

      await self.registration.showNotification(data.title || 'Hearth', {
        body: data.message,
        icon: '/pwa-192.png',
        badge: '/pwa-192.png',
        lang: 'sr-Latn',
      })
    })(),
  )
})

self.addEventListener('notificationclick', (event) => {
  event.notification.close()
  event.waitUntil(
    (async () => {
      const clientList = await self.clients.matchAll({
        type: 'window',
        includeUncontrolled: true,
      })
      if (clientList.length > 0) {
        await clientList[0].focus()
      } else {
        await self.clients.openWindow('/')
      }
    })(),
  )
})

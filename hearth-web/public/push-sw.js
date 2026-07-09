/*
  Web Push handleri — uvezeni u generisani service worker (workbox.importScripts).
  Notifikacija se prikazuje UVEK: iOS ukida pretplatu ako push prođe bez prikaza
  ("silent push budžet"). Dupliranje sa in-app toastom sprečava backend —
  korisnicima sa aktivnom SignalR konekcijom push se uopšte ne šalje.
*/
self.addEventListener('push', (event) => {
  if (!event.data) return
  const data = event.data.json()

  event.waitUntil(
    self.registration.showNotification(data.title || 'Hearth', {
      body: data.message,
      icon: '/pwa-192.png',
      badge: '/pwa-192.png',
      lang: 'sr-Latn',
    }),
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

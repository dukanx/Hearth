import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Bell, BellOff, BellRing, CheckCheck, Loader2 } from 'lucide-react'
import {
  getNotifications,
  markAllNotificationsRead,
  markNotificationRead,
} from '../../api/notifications'
import { getMyPushSubscriptions } from '../../api/push'
import type { HearthNotification } from '../../types/notification'
import { usePushSubscription } from './usePushSubscription'

const REL_TIME = new Intl.RelativeTimeFormat('sr-Latn-RS', { numeric: 'auto' })

function formatRelative(value: string) {
  const diffMs = new Date(value).getTime() - Date.now()
  const diffMin = Math.round(diffMs / 60_000)
  if (diffMin > -1) return 'upravo sad'
  if (diffMin > -60) return REL_TIME.format(diffMin, 'minute')
  const diffHours = Math.round(diffMin / 60)
  if (diffHours > -24) return REL_TIME.format(diffHours, 'hour')
  return REL_TIME.format(Math.round(diffHours / 24), 'day')
}

// Host push servisa -> ime koje korisnik prepoznaje.
function serviceName(host: string) {
  if (host.includes('push.apple.com')) return 'iPhone/iPad (Apple)'
  if (host.includes('fcm.googleapis.com')) return 'Chrome / Android (Google)'
  if (host.includes('mozilla.com')) return 'Firefox'
  if (host.includes('notify.windows.com')) return 'Edge / Windows'
  return host
}

export function NotificationBell() {
  const queryClient = useQueryClient()
  const [open, setOpen] = useState(false)
  const push = usePushSubscription()

  const query = useQuery({
    queryKey: ['notifications'],
    queryFn: () => getNotifications(),
  })

  // Dijagnostika push-a: koje uređaje backend zna. Učitava se tek kad je
  // panel otvoren i push podržan — inače ne troši zahteve.
  const devicesQuery = useQuery({
    queryKey: ['push', 'subscriptions'],
    queryFn: getMyPushSubscriptions,
    enabled: open && push.status !== 'unsupported',
  })

  const invalidate = () =>
    queryClient.invalidateQueries({ queryKey: ['notifications'] })

  const markReadMutation = useMutation({
    mutationFn: markNotificationRead,
    onSuccess: invalidate,
  })

  const markAllMutation = useMutation({
    mutationFn: markAllNotificationsRead,
    onSuccess: invalidate,
  })

  const notifications = query.data ?? []
  const unreadCount = notifications.filter((n) => !n.isRead).length

  function handleItemClick(notification: HearthNotification) {
    if (!notification.isRead) markReadMutation.mutate(notification.id)
  }

  return (
    <div className="relative">
      <button
        type="button"
        onClick={() => setOpen((v) => !v)}
        aria-label={
          unreadCount > 0
            ? `Obaveštenja — ${unreadCount} nepročitanih`
            : 'Obaveštenja'
        }
        aria-expanded={open}
        className="relative flex size-9 items-center justify-center rounded-full bg-ink/6 text-ink-soft transition hover:bg-ink/10 hover:text-ink active:scale-90"
      >
        <Bell size={16} aria-hidden />
        {unreadCount > 0 && (
          <span className="absolute -top-0.5 -right-0.5 flex min-w-4.5 items-center justify-center rounded-full bg-ember-500 px-1 text-[10px] leading-4.5 font-bold text-white shadow-sm animate-pop-in tabular-nums">
            {unreadCount > 9 ? '9+' : unreadCount}
          </span>
        )}
      </button>

      {open && (
        <>
          <button
            type="button"
            aria-label="Zatvori obaveštenja"
            onClick={() => setOpen(false)}
            className="fixed inset-0 z-40 cursor-default"
          />
          <div className="glass-strong absolute right-0 z-50 mt-2 flex max-h-104 w-80 max-w-[calc(100vw-2.5rem)] flex-col overflow-hidden rounded-3xl animate-pop-in">
            <div className="flex items-center justify-between gap-3 border-b border-line/70 px-4 py-3">
              <h2 className="text-sm font-bold text-ink">Obaveštenja</h2>
              {unreadCount > 0 && (
                <button
                  type="button"
                  disabled={markAllMutation.isPending}
                  onClick={() => markAllMutation.mutate()}
                  className="inline-flex items-center gap-1 text-xs font-semibold text-ember-600 transition hover:text-ember-700 disabled:opacity-50"
                >
                  <CheckCheck size={13} aria-hidden />
                  Pročitaj sve
                </button>
              )}
            </div>

            <div className="overflow-y-auto p-1.5">
              {query.isLoading && (
                <p className="px-3 py-6 text-center text-sm text-ink-soft">
                  Učitavanje…
                </p>
              )}

              {!query.isLoading && notifications.length === 0 && (
                <p className="px-3 py-6 text-center text-sm text-ink-soft">
                  Još nema obaveštenja.
                </p>
              )}

              {notifications.map((notification) => (

                <button
                  key={notification.id}
                  type="button"
                  onClick={() => handleItemClick(notification)}
                  className={`flex w-full items-start gap-2.5 rounded-2xl px-3 py-2.5 text-left transition ${
                    notification.isRead
                      ? 'text-ink-soft hover:bg-ink/4'
                      : 'text-ink hover:bg-ember-50/60'
                  }`}
                >
                  <span
                    aria-hidden
                    className={`mt-1.5 size-2 shrink-0 rounded-full ${
                      notification.isRead ? 'bg-transparent' : 'bg-ember-500'
                    }`}
                  />
                  <span className="min-w-0 flex-1">
                    <span
                      className={`block text-sm ${
                        notification.isRead ? '' : 'font-medium'
                      }`}
                    >
                      {notification.message}
                    </span>
                    <span className="mt-0.5 block text-xs text-ink-faint">
                      {formatRelative(notification.createdAt)}
                    </span>
                  </span>
                </button>
              ))}
            </div>

            {/* Sistemske push notifikacije — vidljivo samo kad postoji service worker (production build) */}
            {push.status !== 'unsupported' && (
              <div className="border-t border-line/70 px-4 py-3">
                <div className="flex items-center justify-between gap-3">
                <span className="text-xs font-semibold text-ink-soft">
                  {push.status === 'denied'
                    ? 'Notifikacije su blokirane u browseru'
                    : 'Obaveštenja i kad je app zatvorena'}
                </span>
                {push.status !== 'denied' && (
                  <button
                    type="button"
                    disabled={push.status === 'busy'}
                    onClick={() =>
                      push.status === 'on' ? push.disable() : push.enable()
                    }
                    className={`inline-flex shrink-0 items-center gap-1.5 rounded-full px-3 py-1.5 text-xs font-semibold transition active:scale-95 disabled:opacity-60 ${
                      push.status === 'on'
                        ? 'bg-ember-50 text-ember-700 hover:bg-ember-100'
                        : 'bg-ink/6 text-ink-soft hover:bg-ink/10 hover:text-ink'
                    }`}
                  >
                    {push.status === 'busy' ? (
                      <Loader2 size={13} className="animate-spin" aria-hidden />
                    ) : push.status === 'on' ? (
                      <BellRing size={13} aria-hidden />
                    ) : (
                      <BellOff size={13} aria-hidden />
                    )}
                    {push.status === 'on' ? 'Uključeno' : 'Uključi'}
                  </button>
                )}
                </div>

                {/* Uređaji koje backend zna — ako je prazno a toggle kaže "Uključeno",
                    pretplata nije stigla do baze. */}
                {devicesQuery.data && (
                  <p className="mt-2 text-[11px] leading-relaxed text-ink-faint">
                    {devicesQuery.data.length === 0
                      ? 'Nijedan uređaj nije prijavljen za push.'
                      : `Prijavljeni uređaji: ${devicesQuery.data
                          .map((d) => serviceName(d.service))
                          .join(', ')}`}
                  </p>
                )}
              </div>
            )}
          </div>
        </>
      )}
    </div>
  )
}

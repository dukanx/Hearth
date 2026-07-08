import { useEffect } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import {
  HubConnectionBuilder,
  LogLevel,
  type HubConnection,
} from '@microsoft/signalr'
import { useAuth } from '../../auth/AuthContext'
import { useToast } from '../../components/ToastProvider'
import type { HearthNotification } from '../../types/notification'

/*
  Živa veza sa /hubs/notifications (JWT ide kroz ?access_token=, backend ga tamo čita).
  Na "ReceiveNotification": toast + osvežavanje lista — ako je stiglo obaveštenje,
  neko je nešto promenio, pa su i zadaci/kupovina potencijalno stari.
*/
export function useNotificationsHub() {
  const { token, isAuthenticated, hasHousehold } = useAuth()
  const queryClient = useQueryClient()
  const { showToast } = useToast()

  useEffect(() => {
    if (!isAuthenticated || !hasHousehold || !token) return

    const connection: HubConnection = new HubConnectionBuilder()
      .withUrl('/hubs/notifications', { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build()

    connection.on('ReceiveNotification', (notification: HearthNotification) => {
      showToast(notification.message)
      queryClient.invalidateQueries({ queryKey: ['notifications'] })
      queryClient.invalidateQueries({ queryKey: ['tasks'] })
      queryClient.invalidateQueries({ queryKey: ['shopping'] })
    })

    let stopped = false
    connection.start().catch((err) => {
      if (!stopped) console.error('SignalR konekcija nije uspela:', err)
    })

    return () => {
      stopped = true
      void connection.stop()
    }
  }, [token, isAuthenticated, hasHousehold, queryClient, showToast])
}

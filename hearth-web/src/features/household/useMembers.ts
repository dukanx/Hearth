import { useQuery } from '@tanstack/react-query'
import { getHouseholdMembers } from '../../api/households'
import { useAuth } from '../../auth/AuthContext'

/* Članovi domaćinstva — deljeno između dashboarda i zadataka (imena umesto ID-jeva). */
export function useMembers() {
  const { user } = useAuth()

  const query = useQuery({
    queryKey: ['members'],
    queryFn: getHouseholdMembers,
    staleTime: 5 * 60 * 1000,
    enabled: Boolean(user?.householdId),
  })

  const members = query.data ?? []
  const self = members.find((m) => m.id === user?.id) ?? null

  function memberName(userId: string | null | undefined) {
    if (!userId) return null
    if (userId === user?.id) return self?.displayName ?? 'Ja'
    return members.find((m) => m.id === userId)?.displayName ?? 'Član'
  }

  return { ...query, members, self, memberName }
}

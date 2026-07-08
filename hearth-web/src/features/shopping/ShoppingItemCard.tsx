import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { Check, Pencil, Trash2, UserRound } from 'lucide-react'
import { changeShoppingItemStatus, deleteShoppingItem } from '../../api/shopping'
import { ApiError } from '../../api/client'
import type { ShoppingItem } from '../../types/shopping'

const BOUGHT_AT_FORMAT = new Intl.DateTimeFormat('sr-Latn-RS', {
  day: 'numeric',
  month: 'short',
})

interface ShoppingItemCardProps {
  item: ShoppingItem
  memberName: (userId: string | null | undefined) => string | null
  onEdit: (item: ShoppingItem) => void
}

export function ShoppingItemCard({ item, memberName, onEdit }: ShoppingItemCardProps) {
  const queryClient = useQueryClient()
  const [confirmingDelete, setConfirmingDelete] = useState(false)

  const invalidate = () =>
    queryClient.invalidateQueries({ queryKey: ['shopping'] })

  const statusMutation = useMutation({
    mutationFn: () =>
      changeShoppingItemStatus(
        item.id,
        item.status === 'Bought' ? 'Needed' : 'Bought',
      ),
    onSuccess: invalidate,
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteShoppingItem(item.id),
    onSuccess: invalidate,
  })

  const mutationError = [statusMutation, deleteMutation]
    .map((m) => m.error)
    .find((e) => e instanceof ApiError)?.message

  const bought = item.status === 'Bought'

  return (
    <article className="glass rounded-3xl p-4">
      <div className="flex items-center gap-3.5">
        <button
          type="button"
          onClick={() => statusMutation.mutate()}
          disabled={statusMutation.isPending}
          aria-label={bought ? 'Vrati na listu' : 'Označi kao kupljeno'}
          title={bought ? 'Vrati na listu' : 'Označi kao kupljeno'}
          className={`flex size-7 shrink-0 items-center justify-center rounded-full border-2 transition duration-200 active:scale-90 disabled:pointer-events-none disabled:opacity-40 ${
            bought
              ? 'border-done bg-done text-white'
              : 'border-ink-faint text-transparent hover:border-done hover:bg-done-soft hover:text-done'
          }`}
        >
          <Check size={14} strokeWidth={3} aria-hidden />
        </button>

        <div className="min-w-0 flex-1">
          <p
            className={`truncate font-semibold ${
              bought ? 'text-ink-faint line-through' : 'text-ink'
            }`}
          >
            {item.name}
            {item.quantity > 1 && (
              <span
                className={`ml-2 text-sm font-bold tabular-nums ${
                  bought ? 'text-ink-faint' : 'text-ember-600'
                }`}
              >
                ×{item.quantity}
              </span>
            )}
          </p>
          <p className="mt-0.5 flex flex-wrap items-center gap-x-3 text-[13px] text-ink-soft">
            {bought ? (
              <span className="inline-flex items-center gap-1.5 text-done">
                <Check size={13} aria-hidden />
                {memberName(item.boughtByUserId) ?? 'Neko'}
                {item.boughtAt &&
                  ` · ${BOUGHT_AT_FORMAT.format(new Date(item.boughtAt))}`}
              </span>
            ) : (
              <span className="inline-flex items-center gap-1.5">
                <UserRound size={13} aria-hidden />
                {memberName(item.requestedByUserId) ?? 'Član'}
              </span>
            )}
          </p>
        </div>

        <span className="flex shrink-0 items-center gap-1.5">
          {!bought && (
            <button
              type="button"
              onClick={() => onEdit(item)}
              aria-label="Izmeni"
              title="Izmeni"
              className="flex size-8 items-center justify-center rounded-full bg-ink/6 text-ink-soft transition hover:bg-ink/10 hover:text-ink active:scale-90"
            >
              <Pencil size={14} />
            </button>
          )}

          {confirmingDelete ? (
            <span className="flex items-center gap-1.5 animate-fade-in">
              <button
                type="button"
                disabled={deleteMutation.isPending}
                onClick={() => deleteMutation.mutate()}
                className="rounded-full bg-danger px-3 py-1.5 text-xs font-bold text-white transition hover:bg-danger/85 active:scale-95 disabled:opacity-50"
              >
                Da
              </button>
              <button
                type="button"
                onClick={() => setConfirmingDelete(false)}
                className="rounded-full bg-ink/6 px-3 py-1.5 text-xs font-semibold text-ink-soft transition hover:bg-ink/10 active:scale-95"
              >
                Ne
              </button>
            </span>
          ) : (
            <button
              type="button"
              onClick={() => setConfirmingDelete(true)}
              aria-label="Obriši"
              title="Obriši"
              className="flex size-8 items-center justify-center rounded-full bg-ink/6 text-ink-soft transition hover:bg-danger-soft hover:text-danger active:scale-90"
            >
              <Trash2 size={14} />
            </button>
          )}
        </span>
      </div>

      {mutationError && (
        <p className="mt-3 rounded-xl bg-danger-soft px-3 py-2 text-[13px] text-danger">
          {mutationError}
        </p>
      )}
    </article>
  )
}

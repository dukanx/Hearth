import { type FormEvent, useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Plus, ShoppingBasket } from 'lucide-react'
import {
  createShoppingItem,
  getShoppingItems,
  updateShoppingItem,
} from '../api/shopping'
import { ApiError } from '../api/client'
import type { ShoppingItem, ShoppingItemStatus } from '../types/shopping'
import { useMembers } from '../features/household/useMembers'
import { ShoppingItemCard } from '../features/shopping/ShoppingItemCard'
import { QuantityStepper } from '../features/shopping/QuantityStepper'
import { Modal } from '../components/Modal'
import { Button } from '../components/ui/Button'
import { EmptyState } from '../components/ui/EmptyState'
import { ErrorBanner } from '../components/ui/ErrorBanner'
import { Field, TextInput } from '../components/ui/Field'
import { SegmentedControl } from '../components/ui/SegmentedControl'

type StatusFilter = ShoppingItemStatus | 'All'

const FILTER_OPTIONS: { value: StatusFilter; label: string }[] = [
  { value: 'All', label: 'Sve' },
  { value: 'Needed', label: 'Potrebno' },
  { value: 'Bought', label: 'Kupljeno' },
]

export function ShoppingPage() {
  const queryClient = useQueryClient()
  const { memberName } = useMembers()
  const [statusFilter, setStatusFilter] = useState<StatusFilter>('All')
  const [newName, setNewName] = useState('')
  const [newQuantity, setNewQuantity] = useState(1)
  const [addError, setAddError] = useState<string | null>(null)
  const [editingItem, setEditingItem] = useState<ShoppingItem | null>(null)

  const itemsQuery = useQuery({
    queryKey: ['shopping', statusFilter],
    queryFn: () =>
      getShoppingItems(statusFilter === 'All' ? undefined : statusFilter),
  })

  const invalidate = () =>
    queryClient.invalidateQueries({ queryKey: ['shopping'] })

  const createMutation = useMutation({
    mutationFn: createShoppingItem,
    onSuccess: () => {
      invalidate()
      setNewName('')
      setNewQuantity(1)
      setAddError(null)
    },
    onError: (err) => setAddError(getErrorMessage(err)),
  })

  function handleAdd(event: FormEvent) {
    event.preventDefault()
    if (!newName.trim()) return
    setAddError(null)
    createMutation.mutate({ name: newName.trim(), quantity: newQuantity })
  }

  /* U prikazu "Sve": prvo šta treba kupiti, pa kupljeno. */
  const items = useMemo(() => {
    const data = itemsQuery.data ?? []
    if (statusFilter !== 'All') return data
    return [...data].sort((a, b) =>
      a.status === b.status ? 0 : a.status === 'Needed' ? -1 : 1,
    )
  }, [itemsQuery.data, statusFilter])

  const neededCount = (itemsQuery.data ?? []).filter(
    (i) => i.status === 'Needed',
  ).length

  return (
    <div className="space-y-5">
      <div className="flex items-baseline justify-between gap-4 animate-fade-up">
        <h1 className="text-[1.7rem] font-bold tracking-tight text-ink">
          Kupovina
        </h1>
        {!itemsQuery.isLoading && statusFilter !== 'Bought' && neededCount > 0 && (
          <span className="text-sm font-semibold text-ink-soft tabular-nums">
            {neededCount} {neededCount === 1 ? 'stavka' : 'stavki'}
          </span>
        )}
      </div>

      {/* Brzo dodavanje — uvek pri ruci, bez modala */}
      <form
        onSubmit={handleAdd}
        className="glass flex flex-wrap items-center gap-2.5 rounded-glass p-3 animate-fade-up [animation-delay:60ms]"
      >
        <TextInput
          type="text"
          maxLength={200}
          placeholder="npr. Mleko"
          aria-label="Nova namirnica"
          value={newName}
          onChange={(e) => setNewName(e.target.value)}
          className="min-w-40 flex-1 border-0 bg-white/80"
        />
        <QuantityStepper value={newQuantity} onChange={setNewQuantity} />
        <Button
          type="submit"
          loading={createMutation.isPending}
          disabled={!newName.trim()}
          aria-label="Dodaj na listu"
          className="px-4"
        >
          <Plus size={18} aria-hidden />
        </Button>
        {addError && (
          <p className="w-full rounded-xl bg-danger-soft px-3 py-2 text-[13px] text-danger">
            {addError}
          </p>
        )}
      </form>

      <div className="animate-fade-up [animation-delay:100ms]">
        <SegmentedControl
          ariaLabel="Filter po statusu"
          options={FILTER_OPTIONS}
          value={statusFilter}
          onChange={setStatusFilter}
        />
      </div>

      {itemsQuery.isLoading && (
        <div className="space-y-3" aria-label="Učitavanje">
          {[0, 1, 2].map((i) => (
            <div key={i} className="glass h-18 animate-pulse rounded-3xl" />
          ))}
        </div>
      )}

      {itemsQuery.isError && (
        <ErrorBanner
          message={
            itemsQuery.error instanceof ApiError
              ? itemsQuery.error.message
              : 'Lista nije učitana. Pokušaj ponovo.'
          }
        />
      )}

      {!itemsQuery.isLoading && !itemsQuery.isError && items.length === 0 && (
        <EmptyState
          icon={<ShoppingBasket />}
          title={statusFilter === 'Bought' ? 'Još ništa nije kupljeno' : 'Lista je prazna'}
          description={
            statusFilter === 'Bought'
              ? 'Kupljene stavke će se pojaviti ovde.'
              : 'Dodaj prvu namirnicu — svi ukućani vide istu listu.'
          }
        />
      )}

      <div className="space-y-3">
        {items.map((item, index) => (
          <div
            key={item.id}
            className="animate-fade-up"
            style={{ animationDelay: `${Math.min(index, 8) * 45}ms` }}
          >
            <ShoppingItemCard
              item={item}
              memberName={memberName}
              onEdit={setEditingItem}
            />
          </div>
        ))}
      </div>

      {editingItem && (
        <EditItemModal
          item={editingItem}
          onClose={() => setEditingItem(null)}
          onSaved={() => {
            invalidate()
            setEditingItem(null)
          }}
        />
      )}
    </div>
  )
}

function EditItemModal({
  item,
  onClose,
  onSaved,
}: {
  item: ShoppingItem
  onClose: () => void
  onSaved: () => void
}) {
  const [name, setName] = useState(item.name)
  const [quantity, setQuantity] = useState(item.quantity)
  const [error, setError] = useState<string | null>(null)

  const mutation = useMutation({
    mutationFn: () =>
      updateShoppingItem(item.id, { name: name.trim(), quantity }),
    onSuccess: onSaved,
    onError: (err) => setError(getErrorMessage(err)),
  })

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    mutation.mutate()
  }

  return (
    <Modal title="Izmena stavke" onClose={onClose}>
      <form onSubmit={handleSubmit} className="space-y-4">
        {error && <ErrorBanner message={error} />}

        <Field label="Naziv">
          <TextInput
            type="text"
            required
            maxLength={200}
            autoFocus
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </Field>

        <div>
          <span className="mb-1.5 block text-[13px] font-semibold text-ink-soft">
            Količina
          </span>
          <QuantityStepper value={quantity} onChange={setQuantity} />
        </div>

        <div className="flex justify-end gap-2 pt-2">
          <Button type="button" variant="ghost" onClick={onClose}>
            Otkaži
          </Button>
          <Button
            type="submit"
            loading={mutation.isPending}
            disabled={!name.trim()}
          >
            Sačuvaj
          </Button>
        </div>
      </form>
    </Modal>
  )
}

function getErrorMessage(err: unknown) {
  return err instanceof ApiError
    ? err.message
    : 'Nešto je pošlo naopako. Pokušaj ponovo.'
}

import { ShoppingBasket } from 'lucide-react'
import { EmptyState } from '../components/ui/EmptyState'

export function ShoppingPage() {
  return (
    <div className="space-y-5">
      <h1 className="text-[1.7rem] font-bold tracking-tight text-ink animate-fade-up">
        Kupovina
      </h1>
      <EmptyState
        icon={<ShoppingBasket />}
        title="Uskoro"
        description="Zajednička lista za kupovinu stiže u sledećem koraku (F4)."
      />
    </div>
  )
}

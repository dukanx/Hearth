import { type FormEvent, useState } from 'react'
import type { HouseholdTask, TaskPriority } from '../../types/task'
import { TASK_PRIORITIES } from '../../types/task'
import { PRIORITY_LABELS, toDateInputValue } from './task-utils'
import { Button } from '../../components/ui/Button'
import { Field, Select, TextArea, TextInput } from '../../components/ui/Field'

export interface TaskFormValues {
  title: string
  description: string
  priority: TaskPriority
  dueDate: string
  assignedToUserId: string
}

interface TaskFormProps {
  initial?: HouseholdTask
  memberOptions: { id: string; label: string }[]
  showAssignee?: boolean
  submitLabel: string
  isSubmitting?: boolean
  onSubmit: (values: TaskFormValues) => void
  onCancel: () => void
}

function buildInitial(initial?: HouseholdTask): TaskFormValues {
  return {
    title: initial?.title ?? '',
    description: initial?.description ?? '',
    priority: initial?.priority ?? 'Medium',
    dueDate: toDateInputValue(initial?.dueDate ?? null),
    assignedToUserId: initial?.assignedToUserId ?? '',
  }
}

export function TaskForm({
  initial,
  memberOptions,
  showAssignee = false,
  submitLabel,
  isSubmitting = false,
  onSubmit,
  onCancel,
}: TaskFormProps) {
  const [values, setValues] = useState<TaskFormValues>(() => buildInitial(initial))

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    onSubmit(values)
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <Field label="Naziv">
        <TextInput
          type="text"
          required
          maxLength={200}
          autoFocus
          placeholder="npr. Usisati dnevnu sobu"
          value={values.title}
          onChange={(e) => setValues((v) => ({ ...v, title: e.target.value }))}
        />
      </Field>

      <Field label="Opis">
        <TextArea
          rows={3}
          maxLength={2000}
          placeholder="Detalji (nije obavezno)"
          value={values.description}
          onChange={(e) =>
            setValues((v) => ({ ...v, description: e.target.value }))
          }
        />
      </Field>

      <div className="grid gap-4 sm:grid-cols-2">
        <Field label="Prioritet">
          <Select
            value={values.priority}
            onChange={(e) =>
              setValues((v) => ({
                ...v,
                priority: e.target.value as TaskPriority,
              }))
            }
          >
            {TASK_PRIORITIES.map((priority) => (
              <option key={priority} value={priority}>
                {PRIORITY_LABELS[priority]}
              </option>
            ))}
          </Select>
        </Field>

        <Field label="Rok">
          <TextInput
            type="date"
            value={values.dueDate}
            onChange={(e) =>
              setValues((v) => ({ ...v, dueDate: e.target.value }))
            }
          />
        </Field>
      </div>

      {showAssignee && (
        <Field label="Dodeli">
          <Select
            value={values.assignedToUserId}
            onChange={(e) =>
              setValues((v) => ({ ...v, assignedToUserId: e.target.value }))
            }
          >
            <option value="">Nikome</option>
            {memberOptions.map((member) => (
              <option key={member.id} value={member.id}>
                {member.label}
              </option>
            ))}
          </Select>
        </Field>
      )}

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="ghost" onClick={onCancel}>
          Otkaži
        </Button>
        <Button
          type="submit"
          loading={isSubmitting}
          disabled={!values.title.trim()}
        >
          {submitLabel}
        </Button>
      </div>
    </form>
  )
}

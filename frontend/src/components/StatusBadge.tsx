import clsx from 'clsx'

interface Props {
  status: string
  size?: 'sm' | 'md'
}

const orderStatusColors: Record<string, string> = {
  Pending: 'bg-yellow-100 text-yellow-800 border-yellow-200',
  Processing: 'bg-blue-100 text-blue-800 border-blue-200',
  Delivered: 'bg-green-100 text-green-800 border-green-200',
  Cancelled: 'bg-red-100 text-red-800 border-red-200',
  Healthy: 'bg-green-100 text-green-800 border-green-200',
  Warning: 'bg-yellow-100 text-yellow-800 border-yellow-200',
  Failed: 'bg-red-100 text-red-800 border-red-200',
  Success: 'bg-green-100 text-green-800 border-green-200',
  InProgress: 'bg-blue-100 text-blue-800 border-blue-200',
}

export function StatusBadge({ status, size = 'sm' }: Props) {
  return (
    <span
      className={clsx(
        'inline-flex items-center rounded-full border font-medium',
        size === 'sm' ? 'px-2 py-0.5 text-xs' : 'px-3 py-1 text-sm',
        orderStatusColors[status] ?? 'bg-gray-100 text-gray-800 border-gray-200',
      )}
    >
      {status}
    </span>
  )
}

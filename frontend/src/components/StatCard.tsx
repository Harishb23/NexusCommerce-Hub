import clsx from 'clsx'
import type { ReactNode } from 'react'

interface Props {
  title: string
  value: string | number
  subtitle?: string
  icon?: ReactNode
  variant?: 'default' | 'warning' | 'danger' | 'success'
}

const variantStyles = {
  default: 'border-gray-100',
  warning: 'border-l-4 border-l-yellow-400',
  danger: 'border-l-4 border-l-red-400',
  success: 'border-l-4 border-l-green-400',
}

export function StatCard({ title, value, subtitle, icon, variant = 'default' }: Props) {
  return (
    <div className={clsx('card', variantStyles[variant])}>
      <div className="flex items-start justify-between">
        <div className="flex-1">
          <p className="text-sm font-medium text-gray-500">{title}</p>
          <p className="mt-1 text-3xl font-bold text-gray-900">{value}</p>
          {subtitle && <p className="mt-1 text-sm text-gray-500">{subtitle}</p>}
        </div>
        {icon && (
          <div className="ml-4 flex h-12 w-12 items-center justify-center rounded-lg bg-blue-50 text-blue-600">
            {icon}
          </div>
        )}
      </div>
    </div>
  )
}

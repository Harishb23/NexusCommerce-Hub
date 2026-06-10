import { useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { useOrder, useUpdateOrderStatus } from '@/hooks/useOrders'
import { StatusBadge } from '@/components/StatusBadge'
import { LoadingSpinner } from '@/components/LoadingSpinner'
import { ErrorMessage } from '@/components/ErrorMessage'

const STATUSES = ['Pending', 'Processing', 'Delivered', 'Cancelled']

export function OrderDetail() {
  const { id } = useParams<{ id: string }>()
  const { data: order, isLoading, isError, error, refetch } = useOrder(Number(id))
  const { mutate: updateStatus, isPending } = useUpdateOrderStatus()
  const [selectedStatus, setSelectedStatus] = useState('')

  if (isLoading) return <LoadingSpinner message="Loading order..." />
  if (isError) return <ErrorMessage message={(error as Error).message} onRetry={refetch} />
  if (!order) return <ErrorMessage message="Order not found" />

  const handleUpdate = () => {
    if (!selectedStatus || selectedStatus === order.status) return
    updateStatus({ id: order.id, status: selectedStatus })
    setSelectedStatus('')
  }

  return (
    <div className="space-y-5">
      <div className="flex items-center gap-3">
        <Link to="/orders" className="text-sm text-blue-600 hover:text-blue-700">
          ← Back to Orders
        </Link>
      </div>

      <div className="flex items-start justify-between">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">
            Order {order.externalOrderId}
          </h2>
          <p className="mt-1 text-sm text-gray-500">
            Created {new Date(order.createdAt).toLocaleString()}
          </p>
        </div>
        <StatusBadge status={order.status} size="md" />
      </div>

      <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
        <div className="card space-y-4">
          <h3 className="font-semibold text-gray-900">Order Details</h3>
          <dl className="space-y-3">
            {[
              { label: 'Order ID', value: order.externalOrderId },
              { label: 'Channel', value: order.channel },
              { label: 'Customer', value: order.customerName },
              {
                label: 'Total Amount',
                value: `$${order.totalAmount.toFixed(2)}`,
              },
              {
                label: 'Created At',
                value: new Date(order.createdAt).toLocaleString(),
              },
            ].map(({ label, value }) => (
              <div key={label} className="flex justify-between text-sm">
                <dt className="text-gray-500">{label}</dt>
                <dd className="font-medium text-gray-900">{value}</dd>
              </div>
            ))}
          </dl>
        </div>

        <div className="card space-y-4">
          <h3 className="font-semibold text-gray-900">Update Status</h3>
          <div className="space-y-3">
            <select
              className="select"
              value={selectedStatus || order.status}
              onChange={(e) => setSelectedStatus(e.target.value)}
            >
              {STATUSES.map((s) => (
                <option key={s} value={s}>{s}</option>
              ))}
            </select>
            <button
              className="btn-primary w-full"
              onClick={handleUpdate}
              disabled={isPending || !selectedStatus || selectedStatus === order.status}
            >
              {isPending ? 'Updating...' : 'Update Status'}
            </button>
          </div>

          <div className="rounded-lg bg-gray-50 p-3">
            <p className="text-xs font-medium text-gray-500 uppercase tracking-wide mb-2">
              Status Flow
            </p>
            <div className="flex items-center gap-1 flex-wrap">
              {STATUSES.map((s, i) => (
                <span key={s} className="flex items-center gap-1">
                  <StatusBadge status={s} size="sm" />
                  {i < STATUSES.length - 1 && (
                    <span className="text-gray-300 text-xs">→</span>
                  )}
                </span>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

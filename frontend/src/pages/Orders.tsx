import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useOrders } from '@/hooks/useOrders'
import { StatusBadge } from '@/components/StatusBadge'
import { LoadingSpinner } from '@/components/LoadingSpinner'
import { ErrorMessage } from '@/components/ErrorMessage'
import { EmptyState } from '@/components/EmptyState'

const STATUSES = ['', 'Pending', 'Processing', 'Delivered', 'Cancelled']
const CHANNELS = ['', 'Amazon', 'Noon']

export function Orders() {
  const [search, setSearch] = useState('')
  const [status, setStatus] = useState('')
  const [channel, setChannel] = useState('')
  const [page, setPage] = useState(1)

  const { data, isLoading, isError, refetch, error } = useOrders({
    search: search || undefined,
    status: status || undefined,
    channel: channel || undefined,
    page,
    pageSize: 20,
  })

  const totalPages = data ? Math.ceil(data.total / data.pageSize) : 1

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Orders</h2>
          <p className="mt-1 text-sm text-gray-500">
            {data ? `${data.total.toLocaleString()} total orders` : 'Manage all channel orders'}
          </p>
        </div>
      </div>

      {/* Filters */}
      <div className="card flex flex-wrap gap-3 p-4">
        <input
          className="input max-w-xs"
          placeholder="Search orders..."
          value={search}
          onChange={(e) => { setSearch(e.target.value); setPage(1) }}
        />
        <select
          className="select w-40"
          value={status}
          onChange={(e) => { setStatus(e.target.value); setPage(1) }}
        >
          {STATUSES.map((s) => (
            <option key={s} value={s}>{s || 'All Statuses'}</option>
          ))}
        </select>
        <select
          className="select w-36"
          value={channel}
          onChange={(e) => { setChannel(e.target.value); setPage(1) }}
        >
          {CHANNELS.map((c) => (
            <option key={c} value={c}>{c || 'All Channels'}</option>
          ))}
        </select>
        {(search || status || channel) && (
          <button
            className="btn-secondary"
            onClick={() => { setSearch(''); setStatus(''); setChannel(''); setPage(1) }}
          >
            Clear filters
          </button>
        )}
      </div>

      {/* Table */}
      {isLoading ? (
        <LoadingSpinner message="Loading orders..." />
      ) : isError ? (
        <ErrorMessage message={(error as Error).message} onRetry={refetch} />
      ) : !data?.items.length ? (
        <EmptyState
          title="No orders found"
          description={search || status || channel ? 'Try adjusting your filters.' : 'Run a sync to import orders from your channels.'}
        />
      ) : (
        <>
          <div className="table-container">
            <table className="table">
              <thead>
                <tr>
                  <th>Order ID</th>
                  <th>Channel</th>
                  <th>Customer</th>
                  <th>Total</th>
                  <th>Status</th>
                  <th>Date</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {data.items.map((order) => (
                  <tr key={order.id}>
                    <td className="font-mono text-xs font-medium text-gray-700">
                      {order.externalOrderId}
                    </td>
                    <td>
                      <span className="rounded-md bg-gray-100 px-2 py-0.5 text-xs font-medium text-gray-700">
                        {order.channel}
                      </span>
                    </td>
                    <td className="font-medium">{order.customerName}</td>
                    <td className="font-semibold text-gray-900">
                      ${order.totalAmount.toFixed(2)}
                    </td>
                    <td><StatusBadge status={order.status} /></td>
                    <td className="text-gray-500">
                      {new Date(order.createdAt).toLocaleDateString()}
                    </td>
                    <td>
                      <Link
                        to={`/orders/${order.id}`}
                        className="text-sm font-medium text-blue-600 hover:text-blue-700"
                      >
                        View →
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="flex items-center justify-between">
              <p className="text-sm text-gray-500">
                Page {page} of {totalPages}
              </p>
              <div className="flex gap-2">
                <button
                  className="btn-secondary"
                  onClick={() => setPage((p) => Math.max(1, p - 1))}
                  disabled={page === 1}
                >
                  Previous
                </button>
                <button
                  className="btn-secondary"
                  onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                  disabled={page === totalPages}
                >
                  Next
                </button>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  )
}

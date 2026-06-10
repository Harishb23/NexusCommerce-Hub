import { useState } from 'react'
import { useInventory, useInventoryStats } from '@/hooks/useInventory'
import { StatCard } from '@/components/StatCard'
import { LoadingSpinner } from '@/components/LoadingSpinner'
import { ErrorMessage } from '@/components/ErrorMessage'
import { EmptyState } from '@/components/EmptyState'
import clsx from 'clsx'

export function Inventory() {
  const { data: products, isLoading, isError, refetch, error } = useInventory()
  const { data: stats } = useInventoryStats()
  const [filter, setFilter] = useState<'all' | 'low' | 'out'>('all')

  const filtered = products?.filter((p) => {
    if (filter === 'low') return p.isLowStock && p.stock > 0
    if (filter === 'out') return p.stock === 0
    return true
  })

  return (
    <div className="space-y-5">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Inventory</h2>
        <p className="mt-1 text-sm text-gray-500">Monitor stock levels across all channels</p>
      </div>

      {stats && (
        <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
          <StatCard title="Total Products" value={stats.totalProducts} />
          <StatCard
            title="Low Stock"
            value={stats.lowStockCount}
            variant={stats.lowStockCount > 0 ? 'warning' : 'default'}
          />
          <StatCard
            title="Out of Stock"
            value={stats.outOfStockCount}
            variant={stats.outOfStockCount > 0 ? 'danger' : 'default'}
          />
          <StatCard
            title="Inventory Value"
            value={`$${stats.totalInventoryValue.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 })}`}
            variant="success"
          />
        </div>
      )}

      {/* Filter Tabs */}
      <div className="flex gap-1 rounded-lg bg-gray-100 p-1 w-fit">
        {(['all', 'low', 'out'] as const).map((f) => (
          <button
            key={f}
            onClick={() => setFilter(f)}
            className={clsx(
              'rounded-md px-4 py-1.5 text-sm font-medium transition-colors',
              filter === f
                ? 'bg-white text-gray-900 shadow-sm'
                : 'text-gray-600 hover:text-gray-900',
            )}
          >
            {f === 'all' ? 'All Products' : f === 'low' ? 'Low Stock' : 'Out of Stock'}
          </button>
        ))}
      </div>

      {isLoading ? (
        <LoadingSpinner message="Loading inventory..." />
      ) : isError ? (
        <ErrorMessage message={(error as Error).message} onRetry={refetch} />
      ) : !filtered?.length ? (
        <EmptyState
          title="No products found"
          description="Run a sync to import products from your channels."
        />
      ) : (
        <div className="table-container">
          <table className="table">
            <thead>
              <tr>
                <th>Product</th>
                <th>Channel</th>
                <th>Price</th>
                <th>Stock</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((product) => (
                <tr key={product.id}>
                  <td>
                    <div>
                      <p className="font-medium text-gray-900 max-w-xs truncate">{product.name}</p>
                      <p className="text-xs text-gray-400 font-mono">{product.externalProductId}</p>
                    </div>
                  </td>
                  <td>
                    <span className="rounded-md bg-gray-100 px-2 py-0.5 text-xs font-medium">
                      {product.channel}
                    </span>
                  </td>
                  <td className="font-semibold">${product.price.toFixed(2)}</td>
                  <td>
                    <span
                      className={clsx(
                        'font-bold',
                        product.stock === 0
                          ? 'text-red-600'
                          : product.isLowStock
                            ? 'text-yellow-600'
                            : 'text-green-600',
                      )}
                    >
                      {product.stock}
                    </span>
                  </td>
                  <td>
                    {product.stock === 0 ? (
                      <span className="rounded-full bg-red-100 border border-red-200 px-2 py-0.5 text-xs font-medium text-red-700">
                        Out of Stock
                      </span>
                    ) : product.isLowStock ? (
                      <span className="rounded-full bg-yellow-100 border border-yellow-200 px-2 py-0.5 text-xs font-medium text-yellow-700">
                        Low Stock
                      </span>
                    ) : (
                      <span className="rounded-full bg-green-100 border border-green-200 px-2 py-0.5 text-xs font-medium text-green-700">
                        In Stock
                      </span>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}

import { Link } from 'react-router-dom'
import { useDashboard } from '@/hooks/useDashboard'
import { StatCard } from '@/components/StatCard'
import { StatusBadge } from '@/components/StatusBadge'
import { LoadingSpinner } from '@/components/LoadingSpinner'
import { ErrorMessage } from '@/components/ErrorMessage'

export function Dashboard() {
  const { data, isLoading, isError, refetch, error } = useDashboard()

  if (isLoading) return <LoadingSpinner message="Loading dashboard..." />
  if (isError) return <ErrorMessage message={(error as Error).message} onRetry={refetch} />

  const stats = data!

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Dashboard</h2>
        <p className="mt-1 text-sm text-gray-500">
          Real-time overview of your commerce operations
        </p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <StatCard
          title="Total Orders"
          value={stats.totalOrders.toLocaleString()}
          icon={<span className="text-xl">📦</span>}
        />
        <StatCard
          title="Pending Orders"
          value={stats.pendingOrders.toLocaleString()}
          variant={stats.pendingOrders > 20 ? 'warning' : 'default'}
          icon={<span className="text-xl">⏳</span>}
        />
        <StatCard
          title="Completed Orders"
          value={stats.completedOrders.toLocaleString()}
          variant="success"
          icon={<span className="text-xl">✅</span>}
        />
        <StatCard
          title="Total Revenue"
          value={`$${stats.totalRevenue.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`}
          icon={<span className="text-xl">💰</span>}
        />
        <StatCard
          title="Low Inventory Alerts"
          value={stats.lowInventoryAlerts.toLocaleString()}
          variant={stats.lowInventoryAlerts > 0 ? 'warning' : 'default'}
          icon={<span className="text-xl">⚠️</span>}
        />
        <StatCard
          title="Failed Syncs"
          value={stats.failedSyncs.toLocaleString()}
          variant={stats.failedSyncs > 0 ? 'danger' : 'default'}
          icon={<span className="text-xl">🔴</span>}
        />
      </div>

      {/* Integration Health */}
      <div className="card">
        <div className="mb-4 flex items-center justify-between">
          <h3 className="text-base font-semibold text-gray-900">Integration Health</h3>
          <Link to="/integrations" className="text-sm text-blue-600 hover:text-blue-700">
            View all →
          </Link>
        </div>

        {stats.integrationStatuses.length === 0 ? (
          <p className="text-sm text-gray-500">
            No integrations configured yet.{' '}
            <Link to="/sync" className="text-blue-600 hover:underline">
              Run a sync
            </Link>{' '}
            to initialize.
          </p>
        ) : (
          <div className="divide-y divide-gray-50">
            {stats.integrationStatuses.map((integration) => (
              <div
                key={integration.id}
                className="flex items-center justify-between py-3"
              >
                <div className="flex items-center gap-3">
                  <div
                    className={`h-2.5 w-2.5 rounded-full ${
                      integration.status === 'Healthy'
                        ? 'bg-green-400'
                        : integration.status === 'Warning'
                          ? 'bg-yellow-400'
                          : 'bg-red-400'
                    }`}
                  />
                  <span className="font-medium text-gray-800">{integration.channel}</span>
                </div>
                <div className="flex items-center gap-4 text-right">
                  <div className="hidden text-xs text-gray-500 sm:block">
                    <p>{integration.responseTime.toFixed(0)}ms</p>
                    <p>{integration.failureCount} failures</p>
                  </div>
                  <StatusBadge status={integration.status} />
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Quick Actions */}
      <div className="card">
        <h3 className="mb-4 text-base font-semibold text-gray-900">Quick Actions</h3>
        <div className="flex flex-wrap gap-3">
          <Link to="/sync" className="btn-primary">
            🔄 Sync Now
          </Link>
          <Link to="/orders?status=Pending" className="btn-secondary">
            📦 View Pending Orders
          </Link>
          <Link to="/inventory/low-stock" className="btn-secondary">
            ⚠️ Low Stock Items
          </Link>
          <Link to="/analytics" className="btn-secondary">
            📈 View Analytics
          </Link>
        </div>
      </div>
    </div>
  )
}

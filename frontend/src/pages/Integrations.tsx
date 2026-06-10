import { useIntegrationHealth } from '@/hooks/useIntegrations'
import { StatusBadge } from '@/components/StatusBadge'
import { LoadingSpinner } from '@/components/LoadingSpinner'
import { ErrorMessage } from '@/components/ErrorMessage'
import { EmptyState } from '@/components/EmptyState'
import { Link } from 'react-router-dom'

export function Integrations() {
  const { data: integrations, isLoading, isError, refetch, error } = useIntegrationHealth()

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Integration Health</h2>
          <p className="mt-1 text-sm text-gray-500">
            Monitor external API connections and sync health
          </p>
        </div>
        <Link to="/sync" className="btn-primary">
          🔄 Trigger Sync
        </Link>
      </div>

      {isLoading ? (
        <LoadingSpinner message="Loading integration status..." />
      ) : isError ? (
        <ErrorMessage message={(error as Error).message} onRetry={refetch} />
      ) : !integrations?.length ? (
        <EmptyState
          title="No integrations found"
          description="Run your first sync to initialize integration health tracking."
          action={<Link to="/sync" className="btn-primary">Run Sync →</Link>}
        />
      ) : (
        <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
          {integrations.map((integration) => (
            <div key={integration.id} className="card space-y-4">
              <div className="flex items-start justify-between">
                <div className="flex items-center gap-3">
                  <div
                    className={`h-3 w-3 rounded-full ${
                      integration.status === 'Healthy'
                        ? 'bg-green-400'
                        : integration.status === 'Warning'
                          ? 'bg-yellow-400 animate-pulse'
                          : 'bg-red-400 animate-pulse'
                    }`}
                  />
                  <div>
                    <p className="font-semibold text-gray-900 text-lg">{integration.channel}</p>
                    <p className="text-xs text-gray-500">
                      Last checked: {new Date(integration.lastChecked).toLocaleString()}
                    </p>
                  </div>
                </div>
                <StatusBadge status={integration.status} size="md" />
              </div>

              <div className="grid grid-cols-2 gap-3">
                <div className="rounded-lg bg-gray-50 p-3 text-center">
                  <p className="text-2xl font-bold text-gray-900">
                    {integration.responseTime.toFixed(0)}
                    <span className="text-sm font-normal text-gray-500">ms</span>
                  </p>
                  <p className="text-xs text-gray-500 mt-0.5">Response Time</p>
                </div>
                <div className="rounded-lg bg-gray-50 p-3 text-center">
                  <p
                    className={`text-2xl font-bold ${
                      integration.failureCount > 0 ? 'text-red-600' : 'text-gray-900'
                    }`}
                  >
                    {integration.failureCount}
                  </p>
                  <p className="text-xs text-gray-500 mt-0.5">Failure Count</p>
                </div>
              </div>

              {integration.status !== 'Healthy' && (
                <div className={`rounded-lg p-3 text-sm ${
                  integration.status === 'Warning'
                    ? 'bg-yellow-50 text-yellow-800 border border-yellow-200'
                    : 'bg-red-50 text-red-800 border border-red-200'
                }`}>
                  {integration.status === 'Warning'
                    ? `⚠️ ${integration.failureCount} recent failures detected. Monitor closely.`
                    : `🔴 Integration is failing. ${integration.failureCount} consecutive failures.`}
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

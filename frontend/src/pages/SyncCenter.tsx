import { useSyncLogs, useTriggerSync, useRetrySync } from '@/hooks/useSync'
import { StatusBadge } from '@/components/StatusBadge'
import { LoadingSpinner } from '@/components/LoadingSpinner'
import { ErrorMessage } from '@/components/ErrorMessage'
import { EmptyState } from '@/components/EmptyState'
import { useState } from 'react'

export function SyncCenter() {
  const { data: logs, isLoading, isError, refetch, error } = useSyncLogs()
  const { mutate: triggerSync, isPending: isSyncing, data: syncResult } = useTriggerSync()
  const { mutate: retrySync, isPending: isRetrying } = useRetrySync()
  const [selectedChannel, setSelectedChannel] = useState('')

  return (
    <div className="space-y-5">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Sync Center</h2>
        <p className="mt-1 text-sm text-gray-500">
          Manage data synchronization from external channels
        </p>
      </div>

      {/* Sync Controls */}
      <div className="card space-y-4">
        <h3 className="font-semibold text-gray-900">Trigger Sync</h3>
        <div className="flex flex-wrap items-end gap-3">
          <div>
            <label className="mb-1 block text-sm font-medium text-gray-700">
              Channel (optional)
            </label>
            <select
              className="select w-48"
              value={selectedChannel}
              onChange={(e) => setSelectedChannel(e.target.value)}
            >
              <option value="">All Channels</option>
              <option value="amazon">Amazon</option>
              <option value="noon">Noon</option>
            </select>
          </div>
          <button
            className="btn-primary"
            onClick={() => triggerSync(selectedChannel || undefined)}
            disabled={isSyncing}
          >
            {isSyncing ? (
              <>
                <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />
                Syncing...
              </>
            ) : (
              '🔄 Sync Now'
            )}
          </button>
        </div>

        {syncResult && (
          <div
            className={`rounded-lg p-4 border text-sm ${
              syncResult.success
                ? 'bg-green-50 border-green-200 text-green-800'
                : 'bg-yellow-50 border-yellow-200 text-yellow-800'
            }`}
          >
            <p className="font-semibold">{syncResult.success ? '✅' : '⚠️'} {syncResult.message}</p>
            <div className="mt-2 flex gap-4 text-xs">
              <span>Orders synced: <strong>{syncResult.ordersSynced}</strong></span>
              <span>Products synced: <strong>{syncResult.productsSynced}</strong></span>
            </div>
            {syncResult.errors.length > 0 && (
              <ul className="mt-2 space-y-1 text-xs">
                {syncResult.errors.map((e, i) => (
                  <li key={i} className="text-red-700">• {e}</li>
                ))}
              </ul>
            )}
          </div>
        )}
      </div>

      {/* Sync History */}
      <div className="space-y-3">
        <h3 className="font-semibold text-gray-900">Sync History</h3>

        {isLoading ? (
          <LoadingSpinner message="Loading sync logs..." />
        ) : isError ? (
          <ErrorMessage message={(error as Error).message} onRetry={refetch} />
        ) : !logs?.length ? (
          <EmptyState
            title="No sync history"
            description="Sync history will appear here after your first sync."
          />
        ) : (
          <div className="table-container">
            <table className="table">
              <thead>
                <tr>
                  <th>Channel</th>
                  <th>Status</th>
                  <th>Message</th>
                  <th>Time</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {logs.map((log) => (
                  <tr key={log.id}>
                    <td className="font-medium">{log.channel}</td>
                    <td><StatusBadge status={log.status} /></td>
                    <td className="max-w-sm truncate text-gray-600">{log.message}</td>
                    <td className="text-gray-500 whitespace-nowrap">
                      {new Date(log.createdAt).toLocaleString()}
                    </td>
                    <td>
                      {log.status === 'Failed' && (
                        <button
                          className="btn-secondary py-1 text-xs"
                          onClick={() => retrySync(log.id)}
                          disabled={isRetrying}
                        >
                          Retry
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  )
}

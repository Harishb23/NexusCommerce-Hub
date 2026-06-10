import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { getFailedSyncs, getSyncLogs, retrySync, triggerSync } from '@/services/api'

export const useSyncLogs = () =>
  useQuery({
    queryKey: ['sync', 'logs'],
    queryFn: getSyncLogs,
  })

export const useFailedSyncs = () =>
  useQuery({
    queryKey: ['sync', 'failed'],
    queryFn: getFailedSyncs,
  })

export const useTriggerSync = () => {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (channel?: string) => triggerSync(channel),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['sync'] })
      qc.invalidateQueries({ queryKey: ['orders'] })
      qc.invalidateQueries({ queryKey: ['inventory'] })
      qc.invalidateQueries({ queryKey: ['dashboard'] })
      qc.invalidateQueries({ queryKey: ['integrations'] })
    },
  })
}

export const useRetrySync = () => {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: number) => retrySync(id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['sync'] })
      qc.invalidateQueries({ queryKey: ['dashboard'] })
    },
  })
}

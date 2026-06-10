import { useQuery } from '@tanstack/react-query'
import { getIntegrationHealth } from '@/services/api'

export const useIntegrationHealth = () =>
  useQuery({
    queryKey: ['integrations', 'health'],
    queryFn: getIntegrationHealth,
    refetchInterval: 30_000,
  })

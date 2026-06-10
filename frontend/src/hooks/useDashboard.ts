import { useQuery } from '@tanstack/react-query'
import { getDashboard } from '@/services/api'

export const useDashboard = () =>
  useQuery({
    queryKey: ['dashboard'],
    queryFn: getDashboard,
    refetchInterval: 60_000,
  })

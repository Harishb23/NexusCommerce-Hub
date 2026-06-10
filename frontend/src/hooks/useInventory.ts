import { useQuery } from '@tanstack/react-query'
import { getInventory, getInventoryStats, getLowStock } from '@/services/api'

export const useInventory = () =>
  useQuery({
    queryKey: ['inventory'],
    queryFn: getInventory,
  })

export const useLowStock = () =>
  useQuery({
    queryKey: ['inventory', 'low-stock'],
    queryFn: getLowStock,
  })

export const useInventoryStats = () =>
  useQuery({
    queryKey: ['inventory', 'stats'],
    queryFn: getInventoryStats,
  })

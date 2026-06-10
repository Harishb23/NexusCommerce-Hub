import { useQuery } from '@tanstack/react-query'
import {
  getInventoryHealth,
  getOrderAnalytics,
  getRevenueAnalytics,
} from '@/services/api'

export const useOrderAnalytics = () =>
  useQuery({
    queryKey: ['analytics', 'orders'],
    queryFn: getOrderAnalytics,
  })

export const useRevenueAnalytics = () =>
  useQuery({
    queryKey: ['analytics', 'revenue'],
    queryFn: getRevenueAnalytics,
  })

export const useInventoryHealth = () =>
  useQuery({
    queryKey: ['analytics', 'inventory-health'],
    queryFn: getInventoryHealth,
  })

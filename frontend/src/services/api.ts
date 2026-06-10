import axios from 'axios'
import type {
  DashboardStats,
  IntegrationHealth,
  InventoryHealth,
  InventoryStats,
  Order,
  OrderAnalytics,
  PagedResult,
  Product,
  RevenueAnalytics,
  SyncLog,
  SyncResult,
} from '@/types'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? '/api',
  timeout: 30_000,
  headers: { 'Content-Type': 'application/json' },
})

api.interceptors.response.use(
  (res) => res,
  (error) => {
    const message =
      error.response?.data?.message ??
      error.message ??
      'An unexpected error occurred'
    return Promise.reject(new Error(message))
  },
)

// Dashboard
export const getDashboard = () =>
  api.get<DashboardStats>('/dashboard').then((r) => r.data)

// Orders
export const getOrders = (params: {
  search?: string
  status?: string
  channel?: string
  page?: number
  pageSize?: number
}) => api.get<PagedResult<Order>>('/orders', { params }).then((r) => r.data)

export const getOrder = (id: number) =>
  api.get<Order>(`/orders/${id}`).then((r) => r.data)

export const updateOrderStatus = (id: number, status: string) =>
  api.put<Order>(`/orders/${id}/status`, { status }).then((r) => r.data)

// Inventory
export const getInventory = () =>
  api.get<Product[]>('/inventory').then((r) => r.data)

export const getLowStock = () =>
  api.get<Product[]>('/inventory/low-stock').then((r) => r.data)

export const getInventoryStats = () =>
  api.get<InventoryStats>('/inventory/stats').then((r) => r.data)

// Integrations
export const getIntegrationHealth = () =>
  api.get<IntegrationHealth[]>('/integrations/health').then((r) => r.data)

// Sync
export const triggerSync = (channel?: string) =>
  api.post<SyncResult>('/sync', { channel }).then((r) => r.data)

export const getSyncLogs = () =>
  api.get<SyncLog[]>('/sync/logs').then((r) => r.data)

export const getFailedSyncs = () =>
  api.get<SyncLog[]>('/sync/failed').then((r) => r.data)

export const retrySync = (id: number) =>
  api.post<SyncResult>(`/sync/retry/${id}`).then((r) => r.data)

// Analytics
export const getOrderAnalytics = () =>
  api.get<OrderAnalytics>('/analytics/orders').then((r) => r.data)

export const getRevenueAnalytics = () =>
  api.get<RevenueAnalytics>('/analytics/revenue').then((r) => r.data)

export const getInventoryHealth = () =>
  api.get<InventoryHealth>('/analytics/inventory-health').then((r) => r.data)

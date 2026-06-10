export interface Order {
  id: number
  externalOrderId: string
  channel: string
  customerName: string
  totalAmount: number
  status: string
  createdAt: string
}

export interface PagedResult<T> {
  items: T[]
  total: number
  page: number
  pageSize: number
}

export interface Product {
  id: number
  externalProductId: string
  name: string
  price: number
  stock: number
  channel: string
  createdAt: string
  isLowStock: boolean
}

export interface InventoryStats {
  totalProducts: number
  lowStockCount: number
  outOfStockCount: number
  totalInventoryValue: number
}

export interface SyncLog {
  id: number
  channel: string
  status: string
  message: string
  createdAt: string
}

export interface SyncResult {
  success: boolean
  message: string
  ordersSynced: number
  productsSynced: number
  errors: string[]
}

export interface IntegrationHealth {
  id: number
  channel: string
  status: string
  lastChecked: string
  failureCount: number
  responseTime: number
}

export interface DashboardStats {
  totalOrders: number
  pendingOrders: number
  completedOrders: number
  totalRevenue: number
  lowInventoryAlerts: number
  failedSyncs: number
  integrationStatuses: IntegrationHealth[]
}

export interface ChannelMetric {
  channel: string
  value: number
}

export interface StatusMetric {
  status: string
  count: number
}

export interface OrderAnalytics {
  ordersByChannel: ChannelMetric[]
  ordersByStatus: StatusMetric[]
}

export interface RevenueAnalytics {
  revenueByChannel: ChannelMetric[]
  totalRevenue: number
}

export interface InventoryHealth {
  totalProducts: number
  healthyStock: number
  lowStock: number
  outOfStock: number
}

export type OrderStatus = 'Pending' | 'Processing' | 'Delivered' | 'Cancelled'
export type HealthStatus = 'Healthy' | 'Warning' | 'Failed'
export type SyncStatus = 'Success' | 'Failed' | 'InProgress'

import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  ArcElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js'
import { Bar, Doughnut } from 'react-chartjs-2'
import { useInventoryHealth, useOrderAnalytics, useRevenueAnalytics } from '@/hooks/useAnalytics'
import { LoadingSpinner } from '@/components/LoadingSpinner'
import { ErrorMessage } from '@/components/ErrorMessage'

ChartJS.register(CategoryScale, LinearScale, BarElement, ArcElement, Title, Tooltip, Legend)

const CHANNEL_COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444']
const STATUS_COLORS = ['#f59e0b', '#3b82f6', '#10b981', '#ef4444']

export function Analytics() {
  const { data: orderAnalytics, isLoading: ordersLoading, error: ordersError } = useOrderAnalytics()
  const { data: revenueAnalytics, isLoading: revenueLoading, error: revenueError } = useRevenueAnalytics()
  const { data: inventoryHealth, isLoading: inventoryLoading, error: inventoryError } = useInventoryHealth()

  return (
    <div className="space-y-5">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Analytics</h2>
        <p className="mt-1 text-sm text-gray-500">
          Performance insights across all commerce channels
        </p>
      </div>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        {/* Revenue by Channel */}
        <div className="card">
          <h3 className="mb-4 font-semibold text-gray-900">Revenue by Channel</h3>
          {revenueLoading ? (
            <LoadingSpinner message="Loading revenue..." />
          ) : revenueError ? (
            <ErrorMessage message={(revenueError as Error).message} />
          ) : !revenueAnalytics?.revenueByChannel.length ? (
            <p className="py-8 text-center text-sm text-gray-500">No revenue data yet</p>
          ) : (
            <Bar
              data={{
                labels: revenueAnalytics.revenueByChannel.map((c) => c.channel),
                datasets: [
                  {
                    label: 'Revenue ($)',
                    data: revenueAnalytics.revenueByChannel.map((c) => c.value),
                    backgroundColor: CHANNEL_COLORS,
                    borderRadius: 6,
                  },
                ],
              }}
              options={{
                responsive: true,
                plugins: {
                  legend: { display: false },
                  tooltip: {
                    callbacks: {
                      label: (ctx) => `$${Number(ctx.raw).toLocaleString('en-US', { minimumFractionDigits: 2 })}`,
                    },
                  },
                },
                scales: {
                  y: { beginAtZero: true, grid: { color: '#f3f4f6' } },
                  x: { grid: { display: false } },
                },
              }}
            />
          )}
        </div>

        {/* Orders by Channel */}
        <div className="card">
          <h3 className="mb-4 font-semibold text-gray-900">Orders by Channel</h3>
          {ordersLoading ? (
            <LoadingSpinner message="Loading orders..." />
          ) : ordersError ? (
            <ErrorMessage message={(ordersError as Error).message} />
          ) : !orderAnalytics?.ordersByChannel.length ? (
            <p className="py-8 text-center text-sm text-gray-500">No order data yet</p>
          ) : (
            <Bar
              data={{
                labels: orderAnalytics.ordersByChannel.map((c) => c.channel),
                datasets: [
                  {
                    label: 'Orders',
                    data: orderAnalytics.ordersByChannel.map((c) => c.value),
                    backgroundColor: CHANNEL_COLORS,
                    borderRadius: 6,
                  },
                ],
              }}
              options={{
                responsive: true,
                plugins: { legend: { display: false } },
                scales: {
                  y: { beginAtZero: true, ticks: { stepSize: 1 }, grid: { color: '#f3f4f6' } },
                  x: { grid: { display: false } },
                },
              }}
            />
          )}
        </div>

        {/* Order Status Distribution */}
        <div className="card">
          <h3 className="mb-4 font-semibold text-gray-900">Order Status Distribution</h3>
          {ordersLoading ? (
            <LoadingSpinner message="Loading..." />
          ) : ordersError ? (
            <ErrorMessage message={(ordersError as Error).message} />
          ) : !orderAnalytics?.ordersByStatus.length ? (
            <p className="py-8 text-center text-sm text-gray-500">No data yet</p>
          ) : (
            <div className="flex items-center justify-center">
              <div className="w-64 h-64">
                <Doughnut
                  data={{
                    labels: orderAnalytics.ordersByStatus.map((s) => s.status),
                    datasets: [
                      {
                        data: orderAnalytics.ordersByStatus.map((s) => s.count),
                        backgroundColor: STATUS_COLORS,
                        borderWidth: 2,
                        borderColor: '#fff',
                      },
                    ],
                  }}
                  options={{
                    responsive: true,
                    cutout: '70%',
                    plugins: {
                      legend: {
                        position: 'bottom',
                        labels: { padding: 12, font: { size: 12 } },
                      },
                    },
                  }}
                />
              </div>
            </div>
          )}
        </div>

        {/* Inventory Health */}
        <div className="card">
          <h3 className="mb-4 font-semibold text-gray-900">Inventory Health</h3>
          {inventoryLoading ? (
            <LoadingSpinner message="Loading inventory..." />
          ) : inventoryError ? (
            <ErrorMessage message={(inventoryError as Error).message} />
          ) : !inventoryHealth ? (
            <p className="py-8 text-center text-sm text-gray-500">No data yet</p>
          ) : (
            <div className="space-y-3">
              <div className="flex items-center justify-center">
                <div className="w-56 h-56">
                  <Doughnut
                    data={{
                      labels: ['Healthy', 'Low Stock', 'Out of Stock'],
                      datasets: [
                        {
                          data: [
                            inventoryHealth.healthyStock,
                            inventoryHealth.lowStock,
                            inventoryHealth.outOfStock,
                          ],
                          backgroundColor: ['#10b981', '#f59e0b', '#ef4444'],
                          borderWidth: 2,
                          borderColor: '#fff',
                        },
                      ],
                    }}
                    options={{
                      responsive: true,
                      cutout: '65%',
                      plugins: {
                        legend: {
                          position: 'bottom',
                          labels: { padding: 12, font: { size: 12 } },
                        },
                      },
                    }}
                  />
                </div>
              </div>
              <div className="grid grid-cols-3 gap-2 text-center">
                <div className="rounded-lg bg-green-50 p-2">
                  <p className="text-xl font-bold text-green-700">{inventoryHealth.healthyStock}</p>
                  <p className="text-xs text-green-600">Healthy</p>
                </div>
                <div className="rounded-lg bg-yellow-50 p-2">
                  <p className="text-xl font-bold text-yellow-700">{inventoryHealth.lowStock}</p>
                  <p className="text-xs text-yellow-600">Low Stock</p>
                </div>
                <div className="rounded-lg bg-red-50 p-2">
                  <p className="text-xl font-bold text-red-700">{inventoryHealth.outOfStock}</p>
                  <p className="text-xs text-red-600">Out of Stock</p>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

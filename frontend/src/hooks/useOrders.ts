import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { getOrder, getOrders, updateOrderStatus } from '@/services/api'

export const useOrders = (params: {
  search?: string
  status?: string
  channel?: string
  page?: number
  pageSize?: number
}) =>
  useQuery({
    queryKey: ['orders', params],
    queryFn: () => getOrders(params),
    placeholderData: (prev) => prev,
  })

export const useOrder = (id: number) =>
  useQuery({
    queryKey: ['orders', id],
    queryFn: () => getOrder(id),
    enabled: !!id,
  })

export const useUpdateOrderStatus = () => {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ id, status }: { id: number; status: string }) =>
      updateOrderStatus(id, status),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['orders'] })
      qc.invalidateQueries({ queryKey: ['dashboard'] })
    },
  })
}

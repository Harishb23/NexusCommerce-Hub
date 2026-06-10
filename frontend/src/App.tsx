import { Routes, Route } from 'react-router-dom'
import { MainLayout } from '@/layouts/MainLayout'
import { Dashboard } from '@/pages/Dashboard'
import { Orders } from '@/pages/Orders'
import { OrderDetail } from '@/pages/OrderDetail'
import { Inventory } from '@/pages/Inventory'
import { Integrations } from '@/pages/Integrations'
import { SyncCenter } from '@/pages/SyncCenter'
import { Analytics } from '@/pages/Analytics'

export default function App() {
  return (
    <Routes>
      <Route element={<MainLayout />}>
        <Route path="/" element={<Dashboard />} />
        <Route path="/orders" element={<Orders />} />
        <Route path="/orders/:id" element={<OrderDetail />} />
        <Route path="/inventory" element={<Inventory />} />
        <Route path="/integrations" element={<Integrations />} />
        <Route path="/sync" element={<SyncCenter />} />
        <Route path="/analytics" element={<Analytics />} />
      </Route>
    </Routes>
  )
}

import { Link, NavLink, Outlet } from 'react-router-dom'
import clsx from 'clsx'

const navItems = [
  { to: '/', label: 'Dashboard', icon: '📊', end: true },
  { to: '/orders', label: 'Orders', icon: '📦' },
  { to: '/inventory', label: 'Inventory', icon: '🗄️' },
  { to: '/integrations', label: 'Integrations', icon: '🔗' },
  { to: '/sync', label: 'Sync Center', icon: '🔄' },
  { to: '/analytics', label: 'Analytics', icon: '📈' },
]

export function MainLayout() {
  return (
    <div className="flex h-screen overflow-hidden bg-gray-50">
      {/* Sidebar */}
      <aside className="flex w-64 flex-col bg-gray-900 text-white shadow-xl">
        <div className="flex h-16 items-center gap-3 border-b border-gray-700 px-5">
          <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-blue-600 text-base font-bold">
            C
          </div>
          <div>
            <p className="text-sm font-bold leading-none text-white">Commerce Ops</p>
            <p className="text-xs text-gray-400">Command Center</p>
          </div>
        </div>

        <nav className="flex-1 overflow-y-auto py-4">
          <ul className="space-y-1 px-3">
            {navItems.map(({ to, label, icon, end }) => (
              <li key={to}>
                <NavLink
                  to={to}
                  end={end}
                  className={({ isActive }) =>
                    clsx(
                      'flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors',
                      isActive
                        ? 'bg-blue-600 text-white'
                        : 'text-gray-300 hover:bg-gray-800 hover:text-white',
                    )
                  }
                >
                  <span className="text-base">{icon}</span>
                  {label}
                </NavLink>
              </li>
            ))}
          </ul>
        </nav>

        <div className="border-t border-gray-700 px-4 py-4">
          <p className="text-xs text-gray-500">v1.0.0 · Production</p>
        </div>
      </aside>

      {/* Main content */}
      <div className="flex flex-1 flex-col overflow-hidden">
        <header className="flex h-16 items-center justify-between border-b border-gray-200 bg-white px-6 shadow-sm">
          <h1 className="text-base font-semibold text-gray-800">
            Commerce Operations Command Center
          </h1>
          <div className="flex items-center gap-2">
            <div className="h-2 w-2 rounded-full bg-green-400" />
            <span className="text-sm text-gray-500">System Online</span>
          </div>
        </header>

        <main className="flex-1 overflow-y-auto p-6">
          <Outlet />
        </main>
      </div>
    </div>
  )
}

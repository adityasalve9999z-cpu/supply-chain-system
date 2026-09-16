export const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5080'
export type User = { userId: number; username: string; email: string; role: string; token: string }
export type Product = { id: number; name: string; description: string; price: number; sku: string; categoryId: number }
export type Category = { id: number; name: string; description: string }
export type Warehouse = { id: number; name: string; location: string }
export type InventoryItem = { id: number; productId: number; productName: string; warehouseId: number; warehouseName: string; quantity: number; lowStockThreshold: number; isLowStock: boolean; lastUpdated: string }

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = localStorage.getItem('flowline_token')
  const response = await fetch(`${API_URL}${path}`, { ...options, headers: { 'Content-Type': 'application/json', ...(token ? { Authorization: `Bearer ${token}` } : {}), ...options.headers } })
  if (!response.ok) { const body = await response.json().catch(() => null); throw new Error(body?.message || body?.title || `Request failed (${response.status})`) }
  return response.status === 204 ? (undefined as T) : response.json()
}
export const api = {
  login: (body: { email: string; password: string }) => request<User>('/api/auth/login', { method: 'POST', body: JSON.stringify(body) }),
  register: (body: { username: string; email: string; password: string; role: string }) => request<User>('/api/auth/register', { method: 'POST', body: JSON.stringify(body) }),
  inventory: (lowStockOnly = false) => request<InventoryItem[]>(`/api/inventory?lowStockOnly=${lowStockOnly}`),
  products: () => request<Product[]>('/api/catalog/products'),
  categories: () => request<Category[]>('/api/catalog/categories'),
  warehouses: () => request<Warehouse[]>('/api/catalog/warehouses'),
}

export const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5219'
export type User = { userId: number; username: string; email: string; role: string; token: string }
export type Product = { id: number; name: string; description: string; price: number; sku: string; categoryId: number }
export type Category = { id: number; name: string; description: string }
export type Warehouse = { id: number; name: string; location: string }
export type InventoryItem = { id: number; productId: number; productName: string; warehouseId: number; warehouseName: string; quantity: number; lowStockThreshold: number; isLowStock: boolean; lastUpdated: string }
export type AnalysisResponse = { answer: string; generatedAtUtc: string; inventoryRecords: number; lowStockRecords: number }
export type Supplier = { id: number; name: string; contactEmail: string; phone: string }
export type PurchaseOrder = { id: number; supplierId: number; orderDate: string; expectedDeliveryDate?: string; status: string; items: { productId: number; quantity: number; unitPrice: number }[] }
export type FulfillmentOrder = { id: number; customerName: string; shippingAddress: string; orderDate: string; status: string; items: { productId: number; quantity: number }[] }

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
  createCategory: (body: { name: string; description: string }) => request<Category>('/api/catalog/categories', { method: 'POST', body: JSON.stringify(body) }),
  createWarehouse: (body: { name: string; location: string }) => request<Warehouse>('/api/catalog/warehouses', { method: 'POST', body: JSON.stringify(body) }),
  createProduct: (body: { name: string; description: string; price: number; sku: string; categoryId: number }) => request<Product>('/api/catalog/products', { method: 'POST', body: JSON.stringify(body) }),
  updateProduct: (id: number, body: { name: string; description: string; price: number; sku: string; categoryId: number }) => request<Product>(`/api/catalog/products/${id}`, { method: 'PUT', body: JSON.stringify(body) }),
  deleteProduct: (id: number) => request<void>(`/api/catalog/products/${id}`, { method: 'DELETE' }),
  suppliers: () => request<Supplier[]>('/api/catalog/suppliers'),
  createSupplier: (body: { name: string; contactEmail: string; phone: string }) => request<Supplier>('/api/catalog/suppliers', { method: 'POST', body: JSON.stringify(body) }),
  adjustInventory: (body: { productId: number; warehouseId: number; quantity: number; lowStockThreshold: number; reason: string }) => request<InventoryItem>('/api/inventory/adjust', { method: 'POST', body: JSON.stringify(body) }),
  createPurchaseOrder: (body: { supplierId: number; expectedDeliveryDate?: string; items: { productId: number; quantity: number; unitPrice: number }[] }) => request<PurchaseOrder>('/api/orders/purchase', { method: 'POST', body: JSON.stringify(body) }),
  receivePurchaseOrder: (id: number, warehouseId: number) => request<void>(`/api/orders/purchase/${id}/receive`, { method: 'POST', body: JSON.stringify({ warehouseId }) }),
  createFulfillment: (body: { customerName: string; shippingAddress: string; items: { productId: number; quantity: number; warehouseId: number }[] }) => request<FulfillmentOrder>('/api/orders/fulfillment', { method: 'POST', body: JSON.stringify(body) }),
  analyze: (question: string) => request<AnalysisResponse>('/api/analysis', { method: 'POST', body: JSON.stringify({ question }) }),
}

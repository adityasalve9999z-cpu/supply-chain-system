# Flowline frontend

Production-oriented React + Vite UI for the Supply Chain API. It uses the current `@heroui/react` component package, Tailwind-compatible CSS tooling, TypeScript, and a responsive operations workspace.

## Run locally

```bash
npm install
npm run dev
```

The API defaults to `http://localhost:5080`. To use another API host, create `.env.local`:

```env
VITE_API_URL=http://localhost:5080
```

Start the ASP.NET API separately, then open the Vite URL (normally `http://localhost:5173`). Login and registration responses persist the JWT in `localStorage` and authenticated requests send it as a Bearer token.

## Build

```bash
npm run build
```

The frontend consumes `/api/auth/login`, `/api/auth/register`, `/api/inventory`, and the catalog endpoints for products, categories, and warehouses. No backend files are modified.

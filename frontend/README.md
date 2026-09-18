# Flowline frontend

Production-oriented React + Vite UI for the Supply Chain API. It uses the current `@heroui/react` component package, Tailwind-compatible CSS tooling, TypeScript, and a responsive operations workspace.

## Run locally

```bash
npm install
npm run dev
```

The API defaults to `http://localhost:5219`. To use another API host, create `.env.local`:

```env
VITE_API_URL=http://localhost:5219
```

Start the ASP.NET API separately, then open the Vite URL (normally `http://localhost:5173`). Login and registration responses persist the JWT in `localStorage` and authenticated requests send it as a Bearer token.

## Build

```bash
npm run build
```

The frontend consumes `/api/auth/login`, `/api/inventory`, the catalog endpoints, and `/api/analysis`.

## AI inventory analysis

The **AI analysis** view is a read-only assistant backed by Azure OpenAI. Configure the API process with environment variables before using it:

```powershell
$env:AzureOpenAI__Endpoint = "https://your-resource.openai.azure.com"
$env:AzureOpenAI__Deployment = "your-chat-deployment"
$env:AzureOpenAI__ApiKey = "use-a-secret-store-or-local-user-secret"
```

Do not put the API key in the React app, source control, or `VITE_*` variables. The backend loads the current inventory snapshot and sends it to Azure OpenAI; the agent cannot modify inventory or place orders.

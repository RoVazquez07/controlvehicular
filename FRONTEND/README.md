# FRONTEND

Seed React de Inspinia 2.9.3 para Control Vehicular. Conserva el CSS original (Bootstrap 4.3.1 + `style.css` + Font Awesome 4). Sin jQuery.

Usa **pnpm**. Si `pnpm` no está en el PATH, antepone `corepack` (`corepack pnpm install`).

## Cómo correrlo

```bash
cd FRONTEND
pnpm install
pnpm dev
```

Abre http://localhost:5173/

- `/` — Login (pantalla inicial)
- `/register` — Registro
- `/forgot-password` — Recuperar contraseña
- `/app` — Main view (requiere login)
- `/minor` — Minor view (requiere login)

## Scripts

- `pnpm dev` — desarrollo
- `pnpm build` — producción
- `pnpm preview` — preview del build

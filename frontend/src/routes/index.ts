/**
 * Componente de rotas principal
 * Exporta o Routes para uso no App
 */

import { useRoutes } from "react-router-dom";
import { routes } from "./routes";

export function AppRoutes() {
  return useRoutes(routes);
}
export { routes } from "./routes";
export { publicRoutes } from "./public-routes";
export { authRoutes } from "./auth-routes";

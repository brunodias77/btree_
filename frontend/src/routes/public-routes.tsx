/**
 * Rotas públicas - acessíveis sem autenticação
 */

import { type RouteObject } from 'react-router-dom';
import { ROUTES } from '../config';

import { HomePage } from '../pages/index';

export const publicRoutes: RouteObject[] = [
    {
        path: ROUTES.home,
        element: <HomePage />,
    },
];

export default publicRoutes;
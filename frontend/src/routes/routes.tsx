/**
 * Definição de todas as rotas da aplicação
 */

import { type RouteObject } from 'react-router-dom';
import { publicRoutes } from './public-routes';
import { authRoutes } from './auth-routes';
import { NotFoundPage } from '../pages/';

export const routes: RouteObject[] = [
    ...publicRoutes,
    ...authRoutes,
    {
        path: '*',
        element: <NotFoundPage />,
    },
];
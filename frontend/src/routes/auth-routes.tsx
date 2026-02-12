import { type RouteObject } from 'react-router-dom';
import { ROUTES } from '../config/routes-config';
import {
    LoginPage,
    RegisterPage,
    ForgotPasswordPage,
    ResetPasswordPage,
    ConfirmEmailPage,
} from '../pages';


export const authRoutes: RouteObject[] = [
    {
        path: ROUTES.auth.login,
        element: <LoginPage />,
    },
    {
        path: ROUTES.auth.register,
        element: <RegisterPage />,
    },
    {
        path: ROUTES.auth.forgotPassword,
        element: <ForgotPasswordPage />,
    },
    {
        path: ROUTES.auth.resetPassword,
        element: <ResetPasswordPage />,
    },
    {
        path: ROUTES.auth.confirmEmail,
        element: <ConfirmEmailPage />,
    },
];

export default authRoutes;
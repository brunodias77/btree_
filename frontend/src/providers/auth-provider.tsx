import { useEffect, type ReactNode } from 'react';
import { useAuthStore } from '../stores/auth-store';
import { storage } from '../lib/storage';
import { STORAGE_KEYS } from '../config';

interface AuthProviderProps {
    children: ReactNode;
}
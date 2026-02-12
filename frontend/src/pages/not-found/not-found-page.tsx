
import { Link } from 'react-router-dom';
import { ArrowLeft, Ban } from 'lucide-react';
import { ROUTES } from '../../config';

export function NotFoundPage() {
    return (
        <div className="flex min-h-screen flex-col items-center justify-center bg-dark p-6 text-center">
            {/* 404 Glitch Effect Container */}
            <div className="relative mb-8">
                <h1 className="select-none text-9xl font-black tracking-tighter text-zinc-800 opacity-50 blur-sm">
                    404
                </h1>
                <h1 className="absolute inset-0 select-none text-9xl font-black tracking-tighter text-white mix-blend-overlay">
                    404
                </h1>
                <div className="absolute inset-0 flex items-center justify-center">
                    <Ban className="h-32 w-32 text-brand opacity-20" />
                </div>
            </div>

            {/* Content */}
            <div className="max-w-md space-y-8">
                <div className="space-y-4">
                    <h2 className="text-3xl font-black uppercase tracking-widest text-white">
                        Página Não Encontrada
                    </h2>
                    <p className="font-mono text-zinc-400">
                        A rota solicitada não existe ou foi movida.
                        <br />
                        <span className="text-brand opacity-50">// ERROR_CODE: PAGE_NOT_FOUND</span>
                    </p>
                </div>

                {/* Divider */}
                <div className="h-px w-full bg-zinc-800" />

                {/* Actions */}
                <div className="flex flex-col gap-4">
                    <Link
                        to={ROUTES.home}
                        className="flex w-full items-center justify-center gap-2 bg-brand px-6 py-4 text-sm font-bold uppercase tracking-widest text-black transition-all hover:bg-white hover:shadow-[0_0_20px_rgba(204,243,129,0.3)]"
                    >
                        <ArrowLeft className="h-4 w-4" />
                        Voltar para o Início
                    </Link>

                    <button
                        onClick={() => window.history.back()}
                        className="flex w-full items-center justify-center gap-2 border border-zinc-700 bg-transparent px-6 py-4 text-sm font-bold uppercase tracking-widest text-zinc-400 transition-all hover:border-white hover:text-white"
                    >
                        Página Anterior
                    </button>
                </div>
            </div>

            {/* Footer / Meta */}
            <div className="fixed bottom-6 left-0 w-full text-center">
                <p className="font-mono text-xs uppercase text-zinc-700">
                    BCOMMERCE_SYSTEM_V.04 // ERROR_HANDLER
                </p>
            </div>
        </div>
    );
}

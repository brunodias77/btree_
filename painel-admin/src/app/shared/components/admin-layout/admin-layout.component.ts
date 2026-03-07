import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../sidebar-component/sidebar-component';
import { HeaderComponent } from '../header-component/header-component';

@Component({
    selector: 'app-admin-layout',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterOutlet, SidebarComponent, HeaderComponent],
    template: `
    <div class="flex h-screen bg-zinc-950 text-zinc-50 font-sans selection:bg-[#ccf381] selection:text-black overflow-hidden">
      <app-sidebar />
      <main class="flex-1 flex flex-col min-w-0 overflow-hidden relative">
        <!-- Background grid -->
        <div
          class="absolute inset-0 pointer-events-none opacity-[0.03]"
          style="background-image: linear-gradient(#fff 1px, transparent 1px), linear-gradient(90deg, #fff 1px, transparent 1px); background-size: 40px 40px"
        ></div>
        <app-header />
        <div class="flex-1 overflow-y-auto p-8 relative z-0">
          <div class="max-w-7xl mx-auto">
            <router-outlet />
          </div>
          <footer class="mt-12 border-t border-zinc-800 pt-8 pb-4">
            <div class="flex justify-between items-center text-[10px] font-mono text-zinc-600 uppercase">
              <span>btree_ Systems Inc. &copy; 2025</span>
              <span>DB Schema: v1.1 // Design: v0.4</span>
            </div>
          </footer>
        </div>
      </main>
    </div>
  `,
})
export class AdminLayoutComponent {
}

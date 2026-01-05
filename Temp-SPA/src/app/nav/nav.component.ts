import { Component, Inject, OnDestroy, OnInit, Renderer2 } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { NavigationEnd, Router } from '@angular/router';
import { filter, takeUntil } from 'rxjs';
import { AlertifyService } from '../core/services/alertify.service';
import { AuthService } from '../core/services/auth.service';
import { faSignOutAlt } from '@fortawesome/free-solid-svg-icons';
import { DestroyableComponent } from '../core/base/destroyable.component';

@Component({
    selector: 'app-nav',
    templateUrl: './nav.component.html',
    styleUrls: ['./nav.component.scss'],
    standalone: false
})
export class NavComponent extends DestroyableComponent implements OnInit, OnDestroy {
  signOutIcon = faSignOutAlt
  model: any = {};
  isMenuOpen = false;
  isUserMenuOpen = false;
  private removeDocumentClickListener?: () => void;

  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router,
    private renderer: Renderer2,
    @Inject(DOCUMENT) private document: Document) {
    super();
  }

  ngOnInit(): void {
    this.removeDocumentClickListener = this.renderer.listen(this.document, 'click', () => {
      this.isUserMenuOpen = false;
    });

    this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd), takeUntil(this.destroy$))
      .subscribe(() => {
        this.isUserMenuOpen = false;
        if (this.isMobile()) {
          this.isMenuOpen = false;
          this.applySidebarState();
        }
      });
  }

  ngOnDestroy(): void {
    this.removeDocumentClickListener?.();
    super.ngOnDestroy();
  }


  loggedIn(): any {
    return this.authService.loggedIn();
  }

  toggleSidebar(): void {
    this.isMenuOpen = !this.isMenuOpen;
    this.applySidebarState();
  }

  logout(): void {
    this.isUserMenuOpen = false;
    this.authService.logout()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.authService.clearStorage();
          this.alertify.message('logged out');
          this.router.navigate(['']);
        },
        error: () => {
          this.authService.clearStorage();
          this.alertify.error('Unable to logout');
        }
      });
  }

  toggleUserMenu(event: MouseEvent): void {
    event.stopPropagation();
    this.isUserMenuOpen = !this.isUserMenuOpen;
  }


  private applySidebarState(): void {
    const wrapper = this.document.getElementById('wrapper');
    if (!wrapper) {
      return;
    }

    if (this.isMenuOpen) {
      this.renderer.addClass(wrapper, 'toggled');
    } else {
      this.renderer.removeClass(wrapper, 'toggled');
    }
  }

  private isMobile(): boolean {
    return typeof window !== 'undefined' && window.matchMedia('(max-width: 767px)').matches;
  }
}

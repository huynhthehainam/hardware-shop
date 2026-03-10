import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import Keycloak, { KeycloakTokenParsed } from 'keycloak-js';
import {
  BehaviorSubject,
  catchError,
  from,
  iif,
  map,
  merge,
  of,
  share,
  switchMap,
  tap,
} from 'rxjs';
import { environment } from '@env/environment';
import { Menu } from '../bootstrap/menu.service';
import { isEmptyObject } from './helpers';
import { User } from './interface';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly tokenService = inject(TokenService);
  private readonly keycloak = new Keycloak(environment.keycloak);

  private user$ = new BehaviorSubject<User>({});
  private change$ = merge(
    this.tokenService.change(),
    this.tokenService.refresh().pipe(switchMap(() => this.refresh()))
  ).pipe(
    switchMap(() => this.assignUser()),
    share()
  );

  init() {
    return this.keycloak
      .init({
        onLoad: 'check-sso',
        pkceMethod: 'S256',
        silentCheckSsoRedirectUri: `${window.location.origin}/silent-check-sso.html`,
      })
      .then((authenticated: boolean) => {
        this.bindKeycloakEvents();
        if (authenticated) {
          this.syncToken();
        } else {
          this.clearAuth();
        }
      })
      .catch(() => {
        this.clearAuth();
      });
  }

  change() {
    return this.change$;
  }

  check() {
    return !!this.keycloak.authenticated && this.tokenService.valid();
  }

  login(redirectUrl?: string) {
    const redirectUri = redirectUrl ?? window.location.href;
    if (this.keycloak.authenticated) {
      return of(this.check());
    }
    return from(this.keycloak.login({ redirectUri })).pipe(map(() => this.check()));
  }

  refresh() {
    if (!this.keycloak.authenticated) {
      return of(false);
    }
    return from(this.keycloak.updateToken(30)).pipe(
      tap(() => this.syncToken()),
      map(() => this.check()),
      catchError(() => {
        this.clearAuth();
        return of(false);
      })
    );
  }

  logout() {
    const redirectUri = window.location.origin;
    return from(this.keycloak.logout({ redirectUri })).pipe(
      tap(() => this.clearAuth()),
      map(() => !this.check())
    );
  }

  user() {
    return this.user$.pipe(share());
  }

  menu() {
    console.log('call meny here');
    return iif(
      () => this.check(),
      of<Menu[]>([{
        route: 'design',
        name: 'design',
        type: 'sub',
        icon: 'color_lens',
        label: {
          color: 'azure-50',
          value: 'New'
        },
        children: [
          {
            route: 'colors',
            name: 'colors',
            type: 'link',
          },
          {
            route: 'icons',
            name: 'icons',
            type: 'link',
          }
        ],
        permissions: {
          only: [
            'ADMIN',
            'MANAGER'
          ]
        }
      }]),
      of([])
    );
  }

  private assignUser() {
    if (!this.check()) {
      return of({}).pipe(tap(user => this.user$.next(user)));
    }

    if (!isEmptyObject(this.user$.getValue())) {
      return of(this.user$.getValue());
    }

    const user = this.mapUserFromToken(this.keycloak.tokenParsed);
    return of(user).pipe(tap(nextUser => this.user$.next(nextUser)));
  }

  private bindKeycloakEvents() {
    this.keycloak.onAuthSuccess = () => this.syncToken();
    this.keycloak.onAuthRefreshSuccess = () => this.syncToken();
    this.keycloak.onAuthLogout = () => this.clearAuth();
    this.keycloak.onTokenExpired = () => {
      this.refresh().subscribe();
    };
  }

  private syncToken() {
    if (!this.keycloak.token) {
      this.clearAuth();
      return;
    }

    this.tokenService.set({
      access_token: this.keycloak.token,
      token_type: 'Bearer',
      refresh_token: this.keycloak.refreshToken,
    });

    this.user$.next(this.mapUserFromToken(this.keycloak.tokenParsed));
  }

  private clearAuth() {
    this.tokenService.clear();
    this.user$.next({});
  }

  private mapUserFromToken(token?: KeycloakTokenParsed): User {
    if (!token) {
      return {};
    }

    const tokenInfo = token as KeycloakTokenParsed & {
      email?: string;
      name?: string;
      preferred_username?: string;
      realm_access?: { roles?: string[] };
      resource_access?: Record<string, { roles?: string[] }>;
    };

    const realmRoles = Array.isArray(tokenInfo.realm_access?.roles)
      ? tokenInfo.realm_access!.roles
      : [];
    const resourceRoles = tokenInfo.resource_access
      ? Object.values(tokenInfo.resource_access)
        .flatMap((resource: any) => resource.roles ?? [])
        .filter(role => !!role)
      : [];

    return {
      id: token.sub,
      name: tokenInfo.name ?? tokenInfo.preferred_username ?? '',
      email: tokenInfo.email ?? '',
      roles: [...new Set([...realmRoles, ...resourceRoles])],
    };
  }
}

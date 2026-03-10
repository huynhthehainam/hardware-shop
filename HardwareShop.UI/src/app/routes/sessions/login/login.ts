import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { Router, RouterLink } from '@angular/router';
import { MtxButtonModule } from '@ng-matero/extensions/button';
import { TranslateModule } from '@ngx-translate/core';

import { AuthService } from '@core/authentication';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  styleUrl: './login.scss',
  imports: [
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MtxButtonModule,
    TranslateModule,
  ],
})
export class Login {
  private readonly router = inject(Router);
  private readonly auth = inject(AuthService);

  isSubmitting = false;

  login() {
    this.isSubmitting = true;
    this.auth.login(window.location.origin).subscribe({
      next: authenticated => {
        if (authenticated) {
          this.router.navigateByUrl('/');
        }
      },
      error: () => {
        this.isSubmitting = false;
      },
    });
  }
}

import { ChangeDetectionStrategy, Component, inject, isDevMode } from '@angular/core';

import { PortfolioStore } from '../../core/portfolio.store';
import { ButtonLinkComponent } from '../../shared/ui/button-link.component';
import { ProjectCardComponent } from '../../shared/ui/project-card.component';
import { UiSectionComponent } from '../../shared/ui/section-shell.component';

@Component({
  selector: 'app-home-page',
  imports: [UiSectionComponent, ButtonLinkComponent, ProjectCardComponent],
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomePageComponent {
  protected readonly store = inject(PortfolioStore);
  protected readonly showLibrary = isDevMode();
}

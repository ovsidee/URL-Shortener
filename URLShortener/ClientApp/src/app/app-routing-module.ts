import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UrlTableComponent } from './components/url-table/url-table';
import { UrlDetailsComponent } from './components/url-details/url-details';

const routes: Routes = [
  { path: '', component: UrlTableComponent, pathMatch: 'full' },
  { path: 'url/:id', component: UrlDetailsComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

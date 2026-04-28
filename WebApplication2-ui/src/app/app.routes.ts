import { Routes } from '@angular/router';
import { StudentComponent } from './component/student/student';
import { CountryComponent } from './component/country/country';


export const routes: Routes = [
  { path: '', redirectTo: 'student', pathMatch: 'full' },
  { path: 'student', component: StudentComponent },
  { path: 'country', component: CountryComponent }
];
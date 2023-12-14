import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { BooksComponent } from './books/books.component';
import { AuthorsComponent } from './authors/authors.component';
import { BookEditComponent } from './books/book-edit.component';
import { AuthorEditComponent } from './authors/author-edit.component';
import { LoginComponent } from './auth/login.component';
import { AuthGuard } from './auth/auth.guard';



const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'fetch-data', component: FetchDataComponent },
  { path: 'books', component: BooksComponent },
  { path: 'book/:id', component: BookEditComponent, canActivate: [AuthGuard] },
  { path: 'book', component: BookEditComponent, canActivate: [AuthGuard] },

  { path: 'authors', component: AuthorsComponent },

  { path: 'author/:id', component: AuthorEditComponent, canActivate: [AuthGuard] },
  { path: 'author', component: AuthorEditComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent }

];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

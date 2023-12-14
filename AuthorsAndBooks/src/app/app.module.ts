import { HttpClientModule,  HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AuthInterceptor } from './auth/auth.interceptor';


import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { BooksComponent } from './books/books.component';
import { AngularMaterialModule } from './angular-material.module';
import { MatPaginatorModule } from '@angular/material/paginator';
import { AuthorsComponent } from './authors/authors.component';

import { ReactiveFormsModule } from '@angular/forms';
import { BookEditComponent } from './books/book-edit.component';
import { AuthorEditComponent } from './authors/author-edit.component';
import { LoginComponent } from './auth/login.component';



@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    FetchDataComponent,
    NavMenuComponent,
    BooksComponent,
    AuthorsComponent,
    BookEditComponent,
    AuthorEditComponent,
    LoginComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    AngularMaterialModule,
    MatPaginatorModule,
    ReactiveFormsModule



  ],
  providers: [
    { provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

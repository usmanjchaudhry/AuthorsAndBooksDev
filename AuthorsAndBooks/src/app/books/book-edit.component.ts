import { Component, OnInit } from '@angular/core';
import { HttpClient,  HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn  } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from './../../environments/environment';
import { Book } from './book';
import { Author } from './../authors/author';


@Component({
  selector: 'app-book-edit',
  templateUrl: './book-edit.component.html',
  styleUrls: ['./book-edit.component.scss']
})
export class BookEditComponent implements OnInit {
// the view title
title?: string;

// the form model
form!: FormGroup;

// the city object to edit
book?: Book;
// the city object id, as fetched from the active route:
  // It's NULL when we're adding a new city,
  // and not NULL when we're editing an existing one.
  id?: number;
    // the countries array for the select
    authors?: Author[];

constructor(
  private activatedRoute: ActivatedRoute,
  private router: Router,
  private http: HttpClient) {
}



ngOnInit() {
  this.form = new FormGroup({
    title: new FormControl('', Validators.required),
    genre: new FormControl('', Validators.required),
    maincharacter: new FormControl('', Validators.required),
    authorId: new FormControl('', Validators.required)

  }, null, this.isDupeCity());

  this.loadData();
}

isDupeCity(): AsyncValidatorFn {
  return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
    var book = <Book>{};
    book.id = (this.id) ? this.id : 0;
    book.title = this.form.controls['title'].value;
    book.genre = String(this.form.controls['genre'].value);
    book.maincharacter = String(this.form.controls['maincharacter'].value);
    book.authorId = +this.form.controls['authorId'].value;
    var url = environment.baseUrl + 'api/Books/IsDupeCity';
    return this.http.post<boolean>(url, book).pipe(map(result => {
      return (result ? { isDupeCity: true } : null);
    }));
  }
}

loadData() {
   // load countries
   this.loadAuthors();
  // retrieve the ID from the 'id' parameter
  var idParam = this.activatedRoute.snapshot.paramMap.get('id');
  this.id = idParam ? +idParam : 0;
  if (this.id) {
    // EDIT MODE
    // fetch the city from the server
    var url = environment.baseUrl + 'api/Books/' + this.id;
    this.http.get<Book>(url).subscribe(result => {
      this.book = result;
      this.title = "Edit - " + this.book.title;
      // update the form with the city value
      this.form.patchValue(this.book);
    }, error => console.error(error));
  }
  else {
    // ADD NEW MODE
    this.title = "Create a new Book";
  }
}
loadAuthors() {
  // fetch all the countries from the server
  var url = environment.baseUrl + 'api/Authors';
  var params = new HttpParams()
    .set("pageIndex", "0")
    .set("pageSize", "9999")
    .set("sortColumn", "name");
  this.http.get<any>(url, { params }).subscribe(result => {
    this.authors = result.data;
  }, error => console.error(error));
}

onSubmit() {
  var book = (this.id) ? this.book : <Book>{};
  if (book) {
    book.title = this.form.controls['title'].value;
    book.genre = String(this.form.controls['genre'].value);
    book.maincharacter = String(this.form.controls['maincharacter'].value);
    book.authorId = +this.form.controls['authorId'].value;

    if (this.id) {
      // EDIT mode
      var url = environment.baseUrl + 'api/Books/' + book.id;
      this.http
        .put<Book>(url, book)
        .subscribe(result => {
          console.log("Book " + book!.id + " has been updated.");
          // go back to cities view
          this.router.navigate(['/books']);
        }, error => console.error(error));
    }
    else {
      // ADD NEW mode
      var url = environment.baseUrl + 'api/Books';
      this.http
        .post<Book>(url, book)
        .subscribe(result => {
          console.log("Book " + result.id + " has been created.");
          // go back to cities view
          this.router.navigate(['/books']);
        }, error => console.error(error));
    }
  }
}
}

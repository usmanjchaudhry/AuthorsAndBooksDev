import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from './../../environments/environment';
import { Author } from './author';
@Component({
  selector: 'app-author-edit',
  templateUrl: './author-edit.component.html',
  styleUrls: ['./author-edit.component.scss']
})
export class AuthorEditComponent implements OnInit {
// the view title
title?: string;
// the form model
form!: FormGroup;
// the country object to edit or create
author?: Author;
// the country object id, as fetched from the active route:
// It's NULL when we're adding a new country,
// and not NULL when we're editing an existing one.
id?: number;
// the countries array for the select
authors?: Author[];
constructor(
  private fb: FormBuilder,
  private activatedRoute: ActivatedRoute,
  private router: Router,
  private http: HttpClient) {
}
ngOnInit() {
  this.form = this.fb.group({
    name: ['',
      Validators.required,
      this.isDupeField("name")
    ],
    countryoforigin: ['', Validators.required],  // Removed isDupeField for countryoforigin
    gender: ['', Validators.required]
  });
  this.loadData();
}
loadData() {
  // retrieve the ID from the 'id' parameter
  var idParam = this.activatedRoute.snapshot.paramMap.get('id');
  this.id = idParam ? +idParam : 0;
  if (this.id) {
    // EDIT MODE
    // fetch the country from the server
    var url = environment.baseUrl + "api/Authors/" + this.id;
    this.http.get<Author>(url).subscribe(result => {
      this.author = result;
      this.title = "Edit - " + this.author.name;
      // update the form with the country value
      this.form.patchValue(this.author);
    }, error => console.error(error));
  }
  else {
    // ADD NEW MODE
    this.title = "Create a new Author";
  }
}
onSubmit() {
  var author = (this.id) ? this.author : <Author>{};
  if (author) {
    author.name = this.form.controls['name'].value;
    author.countryoforigin = this.form.controls['countryoforigin'].value;
    author.gender = this.form.controls['gender'].value;
    if (this.id) {
      // EDIT mode
      var url = environment.baseUrl + 'api/Authors/' + author.id;
      this.http
        .put<Author>(url, author)
        .subscribe(result => {
          console.log("Author " + author!.id + " has been updated.");
          // go back to countries view
          this.router.navigate(['/authors']);
        }, error => console.error(error));
    }
    else {
      // ADD NEW mode
      var url = environment.baseUrl + 'api/Authors';
      this.http
        .post<Author>(url, author)
        .subscribe(result => {
          console.log("Author " + result.id + " has been created.");
          // go back to countries view
          this.router.navigate(['/authors']);
        }, error => console.error(error));
    }
  }
}
isDupeField(fieldName: string): AsyncValidatorFn {
  return (control: AbstractControl): Observable<{
    [key: string]: any
  } | null> => {
    var params = new HttpParams()
      .set("authorId", (this.id) ? this.id.toString() : "0")
      .set("fieldName", fieldName)
      .set("fieldValue", control.value);
    var url = environment.baseUrl + 'api/Authors/IsDupeField';
    return this.http.post<boolean>(url, null, { params })
      .pipe(map(result => {
        return (result ? { isDupeField: true } : null);
      }));
  }
}
}

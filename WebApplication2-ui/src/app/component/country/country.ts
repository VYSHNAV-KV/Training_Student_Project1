import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CountryService } from '../../service/country';

@Component({
  selector: 'app-country',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './country.html'
})
export class CountryComponent implements OnInit {

  countries: any[] = [];
  editId: string | null = null;

  form!: FormGroup; // ✅ declare only

  constructor(private fb: FormBuilder, private service: CountryService) {}

  ngOnInit() {
    // ✅ initialize here
    this.form = this.fb.group({
      name: ['', Validators.required],
      sortOrder: ['', Validators.required]
    });

    this.loadCountries();
  }

  loadCountries() {
    this.service.getAll().subscribe(res => this.countries = res);
  }

  submit() {
    if (this.form.invalid) return;

    if (this.editId) {
      this.service.update(this.editId, this.form.value).subscribe(() => {
        alert('Updated ✅');
        this.afterSave();
      });
    } else {
      this.service.create(this.form.value).subscribe(() => {
        alert('Added ✅');
        this.afterSave();
      });
    }
  }

  edit(data: any) {
    this.editId = data.id;
    this.form.patchValue({
      name: data.name,
      sortOrder: data.sortOrder
    });
  }

  delete(id: string) {
    if (!confirm('Delete?')) return;

    this.service.delete(id).subscribe({
      next: () => {
        alert('Deleted ✅');
        this.loadCountries();
      },
      error: err => alert(err.error)
    });
  }

  afterSave() {
    this.reset();
    this.loadCountries();
    
  }

  reset() {
    this.form.reset();
    this.editId = null;
  }

  
}

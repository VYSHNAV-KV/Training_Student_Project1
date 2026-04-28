import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { StudentService } from '../../service/student';
import { CountryService } from '../../service/country';
import { CountryComponent } from '../country/country';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-student',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,CountryComponent],
  templateUrl: './student.html',
  styleUrl: './student.css',
})
export class StudentComponent implements OnInit {
  showModal = false;
  students: any[] = [];
  countries: any[] = [];
  editId: string | null = null;

  form!: FormGroup; // ✅ declare only
 private refresh$ = new BehaviorSubject<boolean>(false);

  constructor(
    private fb: FormBuilder,
    private studentService: StudentService,
    private countryService: CountryService
  ) {}

  ngOnInit() {
    // ✅ initialize here
    this.form = this.fb.group({
      name: ['', Validators.required],
      mobile: ['', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
      dob: ['', Validators.required],
      countryId: [null, Validators.required],
      gender: ['', Validators.required],
      isIndian: [false]
    });
    this.getRefresh().subscribe(() => {
  this.loadCountries();
});

    this.loadStudents();
    this.loadCountries();
  }

  loadStudents() {
    this.studentService.getAll().subscribe(res => this.students = res);
  }

  loadCountries() {
    this.countryService.getAll().subscribe(res => this.countries = res);
  }

  submit() {
    if (this.form.invalid) return;

    const dobValue = this.form.value.dob;

    const payload = {
      ...this.form.value,
      dob: dobValue ? new Date(dobValue).toISOString() : null // ✅ FIX
    };

    if (this.editId) {
      this.studentService.update(this.editId, payload).subscribe(() => {
        alert('Updated ✅');
        this.afterSave();
      });
    } else {
    
      this.studentService.create(payload).subscribe(() => {
        alert('Saved ✅');
        this.afterSave();
      });
    }
  }

  edit(data: any) {
    this.editId = data.id;

    const formattedDate = data.dob
      ? new Date(data.dob).toISOString().split('T')[0]
      : '';

    this.form.patchValue({
      name: data.name,
      mobile: data.mobile,
      dob: formattedDate,
      countryId: data.countryId,
      gender: data.gender,
      isIndian: data.isIndian
    });
  }

  delete(id: string) {
    if (!confirm('Delete?')) return;

    this.studentService.delete(id).subscribe(() => {
      alert('Deleted ✅');
      this.loadStudents();
    });
  }

  afterSave() {
    this.reset();
    this.loadStudents();
  }

  reset() {
    this.form.reset();
    this.editId = null;
  }







    getRefresh() {
    return this.refresh$.asObservable();
  }



openModal() {
  this.showModal = true;
}

closeModal() {
  this.showModal = false;
}
  
}


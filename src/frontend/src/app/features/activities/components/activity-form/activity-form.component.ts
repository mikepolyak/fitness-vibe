import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-activity-form',
  templateUrl: './activity-form.component.html',
  styleUrls: ['./activity-form.component.scss']
})
export class ActivityFormComponent implements OnInit {
  activityForm: FormGroup;
  isEditing = false;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.activityForm = this.fb.group({
      title: ['', Validators.required],
      type: ['', Validators.required],
      duration: ['', [Validators.required, Validators.min(1)]],
      distance: [null, Validators.min(0)],
      calories: ['', [Validators.required, Validators.min(1)]],
      date: [new Date(), Validators.required],
      intensity: ['medium', Validators.required],
      description: [''],
      status: ['planned']
    });
  }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditing = true;
      // This will be replaced with actual API call
      this.loadActivity(parseInt(id));
    }
  }

  loadActivity(id: number) {
    // This will be replaced with actual API call
    const mockActivity = {
      id: 1,
      type: 'running',
      title: 'Morning Run',
      description: 'Great morning run around the park',
      duration: 45,
      distance: 5.2,
      calories: 420,
      date: new Date(),
      intensity: 'medium',
      status: 'completed'
    };

    this.activityForm.patchValue(mockActivity);
  }

  onSubmit() {
    if (this.activityForm.valid) {
      this.isSubmitting = true;
      const activity = this.activityForm.value;

      // This will be replaced with actual API call
      console.log('Saving activity:', activity);
      
      setTimeout(() => {
        this.isSubmitting = false;
        this.router.navigate(['/activities']);
      }, 1000);
    }
  }

  onCancel() {
    this.router.navigate(['/activities']);
  }
}

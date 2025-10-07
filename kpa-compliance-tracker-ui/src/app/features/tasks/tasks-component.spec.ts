import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TasksCoponent } from './tasks-component';

describe('TasksCoponent', () => {
  let component: TasksCoponent;
  let fixture: ComponentFixture<TasksCoponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TasksCoponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TasksCoponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

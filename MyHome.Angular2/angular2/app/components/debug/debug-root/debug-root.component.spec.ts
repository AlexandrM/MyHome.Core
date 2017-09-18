/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { DebugRootComponent } from './debug-root.component';

describe('DebugRootComponent', () => {
  let component: DebugRootComponent;
  let fixture: ComponentFixture<DebugRootComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DebugRootComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DebugRootComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

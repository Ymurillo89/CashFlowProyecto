import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { of } from 'rxjs';

import { AppComponent } from './app.component';
import { CompanyService } from './services/company.service';
import { StoreService } from './services/store.service';
import { MessageService } from 'primeng/api';

// PrimeNG
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import { TableModule } from 'primeng/table';
import { FileUploadModule } from 'primeng/fileupload';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;

  const mockCompanyService = {
    getCompanies: () => of([])
  };

  const mockStoreService = {
    getStores: () => of([])
  };

  const mockMessageService = {
    add: () => {}
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppComponent],
      imports: [
        HttpClientTestingModule,
        FormsModule,
        NoopAnimationsModule,
        ButtonModule,
        InputTextModule,
        InputNumberModule,
        DropdownModule,
        TableModule,
        FileUploadModule,
        DialogModule,
        ToastModule
      ],
      providers: [
        { provide: CompanyService, useValue: mockCompanyService },
        { provide: StoreService, useValue: mockStoreService },
        { provide: MessageService, useValue: mockMessageService }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });
});
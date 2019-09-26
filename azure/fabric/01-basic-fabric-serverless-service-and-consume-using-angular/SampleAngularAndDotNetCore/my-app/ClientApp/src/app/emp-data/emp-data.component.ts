import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-emp-data',
  templateUrl: './emp-data.component.html'
})
export class EmpDataComponent {
  public employees: Employee[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Employee[]>(baseUrl + 'api/SampleData/Employees').subscribe(result => {
      this.employees = result;
    }, error => console.error(error));
  }
}

interface Employee {
  empno: string;
  ename: string;
  sal: string;
  deptno: string;
}

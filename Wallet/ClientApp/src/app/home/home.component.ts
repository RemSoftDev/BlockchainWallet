import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  constructor(private router: Router, private activatedRoute: ActivatedRoute) { }

  search(form) {
    this.router.navigate(['/search/', form.value.searchString.trim()]);
  }
}
